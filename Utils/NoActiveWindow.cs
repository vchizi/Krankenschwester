using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Krankenschwester.Utils
{
    public class NoActiveWindow
    {
        // Needed to prevent window getting focus
        const int GWL_EXSTYLE = -20;
        const int WS_EX_NOACTIVATE = 134217728;
        const int LSFW_LOCK = 1;

        [DllImport("user32")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        // Needed to prevent Application taking focus
        [DllImport("user32")]
        public static extern bool LockSetForegroundWindow(uint UINT);

        public static void SetNoActiveWindow(Window window)
        {
            window.Loaded += delegate (object sender, RoutedEventArgs e)
            {
                WindowInteropHelper helper = new WindowInteropHelper(window);
                SetWindowLong(helper.Handle, GWL_EXSTYLE, WS_EX_NOACTIVATE);
                LockSetForegroundWindow(LSFW_LOCK);
            };
        }
    }
}
