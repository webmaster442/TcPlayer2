// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TcPlayer.Engine.Internals
{
    internal static class StreamTagsParser
    {
        private static readonly Regex TagRegex = new Regex(@"(.+)\=\'(.+)\'", RegexOptions.Compiled);

        public const string TagTitle = "StreamTitle";
        public const string TagUrl = "StreamUrl";

        public static Dictionary<string, string> GetTags(string rawMetaData)
        {
            var result = new Dictionary<string, string>();

            foreach (var tag in rawMetaData.Split(';'))
            {
                if (!TagRegex.IsMatch(tag))
                    continue;

                var keyValue = TagRegex.Split(tag)
                    .Where(s => s.Length > 0)
                    .ToArray();

                if (result.ContainsKey(keyValue[0]))
                    result[keyValue[0]] = keyValue[1];
                else
                    result.Add(keyValue[0], keyValue[1]);
            }

            return result;
        }

        public static string GetKey(this Dictionary<string, string> keyValuePairs, string key)
        {
            if (keyValuePairs.ContainsKey(key))
                return keyValuePairs[key];
            else
                return string.Empty;
        }
    }
}
