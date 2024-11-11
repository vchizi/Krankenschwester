using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Krankenschwester.Presentation
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private string LogFileDirectory;

        private readonly AppSettings settings;

        public SettingsWindow(AppSettings settings)
        {
            InitializeComponent();
            this.settings = settings;

            DataContext = this.settings.Main;
        }

        private void OpenPathToClient_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(settings.Main.ClientTxtPath))
            {
                Process[] processes = Process.GetProcessesByName("PathOfExile_x64Steam");
                if (processes.Length > 0)
                {
                    LogFileDirectory = processes[0].Modules[0].FileName.Replace("PathOfExile_x64Steam.exe", @"logs\");
                }
            }
            else
            {
                LogFileDirectory = settings.Main.ClientTxtPath.Replace(@"logs\Client.txt", "");
            }

            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            if (Directory.Exists(LogFileDirectory))
            {
                ofd.InitialDirectory = LogFileDirectory;
            }
            ofd.Filter = "Text files (*.txt)|*.txt";

            if (ofd.ShowDialog() == true)
            {
                settings.Main.ClientTxtPath = ofd.FileName;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            settings.Save();

            Close();    
        }
    }
}
