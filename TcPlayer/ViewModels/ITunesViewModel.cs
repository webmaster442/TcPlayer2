using System;
using System.Collections.ObjectModel;
using TcPlayer.Engine;
using TcPlayer.Engine.Models;
using TcPlayer.Engine.Ui;
using System.Linq;
using System.Collections.Generic;

namespace TcPlayer.ViewModels
{
    public class ITunesViewModel : ViewModelBase
    {
        private readonly ITunesXmlDb _iTunes;
        private ITunesFilterKind _filter;
        private string _filterText;
        private string _selectedItem;

        public ObservableCollection<string> Items { get; private set; }

        public ITunesFilterKind Filter 
        { 
            get => _filter;
            set
            {
                SetProperty(ref _filter, value);
                DoFiltering();
            }
        }

        public string FilterText
        {
            get => _filterText;
            set
            {
                SetProperty(ref _filterText, value);
                DoFiltering();
            }
        }

        internal IEnumerable<ITunesTrack> GetItems()
        {
            throw new NotImplementedException();
        }

        public string SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public ITunesViewModel()
        {
            ITunesXmlDbOptions options = new ITunesXmlDbOptions
            {
                ExcludeNonExistingFiles = true,
                ParalelParsingEnabled = true
            };
            if (ITunesXmlDb.UserHasItunesDb)
            {
                _iTunes = new ITunesXmlDb(ITunesXmlDb.UserItunesDbPath, options);
                FilterText = string.Empty;
                Filter = ITunesFilterKind.Artist;
                DoFiltering();
            }

        }

        private void DoFiltering()
        {
            IEnumerable<string> items = Enumerable.Empty<string>();
            switch (Filter)
            {
                case ITunesFilterKind.Album:
                    items = _iTunes.Albums;
                    break;
                case ITunesFilterKind.Artist:
                    items = _iTunes.Artists;
                    break;
                case ITunesFilterKind.Genre:
                    items = _iTunes.Genres;
                    break;
                case ITunesFilterKind.Year:
                    items = _iTunes.Years;
                    break;
            }

            if (!string.IsNullOrEmpty(FilterText))
            {
                Items = new ObservableCollection<string>(items.Where(s => s.Contains(FilterText, StringComparison.OrdinalIgnoreCase)));
            }
            else
            {
                Items = new ObservableCollection<string>(items);
            }
        }
    }
}
