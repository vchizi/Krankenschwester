using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace Krankenschwester.Utils
{
    public class POEWindow
    {
        private static readonly string WindowClass = "POEWindowClass";
        private static readonly string WindowName = "Path of Exile";

        // Get a handle to an application window.
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        // Activate an application window.
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        // Activate an application window.
        [DllImport("USER32.DLL")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        private static IntPtr PoeHandle = IntPtr.Zero;

        public static bool IsWindowActive()
        {
            var poeWindow = FindWindow(WindowClass, WindowName);
            if (poeWindow == IntPtr.Zero)
            {
                PoeHandle = IntPtr.Zero;
            }

            return poeWindow == GetForegroundWindow();
        }

        public static void MakeActive(IntPtr? PoeHandle = null)
        {
            if (PoeHandle == null)
            {
                PoeHandle = FindPoeWindow();
            }

            // Need to press ALT because the SetForegroundWindow sometimes does not work
            //iSim.Keyboard.KeyPress(VirtualKeyCode.MENU);

            SetForegroundWindow((IntPtr)PoeHandle);
        }

        private static IntPtr FindPoeWindow()
        {
            if (PoeHandle != IntPtr.Zero)
            {
                return PoeHandle;
            }

            PoeHandle = FindWindow(WindowClass, WindowName);
            if (PoeHandle == IntPtr.Zero)
            {
                throw new ArgumentNullException(WindowName + " is not Running");
            }

            return PoeHandle;
        }

        public static void SendKeyPressToWindow(int key)
        {
            const uint WM_KEYDOWN = 0x0100;
            //const uint WM_KEYUP = 0x0101;
            PostMessage(FindPoeWindow(), WM_KEYDOWN, key, 0);
        }
    }
}
