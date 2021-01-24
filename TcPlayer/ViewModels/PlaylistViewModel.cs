using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public BindingList<FileMetaData> Items { get; }
        public DelegateCommand AddFilesCommand { get; }

        public DelegateCommand<PlaylistSorting> SortListCommand { get; }

        public PlaylistViewModel(IDialogProvider dialogProvider)
        {
            _random = new Random();
            _dialogProvider = dialogProvider;
            Items = new BindingList<FileMetaData>();
            AddFilesCommand = new DelegateCommand(OnAddFiles);
            SortListCommand = new DelegateCommand<PlaylistSorting>(OnSort);

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


        private void UpdateList(IEnumerable<FileMetaData> items, bool clear)
        {
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
