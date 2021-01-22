using System;
using System.Windows;
using TcPlayer.Engine;
using TcPlayer.Engine.Ui;
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

            var window = new MainWindow(messenger);

            Current.MainWindow = window;
            var engine = new AudioEngine(messenger);

            var model = new MainViewModel(engine, window);
            Current.MainWindow.DataContext = model;

            Current.MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (Current.MainWindow is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
