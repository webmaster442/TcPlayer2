using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace TcPlayer.Controls
{
    public class EqConverter : MarkupExtension, IMultiValueConverter
    {
        private const int VerticalHeight = 115;

        private static double MapY(double input)
        {
            var point = (input - 1) * (VerticalHeight - 0) / (-1 - 1);
            return point;
        }

        private static double MapX(int x, double width)
        {
            double avaliable = width / 6;
            return avaliable * x;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double width = 800;
            if (parameter != null)
                width = System.Convert.ToDouble(parameter);

            PointCollection points = new PointCollection(7);
            points.Add(new Point(0, MapY(0)));
            int x =1;
            foreach (var value in values.OfType<double>())
            {
                points.Add(new Point(MapX(x, width), MapY(value)));
                ++x;
            }
            points.Add(new Point(width, MapY(0)));
            return points;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return Array.Empty<object>();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
