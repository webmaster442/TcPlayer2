using ManagedBass;
using ManagedBass.Fx;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Windows;
using TcPlayer.Engine;
using TcPlayer.Engine.Settings;
using TcPlayer.Engine.Ui;
using TcPlayer.Infrastructure;
using TcPlayer.ViewModels;
using TcPlayer.Views;

namespace TcPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App : Application, IDisposable
    {
        private IMessenger _messenger;
        private IEngine _engine;
        private ISettingsFile _settings;
        private IDialogProvider _dialogProvider;
        private Mutex _mutex;
        private bool _mutexCreated;
        private CancellationTokenSource _cancellationTokenSource;

        public App()
        {
            _mutex = new Mutex(true, Entrypoint.MutexName, out _mutexCreated);
            _cancellationTokenSource = new CancellationTokenSource();
        }

        internal void SetupDependencies()
        {
            _messenger = new Messenger();
            _settings = new SettingsFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TcPlayer.Options.json"));
            _engine = new AudioEngine(_messenger);
            _dialogProvider = new DialogProvider();
        }

        internal void SetupEngineDependencies()
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
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            if (_mutexCreated)
            {
                //1st instance
                StartIpcListening(_cancellationTokenSource.Token);
                base.OnStartup(e);
                var window = new MainWindow(_messenger);
                Current.MainWindow = window;
                Current.MainWindow.DataContext = new MainViewModel(_engine, _dialogProvider, window, _messenger, _settings);
                Current.MainWindow.Show();
            }
            else
            {
                _mutex = null;
                WriteStartupParametersOnIpc();
                Current.Shutdown();
            }
        }

        private bool WriteStartupParametersOnIpc(int timeout = 300)
        {
            using (var client = new NamedPipeClientStream(Entrypoint.MutexName))
            {
                try { client.Connect(timeout); }
                catch { return false; }

                if (!client.IsConnected) return false;

                string payload = System.Text.Json.JsonSerializer.Serialize(Environment.GetCommandLineArgs());

                using (StreamWriter writer = new StreamWriter(client))
                {
                    writer.Write(payload);
                    writer.Flush();
                }
            }
            return true;
        }

        private async void StartIpcListening(CancellationToken cancelToken)
        {
            using (var server = new NamedPipeServerStream(Entrypoint.MutexName))
            {
                while (true)
                {
                    await server.WaitForConnectionAsync(cancelToken);
                    using (StreamReader reader = new StreamReader(server))
                    {
                        string json = reader.ReadToEnd();
                        string[] arguments = System.Text.Json.JsonSerializer.Deserialize<string[]>(json);
                        //todo: post messages
                    }

                    if (cancelToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Dispose();
            base.OnExit(e);
        }

        public void Dispose()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
            if (_engine != null)
            {
                _engine.Dispose();
                _engine = null;
            }
            if (_mutex != null)
            {
                _mutex.ReleaseMutex();
                _mutex.Close();
                _mutex = null;
            }
        }
    }
}
