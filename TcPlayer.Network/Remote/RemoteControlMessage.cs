namespace TcPlayer.Network.Remote
{
    public record RemoteControlMessage
    {
        public RemoteControlCommand Command { get; init; }
    }
}
