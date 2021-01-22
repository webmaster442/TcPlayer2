using ManagedBass;
using ManagedBass.Mix;
using ManagedBass.Wasapi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private float _volume;
        private bool _isvolumeSeeking;
        private bool _isSeeking;
        private readonly WasapiProcedure _process;

        public AudioEngine()
        {
            Bass.Configure(Configuration.UpdateThreads, 0);
            Reset();
            CurrentState = EngineState.NoFile;
            _metadata = MetadataFactory.CreateEmpty();
            _process = new WasapiProcedure(Process);
            Volume = 1;
        }

        private void Reset()
        {
            Length = 0.0;
            CurrentPosition = 0.0;
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
                long pos = BassMix.ChannelGetPosition(_decodeChannel, PositionFlags.Bytes);
                CurrentPosition = Bass.ChannelBytes2Seconds(_decodeChannel, pos);
            }
            if (!IsVolumeSeeking)
            {
                Volume = BassWasapi.GetVolume(WasapiVolumeTypes.Session);
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
            set => SetProperty(ref _currentPosition, value);
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
                SetProperty(ref _volume, value);
                BassWasapi.SetVolume(WasapiVolumeTypes.Session, value);
            }
        }

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
                BassWasapi.Init(output.Index, output.SamplingFrequency, output.Channels, Wasapi.InitFlags, Wasapi.BufferSize, 0, _process)
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

        private int Process(System.IntPtr Buffer, int Length, System.IntPtr User)
        {
            return Bass.ChannelGetData(_mixerChanel, Buffer, Length);
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
            TimerEnabled = false;
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
            Reset();
            CurrentState = EngineState.ReadyToPlay;
            TimerEnabled = false;
            Bass.ChannelStop(_mixerChanel);

        }

        public void SetVolume(float level)
        {
            if (!BassWasapi.SetVolume(WasapiVolumeTypes.Session, level))
            {
                Exception();
            }
        }
    }
}
