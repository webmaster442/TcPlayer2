using ManagedBass;
using ManagedBass.Mix;
using ManagedBass.Wasapi;
using System.Collections.Generic;
using TcPlayer.Engine.Internals;
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

        public AudioEngine()
        {
            Bass.Configure(Configuration.UpdateThreads, 0);
            Reset();
            CurrentState = EngineState.NoFile;
            _metadata = MetadataFactory.CreateEmpty();
        }

        private void Reset()
        {
            Length = 0.0f;
            CurrentPosition = 0.0f;
            Metadata = MetadataFactory.CreateEmpty();
        }

        private void Exception(string? message = null)
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
                CurrentPosition = BassMix.ChannelGetPosition(_decodeChannel, PositionFlags.Bytes);
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
            private set => SetProperty(ref _currentPosition, value);
        }

        public EngineState CurrentState
        {
            get => _currentState;
            private set => SetProperty(ref _currentState, value);
        }

        public Metadata Metadata
        {
            get => _metadata;
            private set => SetProperty(ref _metadata, value);
        }

        public IEnumerable<SoundDeviceInfo> AvailableOutputs => Wasapi.GetDevices();

        public bool IsSeeking { get; set; }

        public void Dispose()
        {
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
                Dispose();
            }
            if (Bass.Init(0, output.SamplingFrequency, DeviceInitFlags.Default) &&
                BassWasapi.Init(output.Index, output.SamplingFrequency, output.Channels, Wasapi.InitFlags, Wasapi.BufferSize)
                && BassWasapi.Start())
            {
                _currentChannels = output.Channels;
                _currentRate = output.SamplingFrequency;
                _isInitialized = true;
            }
            else
            {
                Exception();
            }
        }

        public void Load(string fileToPlay)
        {
            if (!_isInitialized)
            {
                Exception(Resources.ErrorNotInitialized);
            }
            _decodeChannel = Bass.CreateStream(fileToPlay, 0, 0, Wasapi.FileLoadFlags);
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
            Metadata = MetadataFactory.CreateFromFile(fileToPlay);
            long len = Bass.ChannelGetLength(_decodeChannel, PositionFlags.Bytes);
            Length = Bass.ChannelBytes2Seconds(_decodeChannel, len);
            long pos = BassMix.ChannelGetPosition(_decodeChannel, PositionFlags.Bytes);
            CurrentPosition = Bass.ChannelBytes2Seconds(_decodeChannel, pos);
            CurrentState = EngineState.ReadyToPlay;
            TimerEnabled = true;
        }

        public void Pause()
        {
            CurrentState = EngineState.Paused;
            TimerEnabled = false;
        }

        public void Play()
        {
            CurrentState = EngineState.Playing;
            TimerEnabled = true;
        }

        public void SeeekTo(float position)
        {
            var bytes = Bass.ChannelSeconds2Bytes(_decodeChannel, position);
            Bass.ChannelSetPosition(_decodeChannel, bytes);
        }

        public void Stop()
        {
            Reset();
            CurrentState = EngineState.ReadyToPlay;
            TimerEnabled = false;
        }
    }
}
