using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcPlayer.Engine.Models;
using TcPlayer.Engine.Ui;

namespace TcPlayer.ViewModels
{
    internal class PlaylistViewModel : ViewModelBase
    {
        public ObservableCollection<FileMetaData> Items { get; }

        public DelegateCommand AddFilesCommand { get; }
        public DelegateCommand ClearListCommand { get; }
        public DelegateCommand DeleteCommand { get; }


        public PlaylistViewModel()
        {
            Items = new ObservableCollection<FileMetaData>();
        }
        
    }
}
