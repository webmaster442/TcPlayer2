using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using TcPlayer.Engine.Models;

namespace TcPlayer.Infrastructure
{
    internal class StateToEnabledConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EngineState state)
            {
                return state switch
                {
                    EngineState.Paused or EngineState.Playing or EngineState.ReadyToPlay or EngineState.Seeking => true,
                    _ => false,
                };
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
