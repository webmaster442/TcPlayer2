﻿using ManagedBass;
using ManagedBass.Fx;
using System;
using System.IO;
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

        public App(string mutexName) : base()
        {
            _mutex = new Mutex(true, mutexName, out _mutexCreated);
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
                base.OnStartup(e);
                var window = new MainWindow(_messenger);
                Current.MainWindow = window;
                Current.MainWindow.DataContext = new MainViewModel(_engine, _dialogProvider, window, _messenger, _settings);
                Current.MainWindow.Show();
            }
            else
            {
                _mutex = null;
                //TODO: Post message to existing app
                Current.Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Dispose();
            base.OnExit(e);
        }

        public void Dispose()
        {
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
