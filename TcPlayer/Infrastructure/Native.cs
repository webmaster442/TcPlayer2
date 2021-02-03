using System;
using System.Runtime.InteropServices;

namespace TcPlayer.Infrastructure
{
    internal static class Native
    {
        public const int GWL_STYLE = -16,
                     WS_MAXIMIZEBOX = 0x10000,
                     WS_MINIMIZEBOX = 0x20000;

        [DllImport("user32.dll")]
        public extern static int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public extern static int SetWindowLong(IntPtr hwnd, int index, int value);
    }
}
