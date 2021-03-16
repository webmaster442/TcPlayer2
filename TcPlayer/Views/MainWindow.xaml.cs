// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using ManagedBass;
using System;
using System.Threading;
using System.Windows;
using TcPlayer.Dialogs;
using TcPlayer.Engine;
using TcPlayer.Infrastructure;

namespace TcPlayer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window, IMainWindow, IDisposable
    {
        private readonly IMessenger _messenger;

        public MainWindow(IMessenger messenger)
        {
            InitializeComponent();
            _messenger = messenger;
        }

        #region IMainWindow
        public void HideUiBlocker()
        {
            Blocker.Hide();
        }

        public CancellationTokenSource ShowUiBlocker()
        {
            return Blocker.Show();
        }

        public void SetMainTab(MainTab tab)
        {
            Dispatcher.Invoke(() =>
            {
                MainTabs.SelectedIndex = (int)tab;
            });
        }
        #endregion

        private void RemoteButtonClick(object sender, RoutedEventArgs e)
        {
            var remote = new RemoteServerDialog(_messenger);
            remote.ShowDialog();
        }

        public void Dispose()
        {
            if (Blocker != null)
            {
                Blocker.Dispose();
                Blocker = null;
            }
            if (DataContext is IDisposable datacontext)
            {
                datacontext.Dispose();
                DataContext = null;
            }
        }

        private void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Bass.PluginFree(0);
            Dispose();
        }
    }
}
