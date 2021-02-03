﻿using System;
using System.Linq;
using TcPlayer.Engine;
using TcPlayer.Engine.Messages;
using TcPlayer.Engine.Ui;
using TcPlayer.Infrastructure;
using TcPlayer.Properties;

namespace TcPlayer.ViewModels
{
    internal sealed class MainViewModel : ViewModelBase, IMessageClient<LoadFileMessage>
    {
        private readonly IDialogProvider _dialogProvider;
        private SoundDeviceInfo _selectedAudioDevice;

        public IEngine Engine { get; }

        public PlaylistViewModel Playlist { get; }

        public DelegateCommand PlayCommand { get; }
        public DelegateCommand PauseCommand { get; }
        public DelegateCommand StopCommand { get; }
        public DelegateCommand LoadCommand { get; }
        public DelegateCommand<double> SeekSliderPositionCommand { get; }
        public DelegateCommand<double> SetVolumeCommand { get; }
        public DelegateCommand PreviousCommand { get; }
        public DelegateCommand NextCommand { get; }

        public SoundDeviceInfo SelectedAudioDevice
        {
            get => _selectedAudioDevice;
            set
            {
                if (SetProperty(ref _selectedAudioDevice, value))
                {
                    Engine.Initialize(_selectedAudioDevice);
                    Settings.Default.AudioOutIndex = _selectedAudioDevice.Index;
                    Settings.Default.Save();
                }
            }
        }

        public Guid MessageReciverID => Guid.NewGuid();

        public MainViewModel(IEngine engine, IDialogProvider dialogProvider, IMessenger messenger)
        {
            Engine = engine;
            _dialogProvider = dialogProvider;
            messenger.SubScribe(this);

            PlayCommand = new DelegateCommand((o) => Engine.Play());
            StopCommand = new DelegateCommand((o) => Engine.Stop());
            PauseCommand = new DelegateCommand((o) => Engine.Pause());
            LoadCommand = new DelegateCommand(Onload);
            SeekSliderPositionCommand = new DelegateCommand<double>(OnSeek);
            SetVolumeCommand = new DelegateCommand<double>(OnSetVolume);
            PreviousCommand = new DelegateCommand(OnPrevious);
            NextCommand = new DelegateCommand(OnNext);
            Playlist = new PlaylistViewModel(dialogProvider, messenger);
            InitSavedAudioDevice();
        }

        private void LoadAndPlayPlaylistTrack()
        {
            Engine.Load(Playlist.Selected.FilePath);
            Engine.Play();
        }

        private void OnNext(object obj)
        {
            if (Playlist.TryStepNext())
            {
                LoadAndPlayPlaylistTrack();
            }
        }

        private void OnPrevious(object obj)
        {
            if (Playlist.TryStepBack())
            {
                LoadAndPlayPlaylistTrack();
            }
        }

        private void OnSetVolume(double obj)
        {
            Engine.Volume = Convert.ToSingle(obj);
        }

        private void OnSeek(double obj)
        {
            Engine.SeeekTo(obj);
        }

        private void InitSavedAudioDevice()
        {
            if (Engine.AvailableOutputs.Any())
            {
                if (Settings.Default.AudioOutIndex < 0)
                {
                    SelectedAudioDevice = Engine.AvailableOutputs.First();
                }
                else
                {
                    var candidate = Engine.AvailableOutputs.FirstOrDefault(i => i.Index == Settings.Default.AudioOutIndex);
                    if (candidate == null)
                    {
                        SelectedAudioDevice = Engine.AvailableOutputs.First();
                    }
                    else
                    {
                        SelectedAudioDevice = candidate;
                    }
                }
            }
        }

        public void HandleMessage(LoadFileMessage message)
        {
            _dialogProvider.SetMainTab(MainTab.Play);
            Engine.Load(message.File);
            Engine.Play();
        }

        private void Onload(object obj)
        {
            if (_dialogProvider.TrySelectFileDialog(Formats.AudioFormatFilterString, out string selected))
            {
                Engine.Load(selected);
                Engine.Play();
            }
        }
    }
}
