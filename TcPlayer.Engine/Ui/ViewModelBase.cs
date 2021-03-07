using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TcPlayer.Engine.Ui
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public bool SetProperty<T>(ref T backfield, T value, [CallerMemberName] string? propName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(backfield, value))
            {
                backfield = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
                return true;
            }
            return false;
        }

        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
