using System;
using TcPlayer.Engine;
using TcPlayer.Engine.Messages;
using TcPlayer.Engine.Models;
using TcPlayer.Engine.Ui;
using TcPlayer.Infrastructure;

namespace TcPlayer
{
    internal class ShellIntegration : IMessageClient<PositionInfoMessage>
    {
        private readonly IMainWindow _mainWindow;

        public ShellIntegration(IMessenger messenger, IMainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            messenger.SubScribe(this);
        }

        public Guid MessageReciverID => Guid.NewGuid();

        public void HandleMessage(PositionInfoMessage message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                switch (message.State)
                {
                    case EngineState.ReadyToPlay:
                    case EngineState.NoFile:
                        _mainWindow.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
                        _mainWindow.TaskbarItemInfo.ProgressValue = 0;
                        break;
                    case EngineState.Playing:

                        if (message.IsIndeterminate)
                            _mainWindow.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Indeterminate;
                        else
                            _mainWindow.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
                        break;
                    case EngineState.Paused:
                        _mainWindow.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Paused;
                        break;
                }

                if (!message.IsIndeterminate)
                    _mainWindow.TaskbarItemInfo.ProgressValue = message.Percent;

            });
        }
    }
}
