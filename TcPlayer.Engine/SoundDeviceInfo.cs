// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

namespace TcPlayer.Engine
{
    public record SoundDeviceInfo
    {
        public int Index { get; init; }
        public string Name { get; init; }
        public int Channels { get; init; }
        public int SamplingFrequency { get; init; }

        public SoundDeviceInfo()
        {
            Name = string.Empty;
        }
    }
}
