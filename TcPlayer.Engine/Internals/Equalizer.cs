using ManagedBass;
using ManagedBass.Fx;
using System;
using System.Runtime.InteropServices;

namespace TcPlayer.Engine.Internals
{
    internal sealed class Equalizer: IDisposable
    {
        private const float EqualizerBandwidth = 2.5f;

        private static readonly float[] EqualizerCenterFrequencies = new float[]
        {
            60.0f, 240.0f, 1000.0f, 3500.0f, 15000.0f
        };

        public static readonly int BandCount = 5;

        private int _fx;
        private GCHandle _handle;
        private PeakEQParameters _eq;
        private float[] _defaultConfig = new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        private int _channelHandle;

        public Equalizer(int channelHandle)
        {
            _eq = new PeakEQParameters
            {
                fGain = 0.0f,
                fBandwidth = EqualizerBandwidth,
                lChannel = FXChannelFlags.All,
            };
            _handle = GCHandle.Alloc(_eq, GCHandleType.Pinned);

            _channelHandle = channelHandle;

            _fx = Bass.ChannelSetFX(_channelHandle, EffectType.PeakEQ, 0);

            if (_fx == 0)
            {
                throw new EngineException($"Error: {Bass.LastError}");
            }

            for (int i = 0; i < BandCount; i++)
            {
                _eq.lBand = i;
                _eq.fCenter = EqualizerCenterFrequencies[i];
                Bass.FXSetParameters(_fx, _handle.AddrOfPinnedObject());
            }
            UpdateEqualizerConfig(_defaultConfig);
        }

        public void UpdateEqualizerConfig(float[] configuration)
        {
            for (int i = 0; i < BandCount; i++)
            {
                _eq.lBand = i;
                Bass.FXGetParameters(_fx, _handle.AddrOfPinnedObject());
                _eq.fGain = configuration[i] * 10;
                Bass.FXSetParameters(_fx, _handle.AddrOfPinnedObject());
            }
        }

        public void Dispose()
        {
            if (_channelHandle != 0)
            {
                Bass.ChannelRemoveFX(_channelHandle, _fx);
                _channelHandle = 0;
                _fx = 0;
            }

            if (_handle.IsAllocated)
            {
                _handle.Free();
            }
        }
    }
}
