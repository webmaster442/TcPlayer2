using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TagLib;
using TcPlayer.Engine.Models;

namespace TcPlayer.Engine
{
    public static class FileInfoFactory
    {
        public static Task<IEnumerable<FileMetaData>> CreateFileInfos(IEnumerable<string> items, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var bag = new ConcurrentBag<FileMetaData>();

                ParallelOptions po = new ParallelOptions();
                po.CancellationToken = cancellationToken;

                Parallel.ForEach(items, po, item =>
                {
                    File f = File.Create(item);
                    var artist = f.Tag.Performers?.Length > 0 ? f.Tag.Performers[0] : string.Empty;
                    bag.Add(new FileMetaData
                    {
                        FilePath = item,
                        Title = $"{artist} - {f.Tag.Title}",
                        Length = f.Properties.Duration,
                    });
                });

                return bag as IEnumerable<FileMetaData>;
            });
        }
    }
}
