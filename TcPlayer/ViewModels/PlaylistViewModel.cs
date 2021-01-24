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

        public BindingList<FileMetaData> Items { get; }
        public DelegateCommand AddFilesCommand { get; }

        public PlaylistViewModel(IDialogProvider dialogProvider)
        {
            _dialogProvider = dialogProvider;
            Items = new BindingList<FileMetaData>();
            AddFilesCommand = new DelegateCommand(OnAddFiles);

        }

        private async void OnAddFiles(object obj)
        {
            if (_dialogProvider.TrySelectFilesDialog(Formats.AudioFormatFilterString, out string[] files))
            {
                var source = _dialogProvider.ShowUiBlocker();
                var items = await FileInfoFactory.CreateFileInfos(files, source.Token);
                UpdateList(items);
                _dialogProvider.HideUiBlocker();
            }
        }

        private void UpdateList(IEnumerable<FileMetaData> items)
        {
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
