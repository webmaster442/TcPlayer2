// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using ManagedBass;
using ManagedBass.Fx;
using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
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
        private ShellIntegration _shellIntegration;

        private Mutex _mutex;
        private bool _mutexCreated;
        private Thread _ipcThread;
        private const string ipcAbort = "__abort__";

        public App()
        {
            _mutex = new Mutex(true, Entrypoint.MutexName, out _mutexCreated);
            _ipcThread = new Thread(IpcListener);
        }

        internal bool IsInstalled
        {
            get
            {
                if (_settings.Settings.IsExisting(SettingConst.AppSettings, SettingConst.Installed))
                {
                   return _settings.Settings.GetBool(SettingConst.AppSettings, SettingConst.Installed);
                }
                return false;
            }
            set
            {
                _settings.Settings.WriteBool(SettingConst.AppSettings, SettingConst.Installed, value);
                _settings.Save();
            }
        }

        internal void SetupDependencies()
        {
            _messenger = new Messenger();
            _settings = new SettingsFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TcPlayer.Options.json"));
            _engine = new AudioEngine(_messenger);
            _dialogProvider = new DialogProvider();

            Controls.CoverImageControl.Messenger = _messenger;
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

        public bool ShouldShowMainWindow { get; set; }


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
                _shellIntegration = new ShellIntegration(_messenger, window);

                string[] args = Environment.GetCommandLineArgs()
                    .Skip(1)
                    .Where(x => !Entrypoint.KnownArguments.Contains(x))
                    .ToArray();

                if (args.Length > 1)
                {
                    //1st argument allways is program location
                    _messenger.SendMessage(new AppArgumentsMessage
                    {
                        Arguments = args
                    });
                }

                if (ShouldShowMainWindow)
                {
                    MainWindow.Show();
                }
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

        private static bool WriteIpc(string payload, int timeout = 300)
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
            if (_shellIntegration != null)
            {
                _shellIntegration.Dispose();
                _shellIntegration = null;
            }
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
