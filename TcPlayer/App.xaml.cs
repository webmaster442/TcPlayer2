using System.Windows;
using TcPlayer.Engine;
using TcPlayer.Engine.Ui;
using TcPlayer.Infrastructure;
using TcPlayer.ViewModels;

namespace TcPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            IMessenger messenger = new Messenger();

            Current.MainWindow = new MainWindow(messenger);
            var engine = new AudioEngine(messenger);

            var model = new MainViewModel(engine, new Dialogs());
            Current.MainWindow.DataContext = model;

            Current.MainWindow.Show();
        }
    }
}
