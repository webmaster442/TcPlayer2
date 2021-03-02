using System;
using System.Linq;
using System.Threading.Tasks;
using TcPlayer.Engine;
using TcPlayer.Engine.Internals;
using TcPlayer.Engine.Messages;
using TcPlayer.Engine.Models;
using TcPlayer.Engine.Settings;
using TcPlayer.Engine.Ui;
using TcPlayer.Infrastructure;

namespace TcPlayer.ViewModels
{
    internal sealed class MainViewModel : ViewModelBase, IMessageClient<LoadFileMessage>
    {
        private readonly IDialogProvider _dialogProvider;
        private readonly ISettingsFile _settingsFile;
        private SoundDeviceInfo _selectedAudioDevice;
        private float[] _currentEq;
        private readonly YoutubeDlInterop _youtubeInterop;

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

        public DelegateCommand<float[]> ApplyEqCommand { get; }

        public SoundDeviceInfo SelectedAudioDevice
        {
            get => _selectedAudioDevice;
            set
            {
                if (SetProperty(ref _selectedAudioDevice, value))
                {
                    Engine.Initialize(_selectedAudioDevice);
                    _settingsFile.Settings.WriteInt(SettingConst.AudioSettings, SettingConst.AudioOutput, _selectedAudioDevice.Index);
                    _settingsFile.Save();
                }
            }
        }

        public float[] CurrentEq 
        { 
            get => _currentEq;
            set
            {
                SetProperty(ref _currentEq, value);
                _settingsFile.Settings.WriteList(SettingConst.AudioSettings, SettingConst.Equalizer, value);
                _settingsFile.Save();
            }

        }

        public Guid MessageReciverID => Guid.NewGuid();

        public MainViewModel(IEngine engine, IDialogProvider dialogProvider, IMessenger messenger, ISettingsFile settingsFile)
        {
            Engine = engine;
            _dialogProvider = dialogProvider;
            _settingsFile = settingsFile;
            messenger.SubScribe(this);
            _youtubeInterop = new YoutubeDlInterop();

            PlayCommand = new DelegateCommand((o) => Engine.Play());
            StopCommand = new DelegateCommand((o) => Engine.Stop());
            PauseCommand = new DelegateCommand((o) => Engine.Pause());
            LoadCommand = new DelegateCommand(Onload);
            SeekSliderPositionCommand = new DelegateCommand<double>(OnSeek);
            SetVolumeCommand = new DelegateCommand<double>(OnSetVolume);
            PreviousCommand = new DelegateCommand(OnPrevious);
            NextCommand = new DelegateCommand(OnNext);
            ApplyEqCommand = new DelegateCommand<float[]>(OnApplyEq);
            Playlist = new PlaylistViewModel(dialogProvider, messenger);
            InitSavedAudioDevice();
            InitSavedEq();

        }

        private void OnApplyEq(float[] obj)
        {
            Engine.SetEqualizerParameters(obj);
            CurrentEq = obj;
        }

        private async Task<bool> LoadAndPlay(string filename)
        {
            if (YoutubeDlInterop.IsYoutubeUrl(filename))
            {
                if (_youtubeInterop.IsInstalled)
                {
                    YoutubeDlResponse response = await _youtubeInterop.ExtractData(filename);
                    if (Engine.LoadYoutube(response))
                    {
                        Engine.Play();
                        Engine.SetEqualizerParameters(CurrentEq);
                        return true;
                    }
                }
                return false;
            }
            else
            {
                Engine.Load(filename);
                Engine.Play();
                Engine.SetEqualizerParameters(CurrentEq);
                return true;
            }
        }

        public async void HandleMessage(LoadFileMessage message)
        {
            _dialogProvider.SetMainTab(MainTab.Play);
            _dialogProvider.ShowUiBlocker();
            await LoadAndPlay(message.File);
            _dialogProvider.HideUiBlocker();
        }

        private void Onload(object obj)
        {
            if (_dialogProvider.TrySelectFileDialog(Formats.AudioFormatFilterString, out string selected))
            {
                Engine.Load(selected);
                Engine.Play();
                Engine.SetEqualizerParameters(CurrentEq);
            }
        }


        private async void OnNext(object obj)
        {
            if (Playlist.TryStepNext())
            {
                _dialogProvider.ShowUiBlocker();
                await LoadAndPlay(Playlist.Selected.FilePath);
                _dialogProvider.HideUiBlocker();
            }
        }

        private async void OnPrevious(object obj)
        {
            if (Playlist.TryStepBack())
            {
                _dialogProvider.ShowUiBlocker();
                await LoadAndPlay(Playlist.Selected.FilePath);
                _dialogProvider.HideUiBlocker();
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
                if (!_settingsFile.Settings.IsExisting(SettingConst.AudioSettings, SettingConst.AudioOutput)
                    || _settingsFile.Settings.GetInt(SettingConst.AudioSettings, SettingConst.AudioOutput) < 0)
                {
                    var output = Engine.AvailableOutputs.FirstOrDefault();
                    if (output == null)
                        throw new InvalidOperationException("No sound device was found");

                    SelectedAudioDevice = output;
                }
                else
                {
                    var candidate = Engine.AvailableOutputs.FirstOrDefault(i => i.Index == _settingsFile.Settings.GetInt(SettingConst.AudioSettings, SettingConst.AudioOutput));
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

        private void InitSavedEq()
        {
            if (_settingsFile.Settings.IsExisting(SettingConst.AudioSettings, SettingConst.Equalizer))
            {
                CurrentEq = _settingsFile.Settings.GetList<float>(SettingConst.AudioSettings, SettingConst.Equalizer).ToArray();
            }
            else
            {
                CurrentEq = new float[5];
            }

        }
    }
}
