using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using TcPlayer.Engine;
using TcPlayer.Engine.Models;
using TcPlayer.Engine.Ui;
using TcPlayer.Infrastructure;

namespace TcPlayer.ViewModels
{
    internal class PlaylistViewModel : ViewModelBase
    {
        private readonly IDialogProvider _dialogProvider;
        private readonly Random _random;

        private FileMetaData _selected;


        public BindingList<FileMetaData> Items { get; }
        public DelegateCommand AddFilesCommand { get; }

        public DelegateCommand<PlaylistSorting> SortListCommand { get; }
        public DelegateCommand SaveListCommand { get; }
        public DelegateCommand<bool> LoadListCommand { get; }


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


        public FileMetaData Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public PlaylistViewModel(IDialogProvider dialogProvider)
        {
            _random = new Random();
            _dialogProvider = dialogProvider;
            Items = new BindingList<FileMetaData>();
            LoadListCommand = new DelegateCommand<bool>(OnLoad);
            SaveListCommand = new DelegateCommand(OnSave);
            AddFilesCommand = new DelegateCommand(OnAddFiles);
            SortListCommand = new DelegateCommand<PlaylistSorting>(OnSort);

        }

        private async void OnLoad(bool clearsList)
        {
            if (_dialogProvider.TrySelectFileDialog(Formats.PlaylistFilterString, out string selected))
            {
                var source = _dialogProvider.ShowUiBlocker();
                IEnumerable<FileMetaData> items = null;
                switch (Path.GetExtension(selected).ToLower())
                {
                    case ".m3u":
                        items = await FileInfoFactory.CreateFromM3UFile(selected, source.Token);
                        break;
                    case ".pls":
                        items = await FileInfoFactory.CreateFromPlsFile(selected, source.Token);
                        break;
                    case ".asx":
                        items = await FileInfoFactory.CreateFromAsxFile(selected, source.Token);
                        break;
                    case ".wpl":
                        items = await FileInfoFactory.CreateFromWplFile(selected, source.Token);
                        break;
                    case ".tpls":
                        items = PlaylistFormat.Load(selected);
                        break;
                }
                UpdateList(items, clearsList);
                _dialogProvider.HideUiBlocker();
            }
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
                var source = _dialogProvider.ShowUiBlocker();
                var items = await FileInfoFactory.CreateFileInfos(files, source.Token);
                UpdateList(items, false);
                _dialogProvider.HideUiBlocker();
            }
        }

        private void OnSort(PlaylistSorting obj)
        {
            List<FileMetaData> ordered = null;
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


        private void UpdateList(IEnumerable<FileMetaData>? items, bool clear)
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
            Items.ResetItem(0);
        }
    }
}
