using System;
using System.Linq;
using TcPlayer.Engine;
using TcPlayer.Engine.Ui;
using TcPlayer.Infrastructure;
using TcPlayer.Properties;

namespace TcPlayer.ViewModels
{
    internal sealed class MainViewModel : ViewModelBase
    {
        private readonly IDialogProvider _dialogProvider;
        private SoundDeviceInfo _selectedAudioDevice;

        public IEngine Engine { get; }

        public DelegateCommand PlayCommand { get; }
        public DelegateCommand PauseCommand { get; }
        public DelegateCommand StopCommand { get; }
        public DelegateCommand LoadCommand { get; }
        public DelegateCommand<double> SeekSliderPositionCommand { get; }
        public DelegateCommand<double> SetVolumeCommand { get; }

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

        public MainViewModel(IEngine engine, IDialogProvider dialogProvider)
        {
            Engine = engine;
            _dialogProvider = dialogProvider;

            PlayCommand = new DelegateCommand((o) => Engine.Play());
            StopCommand = new DelegateCommand((o) => Engine.Stop());
            PauseCommand = new DelegateCommand((o) => Engine.Pause());
            LoadCommand = new DelegateCommand(Onload);
            SeekSliderPositionCommand = new DelegateCommand<double>(OnSeek);
            SetVolumeCommand = new DelegateCommand<double>(OnSetVolume);

            InitSavedAudioDevice();
        }

        private void OnSetVolume(double obj)
        {
            Engine.Volume = (float)obj;
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
