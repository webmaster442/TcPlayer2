using ManagedBass;
using ManagedBass.Fx;
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
    public sealed partial class App : Application, IDisposable
    {
        private IEngine _engine;

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!BassLibs.BassLibs.VerifyDllFiles())
            {
                MessageBox.Show("Engine dll files corrupted. Please reinstall.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(-1);
            }

            foreach (var pluginFile in BassLibs.BassLibs.Plugins)
            {
                Bass.PluginLoad(pluginFile);
            }

            var version = BassFx.Version;

            base.OnStartup(e);
            IMessenger messenger = new Messenger();

            var window = new MainWindow(messenger);

            Current.MainWindow = window;
             _engine = new AudioEngine(messenger);

            var model = new MainViewModel(_engine, window, messenger);
            Current.MainWindow.DataContext = model;

            Current.MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_engine != null)
            {
                _engine.Dispose();
                _engine = null;
            }
        }
    }
}
