using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcPlayer.Engine.Ui;

namespace TcPlayer.ViewModels
{
    internal class PlaylistViewModel : ViewModelBase
    {
        private ObservableCollection<string> Files { get; }

        public PlaylistViewModel()
        {
            Files = new ObservableCollection<string>();
        }
    }
}
