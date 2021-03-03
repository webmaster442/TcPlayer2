using ManagedBass;
using ManagedBass.Wasapi;
using System.Collections.Generic;

namespace TcPlayer.Engine.Internals
{
    internal static class Wasapi
    {
        public const WasapiInitFlags InitFlags = WasapiInitFlags.AutoFormat | WasapiInitFlags.Buffer;
        public const float BufferSize = 0.1f;
        public const BassFlags FileLoadFlags = BassFlags.Decode | BassFlags.Float;
        public const BassFlags MixerFlags = BassFlags.Float | BassFlags.Decode | BassFlags.MixerPositionEx;

        public static IEnumerable<SoundDeviceInfo> GetDevices()
        {
            for (int i = 0; BassWasapi.GetDeviceInfo(i, out WasapiDeviceInfo info); i++) 
            {
                if (!info.IsInput && info.IsEnabled)
                {
                    yield return new SoundDeviceInfo
                    {
                        Index = i,
                        Name = info.Name,
                        Channels = info.MixChannels,
                        SamplingFrequency = info.MixFrequency
                    };
                }
            }
        }
    }
}
