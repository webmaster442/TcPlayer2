// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TcPlayer.Infrastructure
{
    internal static class BitmapHelper
    {
        public static BitmapSource ToBitmapSource(FrameworkElement source, double dpiX = 96, double dpiY = 96)
        {
            if (source == null) return null;

            var size = new Size(source.Width, source.Height);
            var bounds = new Rect(size);
            source.Measure(size);
            source.Arrange(bounds);


            var rtb = new RenderTargetBitmap((int)(bounds.Width * dpiX / 96.0),
                                             (int)(bounds.Height * dpiY / 96.0),
                                             dpiX,
                                             dpiY,
                                             PixelFormats.Pbgra32);

            rtb.Render(source);

            if (rtb.CanFreeze)
                rtb.Freeze();

            return rtb;
        }
    }
}
