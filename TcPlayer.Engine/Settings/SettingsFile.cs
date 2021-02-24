using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TcPlayer.Engine.Settings
{
    public sealed class SettingsFile: ISettingsFile
    {
        private readonly string _openFileName;
        private readonly Dictionary<Setting, string> _storage;

        public SettingsFile(string fileName)
        {
            _storage = new Dictionary<Setting, string>();

            _openFileName = fileName;
            if (File.Exists(_openFileName))
            {
                var temp = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(_openFileName));
                if (temp == null)
                    throw new InvalidOperationException();

                foreach (var container in temp.Keys)
                {
                    foreach (var setting in temp[container])
                    {
                        _storage.Add(new Setting
                        {
                            Container = Guid.Parse(container),
                            Key = setting.Key 
                        },
                        setting.Value);
                    }
                }
            }
            Settings = new SettingsProvider(_storage);
        }

        public ISettingProvider Settings { get; }

        public void Save()
        {
            var toSerialize = new Dictionary<string, Dictionary<string, string>>();

            foreach (var item in _storage)
            {
                var containerId = item.Key.Container.ToString();
                if (toSerialize.ContainsKey(containerId))
                {
                    toSerialize[containerId].Add(item.Key.Key, item.Value);
                }
                else
                {
                    toSerialize.Add(containerId, new Dictionary<string, string>());
                    toSerialize[containerId].Add(item.Key.Key, item.Value);
                }
            }

            var newContents = JsonSerializer.Serialize(toSerialize, new JsonSerializerOptions
            {
                WriteIndented = true,
            });
            var oldfile = Path.ChangeExtension(_openFileName, ".old");

            if (File.Exists(_openFileName))
                File.Move(_openFileName, oldfile);

            using (var f = File.CreateText(_openFileName))
            {
                f.Write(newContents);
            }
            File.Delete(oldfile);
        }
    }
}
