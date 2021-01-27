namespace TcPlayer.Engine.Messages
{
    public record LoadFileMessage
    {
        public string File { get; init; }

        public LoadFileMessage()
        {
            File = string.Empty;
        }
    }
}
