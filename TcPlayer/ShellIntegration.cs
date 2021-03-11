using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using TcPlayer.Engine;
using TcPlayer.Engine.Messages;
using TcPlayer.Engine.Models;
using TcPlayer.Engine.Ui;
using TcPlayer.Infrastructure;
using TcPlayer.Network.Remote;
using Windows.Media;
using UwpMedia = Windows.Media;
using UwpStreams = Windows.Storage.Streams;
using WinShell = System.Windows.Shell;

namespace TcPlayer
{
    internal sealed class ShellIntegration : IMessageClient<ShellNotificationMessage>, IMessageClient<CoverImageChangeMessage>, IDisposable
    {
        private readonly IMainWindow _mainWindow;
        UwpMedia.SystemMediaTransportControls _systemControl;
        private readonly IMessenger _messenger;

        public ShellIntegration(IMessenger messenger, IMainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _systemControl = UwpMedia.Playback.BackgroundMediaPlayer.Current.SystemMediaTransportControls;
            _systemControl.IsEnabled = true;
            _systemControl.IsPlayEnabled = true;
            _systemControl.IsPauseEnabled = true;
            _systemControl.IsStopEnabled = true;
            _systemControl.IsNextEnabled = true;
            _systemControl.IsPreviousEnabled = true;
            _systemControl.ButtonPressed += OnSystemControlButtonPress;
            _messenger = messenger;
            _messenger.SubScribe(this);
        }

        public void Dispose()
        {
            _systemControl.DisplayUpdater.ClearAll();
            _systemControl.IsEnabled = false;
            _systemControl = null;
        }

        private void OnSystemControlButtonPress(UwpMedia.SystemMediaTransportControls sender, UwpMedia.SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            _messenger.SendMessage(new RemoteControlMessage
            {
                Command = GetRemoteControlCommand(args.Button),
            });
        }

        public Guid MessageReciverID => Guid.NewGuid();

        public void HandleMessage(ShellNotificationMessage message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                SetTaskbarProgress(message);
                UpdateSystemMediaControls(message);
            });
        }

        public void HandleMessage(CoverImageChangeMessage message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                _systemControl.DisplayUpdater.AppMediaId = "TcPlayer";
                using (var stream = new MemoryStream(message.CoverData))
                {
                    _systemControl.DisplayUpdater.Thumbnail = UwpStreams.RandomAccessStreamReference.CreateFromStream(stream.AsRandomAccessStream());
                    _systemControl.DisplayUpdater.Update();
                }

            });
        }

        private void UpdateSystemMediaControls(ShellNotificationMessage message)
        {
            _systemControl.DisplayUpdater.ClearAll();
            _systemControl.DisplayUpdater.AppMediaId = "TcPlayer";
            _systemControl.DisplayUpdater.Type = UwpMedia.MediaPlaybackType.Music;
            _systemControl.DisplayUpdater.MusicProperties.Title = message.Metadata.Title;
            _systemControl.DisplayUpdater.MusicProperties.Artist = message.Metadata.Artist;
            _systemControl.PlaybackStatus = GetPlaybackStatus(message.State);
            _systemControl.UpdateTimelineProperties(new UwpMedia.SystemMediaTransportControlsTimelineProperties
            {
                StartTime = TimeSpan.FromSeconds(0),
                Position = TimeSpan.FromSeconds(message.Position),
                EndTime = TimeSpan.FromSeconds(message.Length),
            });
            _systemControl.DisplayUpdater.Update();
        }

        private void SetTaskbarProgress(ShellNotificationMessage message)
        {
            switch (message.State)
            {
                case EngineState.ReadyToPlay:
                case EngineState.NoFile:
                    _mainWindow.TaskbarItemInfo.ProgressState = WinShell.TaskbarItemProgressState.None;
                    _mainWindow.TaskbarItemInfo.ProgressValue = 0;
                    break;
                case EngineState.Playing:

                    if (message.IsIndeterminate)
                        _mainWindow.TaskbarItemInfo.ProgressState = WinShell.TaskbarItemProgressState.Indeterminate;
                    else
                        _mainWindow.TaskbarItemInfo.ProgressState = WinShell.TaskbarItemProgressState.Normal;
                    break;
                case EngineState.Paused:
                    _mainWindow.TaskbarItemInfo.ProgressState = WinShell.TaskbarItemProgressState.Paused;
                    break;
            }

            if (!message.IsIndeterminate)
                _mainWindow.TaskbarItemInfo.ProgressValue = message.Percent;
        }

        private RemoteControlCommand GetRemoteControlCommand(SystemMediaTransportControlsButton button)
        {
            return button switch
            {
                SystemMediaTransportControlsButton.Play => RemoteControlCommand.Play,
                SystemMediaTransportControlsButton.Pause => RemoteControlCommand.Pause,
                SystemMediaTransportControlsButton.Previous => RemoteControlCommand.Previous,
                SystemMediaTransportControlsButton.Stop => RemoteControlCommand.Stop,
                SystemMediaTransportControlsButton.Next => RemoteControlCommand.Next,
                _ => throw new InvalidOperationException("Invalid button"),
            };
        }

        private MediaPlaybackStatus GetPlaybackStatus(EngineState state)
        {
            return state switch
            {
                EngineState.NoFile => MediaPlaybackStatus.Closed,
                EngineState.Paused or EngineState.Seeking => MediaPlaybackStatus.Paused,
                EngineState.Playing => MediaPlaybackStatus.Playing,
                EngineState.ReadyToPlay => MediaPlaybackStatus.Stopped,
                _ => throw new InvalidOperationException("Unknown state"),
            };
        }
    }
}
