// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TcPlayer.Engine.Models;

namespace TcPlayer.Engine.Internals
{
    public class YoutubeDlInterop
    {
        private const string TitleAndUrlWithThumbnail = "-g -e --get-thumbnail -f 251";
        private readonly string _youtubeDlPath;

        public bool IsInstalled { get; }

        public static bool IsYoutubeUrl(string url)
        {
            Regex fullPattern = new Regex(@"http(s){0,1}\:\/\/(www\.){0,1}youtube\.com\/watch\?v\=([a-zA-Z0-9-=]){11}");
            Regex shortPattern = new Regex(@"https\:\/\/youtu\.be\/([a-zA-Z0-9-=]){11}");

            return fullPattern.IsMatch(url)
                || shortPattern.IsMatch(url);
        }

        public YoutubeDlInterop()
        {
            string? pathVariable = Environment.GetEnvironmentVariable("Path");
            List<string> directories = pathVariable?.Split(';').ToList() ?? new List<string>();
            directories.Add(AppContext.BaseDirectory);

            _youtubeDlPath = string.Empty;
            IsInstalled = false;

            foreach (var directory in directories)
            {
                var file = Path.Combine(directory, "youtube-dl.exe");
                if (File.Exists(file))
                {
                    _youtubeDlPath = file;
                    IsInstalled = true;
                    break;
                }
            }
        }

        public async Task<YoutubeDlResponse> ExtractData(string url)
        {
            var processOutput = await AsyncProcess.ExecuteShellCommand(_youtubeDlPath, $"{TitleAndUrlWithThumbnail} {url}");
            if (processOutput.Completed && processOutput.ExitCode > -1 && !string.IsNullOrEmpty(processOutput.Output))
            {
                var response = processOutput.Output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                if (response.Length == 3)
                {
                    return new YoutubeDlResponse
                    {
                        Title = response[0].Trim(),
                        Url = response[1].Trim(),
                        Thumbnail = response[2].Trim(),
                        YoutubeUrl = url.Trim()
                    };
                }
            }
            return new YoutubeDlResponse();
        }
    }
}
