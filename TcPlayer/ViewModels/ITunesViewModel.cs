using System.Collections.ObjectModel;
using TcPlayer.Engine;
using TcPlayer.Engine.Models;
using TcPlayer.Engine.Ui;

namespace TcPlayer.ViewModels
{
    public class ITunesViewModel: ViewModelBase
    {
        private readonly ITunesXmlDb? _iTunes;

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
                Albums = new ObservableCollection<string>(_iTunes.Albums);
                Artists = new ObservableCollection<string>(_iTunes.Artists);
                Years = new ObservableCollection<string>(_iTunes.Years);
                Genres = new ObservableCollection<string>(_iTunes.Genres);
                Playlists = new ObservableCollection<string>(_iTunes.Playlists);
            }
            else
            {
                Albums = new ObservableCollection<string>();
                Artists = new ObservableCollection<string>();
                Genres = new ObservableCollection<string>();
                Years = new ObservableCollection<string>();
                Playlists = new ObservableCollection<string>();
            }

        }

        public ObservableCollection<string> Albums { get; }
        public ObservableCollection<string> Artists { get;  }
        public ObservableCollection<string> Genres { get; }
        public ObservableCollection<string> Years { get; }
        public ObservableCollection<string> Playlists { get; }
    }
}
