using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TcPlayer.Controls
{
    internal class AudioCdMenuItem : MenuItem
    {
        public ICommand LoadDiscCommand
        {
            get { return (ICommand)GetValue(LoadDiscCommandProperty); }
            set { SetValue(LoadDiscCommandProperty, value); }
        }

        public static readonly DependencyProperty LoadDiscCommandProperty =
            DependencyProperty.Register("LoadDiscCommand", typeof(ICommand), typeof(AudioCdMenuItem), new PropertyMetadata(null));

        public object LoadDiscCommandParameter
        {
            get { return (object)GetValue(LoadDiscCommandParameterProperty); }
            set { SetValue(LoadDiscCommandParameterProperty, value); }
        }

        public static readonly DependencyProperty LoadDiscCommandParameterProperty =
            DependencyProperty.Register("LoadDiscCommandParameter", typeof(object), typeof(AudioCdMenuItem), new PropertyMetadata(null));

        public AudioCdMenuItem()
        {
            Header = "Audio Cd";
            Items.Add(new MenuItem
            {
                Header = "No disc found"
            });
            SubmenuOpened += OnSubmenuOpen;
        }

        private void OnSubmenuOpen(object sender, System.Windows.RoutedEventArgs e)
        {
            Items.Clear();
            var drives = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.CDRom && d.IsReady).ToArray();

            if (drives.Length < 1)
            {
                Items.Add(new MenuItem
                {
                    Header = "No disc found"
                });
            }

            int index = 0;
            foreach (var drive in drives)
            {
                var item = new MenuItem
                {
                    Header = drive,
                    Command = LoadDiscCommand,
                    CommandParameter = index
                };
                Items.Add(item);
                ++index;
            }
        }
    }
}
