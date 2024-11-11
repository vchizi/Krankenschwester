using System.Windows;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Interop;

namespace Krankenschwester.Utils
{
    public class Magnifier
    {
        private const int MagnifierSize = 100;
        private const int ZoomFactor = 2;
        private readonly Window _window;
        private readonly System.Windows.Controls.Image _imageControl;
        private readonly DispatcherTimer _timer;

        private (int Width, int Height) primaryMonitorDimensions = ScreenHelper.GetPrimaryMonitorDimensions();

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref System.Drawing.Point lpPoint);

        public Magnifier(Window window, System.Windows.Controls.Image imageControl)
        {
            _window = window;
            _imageControl = imageControl;

            _window.Topmost = true;
            _window.ShowInTaskbar = false;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(SystemExtension.SLEEP_TIME); // Update every 50ms
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            System.Drawing.Point cursorPos = new System.Drawing.Point();
            GetCursorPos(ref cursorPos);

            // Adjust the position of the magnifier
            _window.Left = cursorPos.X + 5 + _window.Width <= primaryMonitorDimensions.Width ? cursorPos.X + 5 : cursorPos.X - _window.Width;
            _window.Top = cursorPos.Y + 5 + _window.Height <= primaryMonitorDimensions.Height ? cursorPos.Y + 5 : cursorPos.Y - _window.Height;

            // Capture the area of the screen around the cursor
            var captureWidth = MagnifierSize / ZoomFactor;
            var captureHeight = MagnifierSize / ZoomFactor;

            Bitmap screenBitmap = CaptureScreen(cursorPos.X - captureWidth / 2, cursorPos.Y - captureHeight / 2, captureWidth, captureHeight);

            // Create a magnified image and display it
            Bitmap magnifiedBitmap = MagnifyImage(screenBitmap, ZoomFactor);
            DrawCenterIndicator(magnifiedBitmap);
            _imageControl.Source = BitmapToImageSource(magnifiedBitmap);
        }

        private Bitmap CaptureScreen(int x, int y, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(width, height));
            }
            return bitmap;
        }

        private Bitmap MagnifyImage(Bitmap original, int zoomFactor)
        {
            int width = original.Width * zoomFactor;
            int height = original.Height * zoomFactor;
            Bitmap magnified = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(magnified))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(original, 0, 0, width, height);
            }

            return magnified;
        }

        private static ImageSource BitmapToImageSource(Bitmap bitmap)
        {
            var handle = bitmap.GetHbitmap();
            var imageSource = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            return imageSource;
        }

        private void DrawCenterIndicator(Bitmap magnifiedBitmap)
        {
            // Draw a simple crosshair or use a cursor image for the center
            using (Graphics g = Graphics.FromImage(magnifiedBitmap))
            {
                // Set up a simple crosshair at the center of the image
                int centerX = magnifiedBitmap.Width / 2;
                int centerY = magnifiedBitmap.Height / 2;

                // Vertical line of the crosshair
                g.DrawLine(Pens.Red, centerX, 0, centerX, magnifiedBitmap.Height);

                // Horizontal line of the crosshair
                g.DrawLine(Pens.Red, 0, centerY, magnifiedBitmap.Width, centerY);
            }
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
