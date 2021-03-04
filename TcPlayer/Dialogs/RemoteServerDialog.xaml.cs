using QRCoder;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TcPlayer.Engine;
using TcPlayer.Remote;

namespace TcPlayer.Dialogs
{
    /// <summary>
    /// Interaction logic for RemoteServerDialog.xaml
    /// </summary>
    public partial class RemoteServerDialog : Window
    {
        private RemoteServer _server;
        private readonly IMessenger _messenger;

        public RemoteServerDialog()
        {
            InitializeComponent();
        }

        public RemoteServerDialog(IMessenger messenger) : this()
        {
            InitializeComponent();
            _messenger = messenger;
        }

        private void OnWindowLoad(object sender, RoutedEventArgs e)
        {
            _server = new RemoteServer(_messenger);
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
    }
}
