using System;
using System.Runtime.InteropServices;

namespace CSharpUtils.Utils
{
    class User32
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
    }
}
