using TcPlayer.Engine;
using TcPlayer.Engine.Ui;
using TcPlayer.Infrastructure;

namespace TcPlayer.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        private readonly IDialogProvider _dialogProvider;
        private SoundDeviceInfo _selectedAudioDevice;

        public IEngine Engine { get; }

        public DelegateCommand PlayCommand { get; }
        public DelegateCommand PauseCommand { get; }
        public DelegateCommand StopCommand { get; }
        public DelegateCommand LoadCommand { get; }

        public SoundDeviceInfo SelectedAudioDevice
        {
            get => _selectedAudioDevice;
            set
            {
                if (SetProperty(ref _selectedAudioDevice, value))
                {
                    Engine.Initialize(_selectedAudioDevice);
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
        }

        private void Onload(object obj)
        {
            if (_dialogProvider.TrySelectFileDialog(Constants.AudioFormats, out string selected))
            {
                Engine.Load(selected);
                Engine.Play();
            }
        }
    }
}
