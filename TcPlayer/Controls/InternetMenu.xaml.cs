using System.Diagnostics;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using TcPlayer.Engine.Models;

namespace TcPlayer.Controls
{
    /// <summary>
    /// Interaction logic for InternetMenu.xaml
    /// </summary>
    public partial class InternetMenu : Button
    {
        public InternetMenu()
        {
            InitializeComponent();
        }

        public Metadata SongMetadata
        {
            get { return (Metadata)GetValue(SongMetadataProperty); }
            set { SetValue(SongMetadataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SongMetadata.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SongMetadataProperty =
            DependencyProperty.Register("SongMetadata", typeof(Metadata), typeof(InternetMenu), new PropertyMetadata(null));

#pragma warning disable S1075 // URIs should not be hardcoded
        private const string GoogleUrlFormat = "https://www.google.com/search?&q={0}";
        private const string YoutubeUrlFormat = "https://www.youtube.com/results?search_query={0}";
        private const string SpotifyUrlFormat = "https://open.spotify.com/search/{0}";
        private const string DiscogsUrlFormat = "https://www.discogs.com/search/?q={0}&type=all";
#pragma warning restore S1075 // URIs should not be hardcoded

        private static void SearchTheWeb(string provider, string term)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = string.Format(provider, HttpUtility.UrlEncode(term));
            p.Start();
        }

        private void SearchTheWebs(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem 
                && menuItem.Tag is string site)
            {
                switch (site)
                {
                    case "Google":
                        SearchTheWeb(GoogleUrlFormat, $"{SongMetadata.Artist} - {SongMetadata.Title}");
                        break;
                    case "Youtube":
                        SearchTheWeb(YoutubeUrlFormat, $"{SongMetadata.Artist} - {SongMetadata.Title}");
                        break;
                    case "Spotify":
                        SearchTheWeb(SpotifyUrlFormat, $"{SongMetadata.Artist} - {SongMetadata.Title}");
                        break;
                    case "Discogs":
                        SearchTheWeb(DiscogsUrlFormat, $"{SongMetadata.AlbumArtist} - {SongMetadata.Album}");
                        break;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu.IsOpen = !ContextMenu.IsOpen;
        }
    }
}
