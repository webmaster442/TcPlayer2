using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcPlayer.Engine.Settings
{
    public interface ISettingsFile
    {
        ISettingProvider Settings { get; }
        void Save();
    }
}
