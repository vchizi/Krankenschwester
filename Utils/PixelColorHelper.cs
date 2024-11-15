using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Krankenschwester.Utils
{
    public static class PixelColorHelper
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        public static uint GetColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(IntPtr.Zero, hdc);

            return pixel;
        }

        public static string GetHtmlColor(uint pixel)
        {
            var color = Color.FromArgb((byte)(pixel & 0x000000FF), (byte)((pixel & 0x0000FF00) >> 8), (byte)((pixel & 0x00FF0000) >> 16));

            return ColorTranslator.ToHtml(color);
        }

        public static void GetColorFast(int x, int y)
        {
            Int32[] Bits = new Int32[2560 * 1440];
            GCHandle BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);

            Bitmap image = new Bitmap(2560, 1440, 2560 * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
            Graphics gr = Graphics.FromImage(image);
            gr.CopyFromScreen(0, 0, 0, 0, image.Size);

            gr.Dispose();
            image.Dispose();
            BitsHandle.Free();

            return;
        }
    }
}
