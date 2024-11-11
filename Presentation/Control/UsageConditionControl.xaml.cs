using Krankenschwester.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsInput.Native;

namespace Krankenschwester.Presentation.Control
{
    /// <summary>
    /// Interaction logic for UsageConditionControl.xaml
    /// </summary>
    public partial class UsageConditionControl : System.Windows.Controls.UserControl
    {
        private readonly AppSettings settings;
        private readonly UsageCondition usageCondition;

        private MagnifierWindow MagnifierWindow;

        public UsageConditionControl(AppSettings settings, UsageCondition usageCondition)
        {
            InitializeComponent();
            this.settings = settings;
            this.usageCondition = usageCondition;

            foreach (var val in Enum.GetValues<VirtualKeyCode>())
            {
                Keys.Items.Add(val);
            }

            DataContext = this.usageCondition;
        }

        private void UseFlaskColorButton_Click(object sender, RoutedEventArgs e)
        {
            MouseHook.Start();
            MouseHook.MouseAction += SetUsageEvent;

            if (settings.UseMagnifier())
            {
                MagnifierWindow = new MagnifierWindow();
                MagnifierWindow.Show();
            }
        }

        private void SetUsageEvent(object sender, MouseClickEvent e)
        {
            this.usageCondition.Usage = new PixelColor(e.X, e.Y, PixelColorHelper.GetColor(e.X, e.Y));

            MouseHook.MouseAction -= SetUsageEvent;
            MouseHook.Stop();

            if (settings.UseMagnifier())
            {
                MagnifierWindow.Close();
                MagnifierWindow = null;
            }
        }
    }
}
