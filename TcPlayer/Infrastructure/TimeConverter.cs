using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace TcPlayer.Infrastructure
{
    internal class TimeConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double seconds)
            {
                var time = TimeSpan.FromSeconds(seconds);
                if (time.Hours > 0)
                    return $"{time.Hours}:{time.Minutes}:{time.Seconds}";
                else
                    return $"{time.Minutes}:{time.Seconds}";
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str 
                && TimeSpan.TryParse(str, out TimeSpan time))
            {
                return time.TotalSeconds;
            }
            return Binding.DoNothing;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
