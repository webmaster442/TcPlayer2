namespace TcPlayer.Engine.Models
{
    public record ChapterInfo
    {
        public string Name { get; init; }
        public double TimeStamp { get; init; }

        public ChapterInfo()
        {
            Name = string.Empty;
        }
    }
}
