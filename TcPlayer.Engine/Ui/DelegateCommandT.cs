using System;
using System.Windows.Input;

namespace TcPlayer.Engine.Ui
{
    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T?> _execute;
        private readonly Predicate<T?>? _canExecute;

        public event EventHandler? CanExecuteChanged;

        public DelegateCommand(Action<T?> execute, Predicate<T?>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            if (_canExecute == null) return true;

            return _canExecute((T)parameter);
        }

        public void Execute(object? parameter)
        {
            if (typeof(T) != parameter?.GetType())
            {
                if (parameter is IConvertible && typeof(T).GetInterface(nameof(IConvertible)) != null)
                {
                    var converted = (T)Convert.ChangeType(parameter, typeof(T));
                    _execute(converted);
                }
                else
                {
                    throw new InvalidCastException(nameof(parameter));
                }
            }
            else
                _execute((T)parameter);
        }


        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
