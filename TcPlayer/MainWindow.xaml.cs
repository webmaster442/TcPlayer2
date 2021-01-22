using System;
using System.Windows;
using TcPlayer.Engine;
using TcPlayer.Engine.Messages;
using TcPlayer.Engine.Ui;

namespace TcPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMessageClient<PositionInfo>
    {
        private readonly IMessenger _messenger;

        public MainWindow(IMessenger messenger)
        {
            InitializeComponent();
            _messenger = messenger;
            _messenger.SubScribe(this);
        }

        public Guid MessageReciverID => Guid.NewGuid();

        public void HandleMessage(PositionInfo message)
        {
            switch (message.State)
            {
                case EngineState.ReadyToPlay:
                case EngineState.NoFile:
                    TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
                    break;
                case EngineState.Playing:
                    TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
                    break;
                case EngineState.Paused:
                    TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Paused;
                    break;
            }
            TaskbarItemInfo.ProgressValue = message.Percent;
        }
    }
}
