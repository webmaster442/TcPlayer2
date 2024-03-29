﻿// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using TcPlayer.Engine;
using TcPlayer.Engine.Models;
using TcPlayer.Engine.Ui;
using System.Linq;
using System.Collections.Generic;

namespace TcPlayer.ViewModels
{
    public class ItunesViewModel : ViewModelBase
    {
        private readonly ItunesXmlDb _iTunes;
        private ItunesTab _filter;
        private string _filterText;
        private string _selectedItem;
        private ObservableCollection<string> _items;

        public ObservableCollection<string> Items 
        { 
            get => _items;
            private set => SetProperty(ref _items, value);
        }

        public ItunesTab Filter
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
            if (string.IsNullOrEmpty(SelectedItem))
                return Enumerable.Empty<ITunesTrack>();

            if (_filter == ItunesTab.Playlists)
                return _iTunes.ReadPlaylist(SelectedItem);

            ITunesFilterKind filter = GetFilter(_filter);

            return _iTunes.Filter(filter, SelectedItem);
        }

        private ITunesFilterKind GetFilter(ItunesTab filter)
        {
            switch (filter)
            {
                case ItunesTab.Albums:
                    return ITunesFilterKind.Album;
                case ItunesTab.Artists:
                    return ITunesFilterKind.Artist;
                case ItunesTab.Genres:
                    return ITunesFilterKind.Genre;
                case ItunesTab.Years:
                    return ITunesFilterKind.Year;
                default:
                    throw new ArgumentException(nameof(filter));
            }
        }

        public string SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public ItunesViewModel()
        {
            ItunesXmlDbOptions options = new ItunesXmlDbOptions
            {
                ExcludeNonExistingFiles = true,
                ParalelParsingEnabled = true
            };
            if (ItunesXmlDb.UserHasItunesDb)
            {
                _iTunes = new ItunesXmlDb(ItunesXmlDb.UserItunesDbPath, options);
                FilterText = string.Empty;
                Filter = ItunesTab.Artists;
                DoFiltering();
            }

        }

        private void DoFiltering()
        {
            IEnumerable<string> items = Enumerable.Empty<string>();
            switch (Filter)
            {
                case ItunesTab.Albums:
                    items = _iTunes.Albums;
                    break;
                case ItunesTab.Artists:
                    items = _iTunes.Artists;
                    break;
                case ItunesTab.Genres:
                    items = _iTunes.Genres;
                    break;
                case ItunesTab.Years:
                    items = _iTunes.Years;
                    break;
                case ItunesTab.Playlists:
                    items = _iTunes.Playlists;
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
