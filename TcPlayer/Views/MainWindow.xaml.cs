using ManagedBass;
using System;
using System.Threading;
using System.Windows;
using TcPlayer.Dialogs;
using TcPlayer.Engine;
using TcPlayer.Engine.Messages;
using TcPlayer.Engine.Models;
using TcPlayer.Engine.Ui;
using TcPlayer.Infrastructure;

namespace TcPlayer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window, IMessageClient<PositionInfoMessage>, IMainWindow, IDisposable
    {
        private readonly IMessenger _messenger;

        public MainWindow(IMessenger messenger)
        {
            InitializeComponent();
            _messenger = messenger;
            _messenger.SubScribe(this);
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

        public Guid MessageReciverID => Guid.NewGuid();

        public void HandleMessage(PositionInfoMessage message)
        {
            Dispatcher.Invoke(() =>
            {
                switch (message.State)
                {
                    case EngineState.ReadyToPlay:
                    case EngineState.NoFile:
                        TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
                        TaskbarItemInfo.ProgressValue = 0;
                        break;
                    case EngineState.Playing:

                        if (message.IsIndeterminate)
                            TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Indeterminate;
                        else
                            TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
                        break;
                    case EngineState.Paused:
                        TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Paused;
                        break;
                }

                if (!message.IsIndeterminate)
                    TaskbarItemInfo.ProgressValue = message.Percent;

            });
        }

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
