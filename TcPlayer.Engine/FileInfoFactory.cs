using System.Collections.Generic;
using TagLib;
using TcPlayer.Engine.Models;

namespace TcPlayer.Engine
{
    public static class FileInfoFactory
    {
        public static IEnumerable<FileMetaData> CreateFileInfos(IEnumerable<string> items)
        {

            foreach (var item in items)
            {
                File f = File.Create(item);
                var artist = f.Tag.Performers?.Length > 0 ? f.Tag.Performers[0] : string.Empty;

                yield return new FileMetaData
                {
                    FilePath = item,
                    Title = $"{artist} - {f.Tag.Title}",
                    Length = f.Properties.Duration,
                };
            }
        }
    }
}
