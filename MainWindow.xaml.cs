using Krankenschwester.Application;
using Krankenschwester.Application.LogReader;
using Krankenschwester.Presentation;
using Krankenschwester.Presentation.Control;
using Krankenschwester.Utils;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Krankenschwester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TrayIcon trayIcon;

        private AppSettings settings = AppSettings.Load();
        private ClientTxtReader clientReader;
        private SettingsWindow settingsWindow;

        public MainWindow()
        {
            SystemExtension.ExitIfAlreadyRunning();

            InitializeComponent();
            NoActiveWindow.SetNoActiveWindow(this);

            DataContext = settings.Main;

            ButtonsStack.Children.Add(new MainButton(settings));

            trayIcon = new TrayIcon();
            trayIcon.AddItem("Settings", CMenu_OpenSettings);
            trayIcon.AddItem("Exit", CMenu_Close);

            new ProcessHandler(settings);
            clientReader = new ClientTxtReader(settings);
        }

        private void CMenu_OpenSettings(object? sender, EventArgs e)
        {
            if (settingsWindow == null)
            {
                settingsWindow = new SettingsWindow(settings);
                settingsWindow.Show();

                settingsWindow.Closed += SettingsWindow_Closed;
            }
        }

        private void SettingsWindow_Closed(object? sender, EventArgs e)
        {
            settingsWindow.Closed -= SettingsWindow_Closed;
            settingsWindow = null;
        }

        private void CMenu_Close(object sender, EventArgs e)
        {
            clientReader.Dispose();

            trayIcon.Dispose();

            Close();
            System.Windows.Application.Current.Shutdown();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            settings.Save();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}