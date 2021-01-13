namespace TcPlayer.Engine
{
    public record SoundDeviceInfo
    {
        public int Index { get; init; }
        public string Name { get; init; }
        public int Channels { get; init; }
        public int SamplingFrequency { get; init; }
    }
}
