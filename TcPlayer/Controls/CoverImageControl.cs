using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TcPlayer.Engine.Models;
using TcPlayer.Infrastructure;

namespace TcPlayer.Controls
{
    internal class CoverImageControl : Image
    {
        public CoverImageControl()
        {
            StretchDirection = StretchDirection.DownOnly;
            Stretch = Stretch.Uniform;
        }

        public Metadata SongMetaData
        {
            get { return (Metadata)GetValue(SongMetaDataProperty); }
            set { SetValue(SongMetaDataProperty, value); }
        }

        public static readonly DependencyProperty SongMetaDataProperty =
            DependencyProperty.Register("SongMetaData", typeof(Metadata), typeof(CoverImageControl), new PropertyMetadata(null, MetaChange));

        private static void MetaChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CoverImageControl cic)
            {
                 cic.Refresh();
            }
        }

#pragma warning disable S3168 // "async" methods should not return "void"
        private async void Refresh()
#pragma warning restore S3168 // "async" methods should not return "void"
        {
            byte[] downloaded = null;
            if (SongMetaData != null)
            {
                if (SongMetaData.Cover.Length > 0)
                {
                    LoadFromArray(SongMetaData.Cover);
                    return;
                }
                else if (!string.IsNullOrEmpty(SongMetaData.CoverUrl)
                    && SongMetaData.MediaKind == MediaKind.Stream)
                {
                    downloaded = await DownloadCover(SongMetaData.CoverUrl);
                }
                else if (!string.IsNullOrEmpty(SongMetaData.Title)
                    && SongMetaData.MediaKind == MediaKind.Stream)
                {
                    downloaded = await GetCover(SongMetaData.Title);
                }
                else if (!string.IsNullOrEmpty(SongMetaData.Artist)
                        && !string.IsNullOrEmpty(SongMetaData.Title))
                {
                    downloaded = await GetCover($"{SongMetaData.Artist} - {SongMetaData.Title}");
                }

                if (downloaded != null && downloaded.Length > 0)
                {
                    LoadFromArray(downloaded);

                }
                else
                {
                    LoadDefault();
                }

            }
            else
            {
                LoadDefault();
            }
        }

        private void LoadDefault()
        {
            if (SongMetaData == null) return;

            string resource = "IconMusic";
            switch (SongMetaData.MediaKind)
            {
                case MediaKind.Cd:
                    resource = "IconCd";
                    break;
                case MediaKind.Stream:
                    resource = "IconNetwork";
                    break;

            }

            if (FindResource(resource) is Viewbox icon)
            {
                icon.Width = 300;
                icon.Height = 300;
                Source = BitmapHelper.ToBitmapSource(icon);
            }
        }

        private void LoadFromArray(byte[] array)
        {
            using (var stream = new MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.DecodePixelWidth = 300;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                if (image.CanFreeze)
                    image.Freeze();
                Source = image;
            }
        }

        private static WebClient CreateClientWithProxy()
        {
            var client = new WebClient();
            IWebProxy defaultProxy = WebRequest.DefaultWebProxy;
            if (defaultProxy != null)
            {
                defaultProxy.Credentials = CredentialCache.DefaultCredentials;
                client.Proxy = defaultProxy;
            }
            return client;
        }

        private async Task<byte[]> DownloadCover(string coverUrl)
        {
            try
            {
                using (WebClient client = CreateClientWithProxy())
                {
                    return await client.DownloadDataTaskAsync(coverUrl);
                }
            }
            catch (Exception)
            {
                return Array.Empty<byte>();
            }
        }

        private async Task<byte[]> GetCover(string query)
        {
            try
            {
                using (WebClient client = CreateClientWithProxy())
                {
                    string encoded = HttpUtility.UrlEncode(query);
                    string fulladdress = $"https://itunes.apple.com/search?term={encoded}&media=music&limit=1";
                    string response = client.DownloadString(fulladdress);
                    var responseObject = JsonSerializer.Deserialize<RootObject>(response);
                    if (responseObject != null && responseObject.ResultCount > 0)
                    {
                        string artwork = responseObject.Results?[0].ArtworkUrl100;
                        artwork = artwork?.Replace("100x100", "600x600");

                        return await client.DownloadDataTaskAsync(artwork);
                    }
                    return Array.Empty<byte>();
                }
            }
            catch (Exception)
            {
                return Array.Empty<byte>();
            }
        }
    }
}
