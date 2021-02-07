using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Control;

namespace TcPlayer
{
    internal class SessionManager
    {
        private SystemMediaTransportControls _systemMediaControls;

        public SessionManager()
        {
            _systemMediaControls = SystemMediaTransportControls.GetForCurrentView();
            _systemMediaControls.IsEnabled = false;
            _systemMediaControls.ButtonPressed += _systemMediaControls_ButtonPressed;
        }

        private void _systemMediaControls_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
