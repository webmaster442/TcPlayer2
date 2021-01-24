using System.Xml.Serialization;

namespace TcPlayer.Engine.Models
{
    public record PlaylistItem
    {
        public string FilePath { get; init; }
        [XmlAttribute]
        public string Artist { get; init; }
        [XmlAttribute]
        public string Title { get; init; }
        [XmlAttribute]
        public double Length { get; init; }

        public PlaylistItem()
        {
            FilePath = string.Empty;
            Artist = string.Empty;
            Title = string.Empty;
        }
    }
}
