using TcPlayer.Engine.Settings;
using TcPlayer.Engine.Ui;

namespace TcPlayer.ViewModels
{
    internal class SettingsViewModel : ViewModelBase
    {
        private readonly ISettingsFile _settingsFile;

        private bool _notifySongChange;
        private bool _showInstallDialog;

        public bool NotifySongChange 
        {
            get { return _notifySongChange; }
            set
            {
                SetProperty(ref _notifySongChange, value);
                Save();
            }
        }

        public bool ShowInstallDialog 
        {
            get { return _showInstallDialog; }
            set
            {
                SetProperty(ref _showInstallDialog, value);
                Save();
            }
        }

        public SettingsViewModel(ISettingsFile settingsFile)
        {
            _settingsFile = settingsFile;
            Update();

        }

        private void Update()
        {
            _notifySongChange = true;
            if (_settingsFile.Settings.IsExisting(SettingConst.AppSettings, SettingConst.NotifySongChange))
            {
                _notifySongChange = _settingsFile.Settings.GetBool(SettingConst.AppSettings, SettingConst.NotifySongChange);
            }
            if (_settingsFile.Settings.IsExisting(SettingConst.AppSettings, SettingConst.Installed))
            {
                _showInstallDialog = !_settingsFile.Settings.GetBool(SettingConst.AppSettings, SettingConst.Installed);
            }
        }

        private void Save()
        {
            _settingsFile.Settings.WriteBool(SettingConst.AppSettings, SettingConst.NotifySongChange, _notifySongChange);
            _settingsFile.Settings.WriteBool(SettingConst.AppSettings, SettingConst.Installed, !_showInstallDialog);
            _settingsFile.Save();
        }
    }
}
