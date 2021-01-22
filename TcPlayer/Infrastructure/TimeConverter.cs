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
                    return $"{time.Hours:00}:{time.Minutes:00}:{time.Seconds:00}";
                else
                    return $"{time.Minutes:00}:{time.Seconds:00}";
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
