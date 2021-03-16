// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

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
                if (double.IsNaN(seconds)
                    || double.IsInfinity(seconds))
                    return seconds.ToString();

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
