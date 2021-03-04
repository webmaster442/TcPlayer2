namespace TcPlayer.Remote
{
    public record RemoteControlMessage
    {
        public RemoteControlCommand Command { get; init; }
        public int Value { get; init; }
    }
}
