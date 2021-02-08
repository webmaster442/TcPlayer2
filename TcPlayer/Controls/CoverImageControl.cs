﻿using System;
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

        private async void Refresh()
        {
            byte[] downloaded = null;
            if (SongMetaData != null)
            {
                if (SongMetaData.Cover.Length > 0)
                {
                    LoadFromArray(SongMetaData.Cover);
                    return;
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

        private async Task<byte[]> GetCover(string query)
        {
            try
            {
                using (var client = new WebClient())
                {
                    IWebProxy defaultProxy = WebRequest.DefaultWebProxy;
                    if (defaultProxy != null)
                    {
                        defaultProxy.Credentials = CredentialCache.DefaultCredentials;
                        client.Proxy = defaultProxy;
                    }

                    string encoded = HttpUtility.UrlEncode(query);
                    string fulladdress = $"https://itunes.apple.com/search?term={encoded}&media=music&limit=1";
                    string response = client.DownloadString(fulladdress);
                    var responseObject = JsonSerializer.Deserialize<RootObject>(response);
                    if (responseObject != null && responseObject.resultCount > 0)
                    {

                        string artwork = responseObject.results?[0].artworkUrl100;
                        artwork = artwork?.Replace("100x100", "600x600");

                        return await client.DownloadDataTaskAsync(artwork);
                    }
                    else
                    {
                        return Array.Empty<byte>();
                    }
                }
            }
            catch (Exception)
            {
                return Array.Empty<byte>();
            }
        }
    }
}
