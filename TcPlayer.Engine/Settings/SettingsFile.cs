using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TcPlayer.Engine.Settings
{
    public sealed class SettingsFile
    {
        private readonly string _openFileName;
        private readonly Dictionary<Setting, string> _storage;

        public SettingsFile(string fileName)
        {
            _storage = new Dictionary<Setting, string>();

            _openFileName = fileName;
            if (File.Exists(_openFileName))
            {
                var temp = JsonSerializer.Deserialize<Dictionary<Setting, string>>(File.ReadAllText(_openFileName));
                if (temp == null)
                    throw new InvalidOperationException();

                _storage = temp;
            }
            else
            {
                _storage = new Dictionary<Setting, string>();
                var empty = JsonSerializer.Serialize(_storage);
                using (var f = File.CreateText(_openFileName))
                {
                    f.Write(empty);
                }
            }
            Settings = new SettingsProvider(_storage);
        }

        public ISettingProvider Settings { get; }

        public void Save()
        {
            var newContents = JsonSerializer.Serialize(_storage);
            var oldfile = Path.ChangeExtension(_openFileName, ".old");
            File.Move(_openFileName, oldfile);
            using (var f = File.CreateText(_openFileName))
            {
                f.Write(newContents);
            }
            File.Delete(oldfile);
        }
    }
}
