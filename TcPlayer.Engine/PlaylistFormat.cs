using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using TcPlayer.Engine.Models;

namespace TcPlayer.Engine
{
    public class PlaylistFormat
    {
        public static IEnumerable<FileMetaData> Load(string file)
        {
            using (var stream = File.OpenRead(file))
            {
                XmlSerializer xs = new XmlSerializer(typeof(FileMetaData[]));
                var data = xs.Deserialize(stream);
                if (data != null)
                    return (FileMetaData[])data;
                return Enumerable.Empty<FileMetaData>();
            }
        }

        public static void Save(string file, FileMetaData[] items)
        {
            using (var stream = File.Create(file))
            {
                XmlSerializer xs = new XmlSerializer(typeof(FileMetaData[]));
                xs.Serialize(stream, items);
            }
        }
    }
}
