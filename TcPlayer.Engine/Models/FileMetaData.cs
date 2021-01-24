using System;

namespace TcPlayer.Engine.Models
{
    public record FileMetaData
    {
        public string FilePath { get; init; }
        public string Artist { get; init; }
        public string Title { get; init; }
        public double Length { get; init; }

        public FileMetaData()
        {
            FilePath = string.Empty;
            Artist = string.Empty;
            Title = string.Empty;
        }
    }
}
