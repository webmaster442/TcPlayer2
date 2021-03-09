using System.Diagnostics;
using System.Windows;

namespace TcPlayer.Dialogs
{
    public partial class NoYoutubeDlFoundDialog : Window
    {
        public NoYoutubeDlFoundDialog(Infrastructure.YoutubeDlState state)
        {
            InitializeComponent();
            switch (state)
            {
                case Infrastructure.YoutubeDlState.NotInstalled:
                    DialogText.Text = "To playback youtube videos youtube-dl is required. Download it and copy it to the TCPlayer install directory";
                    break;
                case Infrastructure.YoutubeDlState.Outdated:
                    DialogText.Text = "Your youtube-dl is probably outdated. Download a newer version and copy it to the TCPlayer install directory";
                    break;
            }
        }

        private static void OpenLink(string link)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = link;
                process.StartInfo.UseShellExecute = true;
                process.Start();
            }
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OnWebsite(object sender, RoutedEventArgs e)
        {
            OpenLink("https://youtube-dl.org/");
            DialogResult = false;
        }

        private void OnDownload(object sender, RoutedEventArgs e)
        {
            OpenLink("https://youtube-dl.org/downloads/latest/youtube-dl.exe");
            DialogResult = false;
        }
    }
}
