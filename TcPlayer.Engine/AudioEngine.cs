using ManagedBass;
using ManagedBass.Mix;
using ManagedBass.Tags;
using ManagedBass.Wasapi;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TcPlayer.Engine.Internals;
using TcPlayer.Engine.Messages;
using TcPlayer.Engine.Models;
using TcPlayer.Engine.Properties;

namespace TcPlayer.Engine
{
    public sealed class AudioEngine : NotifyObject, IEngine
    {
        private bool _isInitialized;
        private int _decodeChannel;
        private int _mixerChanel;

        private int _currentChannels;
        private int _currentRate;
        private double _currentPosition;
        private double _length;
        private EngineState _currentState;
        private Metadata _metadata;
        private float _volume;
        private bool _isvolumeSeeking;
        private bool _isSeeking;

        private readonly WasapiProcedure _waspiCallback;
        private readonly DownloadProcedure _downloadCallback;
        private readonly IMessenger _messenger;
        private string? _lastStreamInfo;
        private string _currentFile;

        private Equalizer? _equalizer;
        
        public AudioEngine(IMessenger messenger)
        {
            _messenger = messenger;
            Bass.Configure(Configuration.UpdateThreads, 0);
            Reset();
            CurrentState = EngineState.NoFile;
            _metadata = MetadataFactory.CreateEmpty();
            _waspiCallback = new WasapiProcedure(OnWasapiCallback);
            _downloadCallback = new DownloadProcedure(OnDownload);
            _currentFile = string.Empty;
        }

        private void Reset()
        {
            _currentFile = string.Empty;
            Length = 0.0;
            CurrentPosition = 0.0;
            IsSeeking = false;
            CurrentState = EngineState.NoFile;
            Metadata = MetadataFactory.CreateEmpty();
            TimerEnabled = false;
            _equalizer?.Dispose();
            _equalizer = null;
        }

        private static void Exception(string? message = null)
        {
            if (message != null)
            {
                throw new EngineException(message);
            }
            throw new EngineException($"Error: {Bass.LastError}");
        }

        protected override void UpdateTimer()
        {
            if (!IsSeeking)
            {
                long pos = BassMix.ChannelGetPosition(_decodeChannel, PositionFlags.Bytes);
                CurrentPosition = Bass.ChannelBytes2Seconds(_decodeChannel, pos);
            }
        }

        protected override void UpdateTimerLong()
        {
            float vol = BassWasapi.GetVolume(WasapiVolumeTypes.Session | WasapiVolumeTypes.WindowsHybridCurve);
            if (vol != _volume)
            {
                Volume = vol;
            }
        }

        public double Length
        {
            get => _length;
            private set => SetProperty(ref _length, value);
        }

        public double CurrentPosition
        {
            get => _currentPosition;
            set
            {
                SetProperty(ref _currentPosition, value);
                SendTaskBarInfoMessage();
            }
        }

        public EngineState CurrentState
        {
            get => _currentState;
            private set
            {
                SetProperty(ref _currentState, value);
                SendTaskBarInfoMessage();
            }
        }

        private void SendTaskBarInfoMessage()
        {
            _messenger.SendMessage(new PositionInfoMessage
            {
                State = CurrentState,
                Percent = double.IsInfinity(_length) || double.IsNaN(_length) ? double.PositiveInfinity : (_currentPosition / _length)
            });
        }

        public Metadata Metadata
        {
            get => _metadata;
            private set => SetProperty(ref _metadata, value);
        }

        public IEnumerable<SoundDeviceInfo> AvailableOutputs => Wasapi.GetDevices();

        public bool IsSeeking
        {
            get => _isSeeking;
            set => SetProperty(ref _isSeeking, value);
        }

        public bool IsVolumeSeeking
        {
            get => _isvolumeSeeking;
            set => SetProperty(ref _isvolumeSeeking, value);
        }

        public float Volume
        {
            get => _volume;
            set
            {
                float target = value > 1.0f ? 1.0f : value;
                SetProperty(ref _volume, target);
                if (_isInitialized)
                    BassWasapi.SetVolume(WasapiVolumeTypes.Session | WasapiVolumeTypes.WindowsHybridCurve, target);
            }
        }

        public void Dispose()
        {
            if (_equalizer != null)
            {
                _equalizer.Dispose();
                _equalizer = null;
            }
            if (_mixerChanel != 0)
            {
                Bass.StreamFree(_mixerChanel);
                _mixerChanel = 0;
            }
            if (_decodeChannel != 0)
            {
                Bass.StreamFree(_decodeChannel);
                _decodeChannel = 0;
            }
            if (_isInitialized)
            {
                BassWasapi.Free();
                Bass.Free();
                _isInitialized = false;
            }
        }

        public void Initialize(SoundDeviceInfo output)
        {
            if (_isInitialized)
            {
                Reset();
                Dispose();
            }
            if (Bass.Init(0, output.SamplingFrequency, DeviceInitFlags.Default) &&
                BassWasapi.Init(output.Index, output.SamplingFrequency, output.Channels, Wasapi.InitFlags, Wasapi.BufferSize, 0, _waspiCallback)
                && BassWasapi.Start())
            {
                _currentChannels = output.Channels;
                _currentRate = output.SamplingFrequency;
                _isInitialized = true;
                _volume = BassWasapi.GetVolume(WasapiVolumeTypes.WindowsHybridCurve | WasapiVolumeTypes.Session);
                OnPropertyChanged(nameof(Volume));
            }
            else
            {
                Exception();
            }
        }

        private int OnWasapiCallback(System.IntPtr Buffer, int Length, System.IntPtr User)
        {
            return Bass.ChannelGetData(_mixerChanel, Buffer, Length);
        }

        private void OnDownload(System.IntPtr Buffer, int Length, System.IntPtr User)
        {
            var ptr = Bass.ChannelGetTags(_decodeChannel, TagType.META);
            if (ptr != IntPtr.Zero)
            {
                var streamInfo = Marshal.PtrToStringAnsi(ptr);
                if (_lastStreamInfo != streamInfo)
                {
                    Metadata = NetworkMetadataFactory.CreateFromStream(_currentFile, streamInfo);
                    _lastStreamInfo = streamInfo;
                }
            }
            else
            {
                Metadata = NetworkMetadataFactory.CreateFromBassTags(_decodeChannel, _currentFile);
            }
        }

        public void Load(string fileToPlay)
        {
            Reset();
            if (!_isInitialized)
            {
                Exception(Resources.ErrorNotInitialized);
            }

            if (MediaLoader.IsStream(fileToPlay))
            {
                _lastStreamInfo = string.Empty;
                _decodeChannel = Bass.CreateStream(fileToPlay, 0, Wasapi.FileLoadFlags, _downloadCallback);
                Metadata = NetworkMetadataFactory.CreateFromStream(_currentFile, string.Empty);
            }
            else
            {
                _decodeChannel = MediaLoader.LoadLocalFile(fileToPlay, Wasapi.FileLoadFlags);
                Metadata = MetadataFactory.CreateFromFile(fileToPlay);
            }

            if (_decodeChannel == 0)
            {
                Exception();
            }
            _mixerChanel = BassMix.CreateMixerStream(_currentRate, _currentChannels, Wasapi.MixerFlags);
            if (_mixerChanel == 0)
            {
                Exception();
            }
            if (!BassMix.MixerAddChannel(_mixerChanel, _decodeChannel, BassFlags.MixerDownMix))
            {
                Exception();
            }

            _equalizer = new Equalizer(_mixerChanel);

            long len = Bass.ChannelGetLength(_decodeChannel, PositionFlags.Bytes);
            if (len < 0)
                Length = double.PositiveInfinity;
            else
                Length = Bass.ChannelBytes2Seconds(_decodeChannel, len);

            long pos = BassMix.ChannelGetPosition(_decodeChannel, PositionFlags.Bytes);
            CurrentPosition = Bass.ChannelBytes2Seconds(_decodeChannel, pos);
            CurrentState = EngineState.ReadyToPlay;
            TimerEnabled = false;
            _currentFile = fileToPlay;
        }

        public void Pause()
        {
            CurrentState = EngineState.Paused;
            TimerEnabled = false;
            Bass.ChannelStop(_mixerChanel);
        }

        public void Play()
        {
            CurrentState = EngineState.Playing;
            Bass.ChannelSetPosition(_mixerChanel, 0, PositionFlags.Bytes);
            TimerEnabled = true;
        }

        public void SeeekTo(double position)
        {
            var bytes = Bass.ChannelSeconds2Bytes(_decodeChannel, position);
            Bass.ChannelSetPosition(_decodeChannel, bytes);
        }

        public void Stop()
        {
            Bass.ChannelStop(_mixerChanel);
            Reset();
            CurrentState = EngineState.ReadyToPlay;
        }

        public void SetEqualizerParameters(float[] parameters)
        {
            if (_equalizer == null)
                return;

            _equalizer.UpdateEqualizerConfig(parameters);
        }
    }
}
