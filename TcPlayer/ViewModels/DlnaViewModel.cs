// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TcPlayer.Network;
using TcPlayer.Engine.Ui;

namespace TcPlayer.ViewModels
{
    public class DlnaViewModel : ViewModelBase
    {
        private ObservableCollection<DlnaItem> _items;
        private bool _isUiBlocked;
        private bool _canImport;

        private Stack<string> Navigation { get; }

        public DelegateCommand ServersCommand { get; }
        public DelegateCommand<DlnaItem> ItemClickCommand { get; }
        public DelegateCommand PreviousCommand { get; }

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

        public bool CanImport
        {
            get => _canImport;
            set => SetProperty(ref _canImport, value);
        }

        public string CurrentServer { get; private set; }

        public DlnaViewModel()
        {
            Items = new ObservableCollection<DlnaItem>();
            ServersCommand = new DelegateCommand(OnListServers);
            ItemClickCommand = new DelegateCommand<DlnaItem>(OnItemClick);
            Navigation = new Stack<string>();
            PreviousCommand = new DelegateCommand(OnPrevious, CanNavigate);
            CanImport = false;
        }

        private bool CanNavigate(object obj)
        {
            return Navigation.Count > 1;
        }

        private async void OnPrevious(object obj)
        {
            IsUiBlocked = true;
            Navigation.Pop(); //current
            var previousId = Navigation.Pop();
            await Update(CurrentServer, previousId);
            IsUiBlocked = false;
        }

        private async void OnItemClick(DlnaItem obj)
        {
            IsUiBlocked = true;
            if (obj.IsServer)
            {
                CurrentServer = obj.Locaction;
                await Update(obj.Locaction);
            }
            else if (obj.IsBrowsable)
            {
                await Update(CurrentServer, obj.Id);
            }
            IsUiBlocked = false;
            PreviousCommand.RaiseCanExecuteChanged();
        }

        private async Task Update(string url, string id = "0")
        {
            var items = await DlnaClient.GetContents(url, id);
            Items = new ObservableCollection<DlnaItem>(items);

            CanImport = Items.Any(i => !i.IsBrowsable);

            Navigation.Push(id);
        }

        private async void OnListServers(object obj)
        {
            IsUiBlocked = true;
            var servers = await DlnaClient.GetServers();
            Items = new ObservableCollection<DlnaItem>(servers);
            IsUiBlocked = false;
        }

        public IEnumerable<string> GetUrls()
        {
            return Items.Where(i => !i.IsBrowsable)
                        .Select(i => i.Locaction);
        }
    }
}
