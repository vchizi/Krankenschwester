using System.IO;

namespace Krankenschwester.Presentation
{
    public class TrayIcon: IDisposable
    {
        private readonly NotifyIcon NIcon = new NotifyIcon();
        ContextMenuStrip cMenu = new ContextMenuStrip();

        public TrayIcon()
        {
            Stream iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Resources/icon.ico")).Stream;
            NIcon.Icon = new Icon(iconStream);
            NIcon.Visible = true;
            NIcon.ContextMenuStrip = cMenu;
        }

        public void AddItem(string name, EventHandler EventHandler) {
            var item = cMenu.Items.Add(name);
            item.Click += EventHandler;
        }

        public void Dispose()
        {
            NIcon.Dispose();
        }
    }
}
