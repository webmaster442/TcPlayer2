// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using TcPlayer.Engine;
using TcPlayer.Engine.Messages;
using TcPlayer.Engine.Models;
using TcPlayer.Engine.Ui;
using TcPlayer.Infrastructure;

namespace TcPlayer.ViewModels
{
    internal class PlaylistViewModel : ViewModelBase, IMessageClient<AppArgumentsMessage>
    {
        private readonly IDialogProvider _dialogProvider;
        private readonly IMessenger _messenger;
        private readonly Random _random;

        private PlaylistItem _selected;
        private readonly IMainWindow _mainWindow;

        public BindingList<PlaylistItem> Items { get; }
        public DelegateCommand AddFilesCommand { get; }

        public DelegateCommand<PlaylistSorting> SortListCommand { get; }
        public DelegateCommand SaveListCommand { get; }
        public DelegateCommand<bool> LoadListCommand { get; }
        public DelegateCommand<PlaylistItem> SelectedClick { get; }
        public DelegateCommand ImportITunesCommand { get; }

        public DelegateCommand<int> LoadDiscCommand { get; }
        public DelegateCommand ImportUrlCommand { get; }
        public DelegateCommand ImportDlnaCommand { get; }


        public bool TryStepNext()
        {
            int index = Items.IndexOf(Selected);
            if (index == -1) index = -1;
            if (index + 1 > Items.Count - 1)
            {
                return false;
            }
            else
            {
                Selected = Items[index + 1];
                return true;
            }
        }

        public bool TryStepBack()
        {
            int index = Items.IndexOf(Selected);
            if (index == -1) index = 0;
            if (index -1 < 0)
            {
                return false;
            }
            else
            {
                Selected = Items[index - 1];
                return true;
            }
        }


        public PlaylistItem Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public Guid MessageReciverID => Guid.NewGuid();

        public PlaylistViewModel(IDialogProvider dialogProvider, IMainWindow mainWindow, IMessenger messenger)
        {
            _random = new Random();
            _dialogProvider = dialogProvider;
            _messenger = messenger;
            _mainWindow = mainWindow;
            Items = new BindingList<PlaylistItem>();
            LoadListCommand = new DelegateCommand<bool>(OnLoad);
            SaveListCommand = new DelegateCommand(OnSave);
            AddFilesCommand = new DelegateCommand(OnAddFiles);
            SortListCommand = new DelegateCommand<PlaylistSorting>(OnSort);
            SelectedClick = new DelegateCommand<PlaylistItem>(OnSelected);
            LoadDiscCommand = new DelegateCommand<int>(OnLoadDisc);
            ImportITunesCommand = new DelegateCommand(OnImportItunes, CanImportItunes);
            ImportUrlCommand = new DelegateCommand(OnImportUrl);
            ImportDlnaCommand = new DelegateCommand(OnImportDlna);
            _messenger.SubScribe(this);
        }

        public void HandleMessage(AppArgumentsMessage message)
        {
            _mainWindow.SetMainTab(MainTab.Playlist);
            OnImportFiles(message.Arguments, true);
        }

        private async void OnImportFiles(IEnumerable<string> items, bool clearList)
        {
            var source = _mainWindow.ShowUiBlocker();
            var itemsToAdd = new List<PlaylistItem>();
            foreach (var item in items)
            {
                if (Formats.IsAudioFile(item))
                {
                    var files = await PlaylistItemFactory.CreateFileInfos(new[] { item }, source.Token);
                    itemsToAdd.AddRange(files);
                }
                else if (Formats.IsPLaylist(item))
                {
                    var playlistItems = await PlaylistItemFactory.LoadPlaylist(item, source.Token);
                    itemsToAdd.AddRange(playlistItems);
                }
                else if (item.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                    || item.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    var urls = await PlaylistItemFactory.CreateFromUrl(item, source.Token);
                    itemsToAdd.AddRange(urls);
                }
            }
            UpdateList(itemsToAdd, clearList);
            _mainWindow.HideUiBlocker();

        }

        private async void OnImportDlna(object obj)
        {
            if (_dialogProvider.TryImportDlna(out IEnumerable<string> urls))
            {
                var source = _mainWindow.ShowUiBlocker();
                var canLoaded = urls.Where(x => Formats.AudioFormats.Contains(Path.GetExtension(x)));
                var items = await PlaylistItemFactory.CreateFileInfos(canLoaded, source.Token);
                UpdateList(items, false);
                _mainWindow.HideUiBlocker();
            }
        }

        private async void OnImportUrl(object obj)
        {
            if (_dialogProvider.TryImportUrl(out string url))
            {
                var source = _mainWindow.ShowUiBlocker();
                var items = await PlaylistItemFactory.CreateFromUrl(url, source.Token);
                UpdateList(items, false);
                _mainWindow.HideUiBlocker();
            }
        }

        private bool CanImportItunes(object obj)
        {
            return ItunesXmlDb.UserHasItunesDb;
        }

        private void OnImportItunes(object obj)
        {
            if (_dialogProvider.TryImportITunes(out IEnumerable<ITunesTrack> itunesItems))
            {
                var items = PlaylistItemFactory.CreateFromITunesTracks(itunesItems);
                UpdateList(items, false);
            }
        }

        private void OnSelected(PlaylistItem obj)
        {
            if (obj == null) return;
            _messenger.SendMessage(new LoadFileMessage
            {
                File = obj.FilePath
            });
        }

        private async void OnLoad(bool clearsList)
        {
            if (_dialogProvider.TrySelectFileDialog(Formats.PlaylistFilterString, out string selected))
            {
                var source = _mainWindow.ShowUiBlocker();
                IEnumerable<PlaylistItem> items = await PlaylistItemFactory.LoadPlaylist(selected, source.Token);

                UpdateList(items, clearsList);
                _mainWindow.HideUiBlocker();
            }
        }

        private async void OnLoadDisc(int obj)
        {
            var source = _mainWindow.ShowUiBlocker();
            IEnumerable<PlaylistItem> items = await PlaylistItemFactory.LoadCd(obj, source.Token);
            UpdateList(items, true);
            _mainWindow.HideUiBlocker();
        }

        private void OnSave(object obj)
        {
            if (_dialogProvider.TrySaveFileDialog(Formats.NativePlaylistFilterString, out string selected))
            {
                PlaylistFormat.Save(selected, Items.ToArray());
            }
        }

        private async void OnAddFiles(object obj)
        {
            if (_dialogProvider.TrySelectFilesDialog(Formats.AudioFormatFilterString, out string[] files))
            {
                var source = _mainWindow.ShowUiBlocker();
                var items = await PlaylistItemFactory.CreateFileInfos(files, source.Token);
                UpdateList(items, false);
                _mainWindow.HideUiBlocker();
            }
        }

        private void OnSort(PlaylistSorting obj)
        {
            List<PlaylistItem> ordered = null;
            switch (obj)
            {
                case PlaylistSorting.Artist:
                    ordered = Items.OrderBy(i => i.Artist).ToList();
                    break;
                case PlaylistSorting.Title:
                    ordered = Items.OrderBy(i => i.Title).ToList();
                    break;
                case PlaylistSorting.Length:
                    ordered = Items.OrderBy(i => i.Length).ToList();
                    break;
                case PlaylistSorting.Path:
                    ordered = Items.OrderBy(i => i.FilePath).ToList();
                    break;
                case PlaylistSorting.Reverse:
                    ordered = Items.Reverse().ToList();
                    break;
                case PlaylistSorting.Random:
                    ordered = Items.OrderBy(i => _random.Next()).ToList();
                    break;
            }
            UpdateList(ordered, true);
        }


        private void UpdateList(IEnumerable<PlaylistItem> items, bool clear)
        {
            if (items == null) return;

            if (clear)
                Items.Clear();
            Items.RaiseListChangedEvents = false;
            foreach (var item in items)
            {
                Items.Add(item);
            }
            Items.RaiseListChangedEvents = true;
            if (Items.Count > 0)
                Items.ResetItem(0);
        }
    }
}
