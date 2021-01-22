namespace TcPlayer.Engine.Models
{
    public record FileInfo
    {
        public string FilePath { get; init; }
        public string Artist { get; init; }
        public string Title { get; init; }

        public FileInfo()
        {
            FilePath = string.Empty;
            Artist = string.Empty;
            Title = string.Empty;
        }
    }
}
