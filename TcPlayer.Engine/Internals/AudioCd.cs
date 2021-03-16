// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using ManagedBass.Cd;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TcPlayer.Engine.Models;

namespace TcPlayer.Engine.Internals
{
    public static class AudioCd
    {
        public static Dictionary<string, string> CdDataCache { get; } = new();

        internal static string GetAlbum()
        {
            if (CdDataCache.ContainsKey($"TITLE0"))
                return CdDataCache[$"TITLE0"];
            else
                return $"Unknown album";
        }

        internal static string GetPerformer(int trackIndex)
        {
            if (CdDataCache.ContainsKey($"PERFORMER{trackIndex}"))
                return CdDataCache[$"PERFORMER{trackIndex}"];
            else
                return "Unknown artist";
        }

        internal static string GetTitle(int trackIndex)
        {
            if (CdDataCache.ContainsKey($"TITLE{trackIndex}"))
                return CdDataCache[$"TITLE{trackIndex}"];
            else
                return $"Track #{trackIndex}";
        }

        public static Task<IEnumerable<PlaylistItem>> LoadCd(int driveIndex, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                List<PlaylistItem> results = new List<PlaylistItem>();
                cancellationToken.ThrowIfCancellationRequested();
                CdDataCache.Clear();
                if (BassCd.IsReady(driveIndex))
                {
                    var cdText = BassCd.GetIDText(driveIndex);
                    if (cdText.Length > 0)
                    {
                        foreach (var data in cdText)
                        {
                            var item = data.Split('=');
                            CdDataCache.Add(item[0], item[1]);
                        }
                    }

                    int tracks = BassCd.GetTracks(driveIndex);

                    for (int i = 1; i <= tracks; i++)
                    {
                        results.Add(new PlaylistItem
                        {
                            FilePath = $"cd://{driveIndex}/{i}",
                            Length = BassCd.GetTrackLength(driveIndex, i),
                            Artist = GetPerformer(i),
                            Title = GetTitle(i)
                        });
                    }
                    BassCd.Release(driveIndex);

                }
                return results.AsEnumerable();
            });
        }

    }
}
