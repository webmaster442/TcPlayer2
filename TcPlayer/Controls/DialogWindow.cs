// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Windows;
using TcPlayer.Infrastructure.Native;

namespace TcPlayer.Controls
{
    public class DialogWindow : Window
    {
        protected DialogWindow()
        {
            SourceInitialized += OnSourceInitialized;
        }

        private void OnSourceInitialized(object sender, EventArgs e)
        {
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            int currentStyle = User32.GetWindowLong(hwnd, User32.GWL_STYLE);
            User32.SetWindowLong(hwnd, User32.GWL_STYLE, (currentStyle & ~User32.WS_MAXIMIZEBOX & ~User32.WS_MINIMIZEBOX));
        }
    }
}
