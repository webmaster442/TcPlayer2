using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using TcPlayer.Engine.Models;

namespace TcPlayer.Engine
{
    public class PlaylistFormat
    {
        public static IEnumerable<PlaylistItem> Load(string file)
        {
            using (var stream = File.OpenRead(file))
            {
                XmlSerializer xs = new XmlSerializer(typeof(PlaylistItem[]));
                var data = xs.Deserialize(stream);
                if (data != null)
                    return (PlaylistItem[])data;
                return Enumerable.Empty<PlaylistItem>();
            }
        }

        public static void Save(string file, PlaylistItem[] items)
        {
            using (var stream = File.Create(file))
            {
                XmlSerializer xs = new XmlSerializer(typeof(PlaylistItem[]));
                xs.Serialize(stream, items);
            }
        }
    }
}
