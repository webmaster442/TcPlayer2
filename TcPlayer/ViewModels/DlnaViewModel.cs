using System.Collections.Generic;
using System.Collections.ObjectModel;
using TcPlayer.Dlna;
using TcPlayer.Engine.Ui;

namespace TcPlayer.ViewModels
{
    public class DlnaViewModel : ViewModelBase
    {
        private ObservableCollection<DlnaItem> _items;
        private bool _isUiBlocked;

        public DelegateCommand ServersCommand { get; }
        public DelegateCommand<DlnaItem> ItemClickCommand { get; }

        public ObservableCollection<DlnaItem> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        public bool IsUiBlocked 
        { 
            get => _isUiBlocked;
            set => SetProperty(ref _isUiBlocked, value);
        }

        public DlnaViewModel()
        {
            Items = new ObservableCollection<DlnaItem>();
            ServersCommand = new DelegateCommand(OnListServers);
            ItemClickCommand = new DelegateCommand<DlnaItem>(OnItemClick);
        }

        private async void OnItemClick(DlnaItem obj)
        {
            IsUiBlocked = true;
            if (obj.IsServer)
            {
               await DlnaClient.GetContents(obj.Locaction);
            }
            IsUiBlocked = false;

        }

        private async void OnListServers(object obj)
        {
            IsUiBlocked = true;
            var servers = await DlnaClient.GetServers();
            Items = new ObservableCollection<DlnaItem>(servers);
            IsUiBlocked = false;
        }
    }
}
