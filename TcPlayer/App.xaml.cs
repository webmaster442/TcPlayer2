using ManagedBass;
using ManagedBass.Fx;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Windows;
using TcPlayer.Engine;
using TcPlayer.Engine.Messages;
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
        private Thread _ipcThread;
        private const string ipcAbort = "__abort__";

        public App()
        {
            _mutex = new Mutex(true, Entrypoint.MutexName, out _mutexCreated);
            _ipcThread = new Thread(IpcListener);
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

            //required to initialize BassFx
            //Unused local variables should be removed
#pragma warning disable S1481
            var version = BassFx.Version;
#pragma warning restore S1481
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            if (_mutexCreated)
            {
                //1st instance
                _ipcThread.Start();
                base.OnStartup(e);
                var window = new MainWindow(_messenger);
                Current.MainWindow = window;
                Current.MainWindow.DataContext = new MainViewModel(_engine, _dialogProvider, window, _messenger, _settings);
                Current.MainWindow.Show();

                _messenger.SendMessage(new AppArgumentsMessage
                {
                    Arguments = Environment.GetCommandLineArgs()
                });

            }
            else
            {
                _mutex = null;
                if (!WriteStartupParametersOnIpc())
                {
                    MessageBox.Show("Running app activation failed");
                }
                Current.Shutdown();
            }
        }

        private bool WriteIpc(string payload, int timeout = 300)
        {
            using (var client = new NamedPipeClientStream(Entrypoint.MutexName))
            {
                try { client.Connect(timeout); }
                catch { return false; }

                if (!client.IsConnected) return false;

                using (StreamWriter writer = new StreamWriter(client))
                {
                    writer.Write(payload);
                    writer.Flush();
                }
            }
            return true;
        }

        private bool WriteStartupParametersOnIpc(int timeout = 300)
        {
            string payload = System.Text.Json.JsonSerializer.Serialize(Environment.GetCommandLineArgs());
            return WriteIpc(payload, timeout);
        }

        private void IpcListener()
        {
            using (var server = new NamedPipeServerStream(Entrypoint.MutexName))
            {
                while (true)
                {
                    //ipc connection needed to abort
                    server.WaitForConnection();
                    using (StreamReader reader = new StreamReader(server))
                    {
                        string payload = reader.ReadToEnd();
                        if (payload == ipcAbort)
                        {
                            //app is shuting down, break the cycle
                            break;
                        }
                        else
                        {
                            _messenger.SendMessage(new AppArgumentsMessage
                            {
                                Arguments = System.Text.Json.JsonSerializer.Deserialize<string[]>(payload)
                            });
                        }
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
            if (_ipcThread != null)
            {
                WriteIpc(ipcAbort);
                _ipcThread.Join();
                _ipcThread = null;
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
