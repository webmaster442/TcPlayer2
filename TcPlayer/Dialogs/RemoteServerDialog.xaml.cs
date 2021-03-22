// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using QRCoder;
using TcPlayer.Engine;
using TcPlayer.Network.Http;
using TcPlayer.Network.Remote;

namespace TcPlayer.Dialogs
{
    /// <summary>
    /// Interaction logic for RemoteServerDialog.xaml
    /// </summary>
    public partial class RemoteServerDialog : ILog
    {
        private RemoteServer _server;
        private readonly IMessenger _messenger;
        private ObservableCollection<string> _log;

        public RemoteServerDialog()
        {
            InitializeComponent();
            _log = new ObservableCollection<string>();
            LogView.ItemsSource = _log;
        }

        public RemoteServerDialog(IMessenger messenger) : this()
        {
            InitializeComponent();
            _messenger = messenger;
        }

        private void OnWindowLoad(object sender, RoutedEventArgs e)
        {
            _server = new RemoteServer(_messenger, this);
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(_server.ServerLink, QRCodeGenerator.ECCLevel.H);
            QRCode qRCode = new QRCode(qrCodeData);
            var bitmap = qRCode.GetGraphic(20);
            QrCodeImage.Source = GetBitmapSource(bitmap);
            QrCodeUrl.Text = _server.ServerLink;
            _server.Start();
        }

        private ImageSource GetBitmapSource(System.Drawing.Bitmap bitmap)
        {
            return System.Windows.Interop
                .Imaging
                .CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(),
                                               IntPtr.Zero,
                                               Int32Rect.Empty,
                                               BitmapSizeOptions.FromWidthAndHeight(bitmap.Width, bitmap.Height));
        }

        private void OnWindowClose(object sender, CancelEventArgs e)
        {
            _server.Dispose();
        }

        public void Log(string format, params object[] parameters)
        {
            var str = string.Format(format, parameters);
            _log.Add(str);

            if (_log.Count > 100)
            {
                _log.Clear();
            }

        }
    }
}
