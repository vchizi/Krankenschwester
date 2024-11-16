
using GalaSoft.MvvmLight.Messaging;
using Krankenschwester.Domain.Events;
using Krankenschwester.Utils;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Krankenschwester.Presentation.Control
{
    /// <summary>
    /// Interaction logic for MainButton.xaml
    /// </summary>
    public partial class MainButton : System.Windows.Controls.UserControl
    {
        private static string ActivatedIco = "/Resources/heart-active.ico";
        private static string DeactivatedIco = "/Resources/heart-deactivated.ico";
        private static string HoveredIco = "/Resources/heart-hover.ico";

        protected bool _activated = false;
        private bool Activated
        {
            get => _activated;
            set
            {
                if (value != _activated)
                {
                    _activated = value;
                    ActivatedChanged();
                }
            }
        }
        private readonly AppSettings settings;

        public MainButton(AppSettings settings)
        {
            InitializeComponent();
            this.settings = settings;

            Activated = settings.Main.Activated;

            ControlButton.Click += ControlButton_Click;
            ControlButton.MouseRightButtonDown += ControlButton_MouseRightButtonDown;

            if (Activated)
            {
                Messenger.Default.Send(new ProcessStateChanged(Activated));
            }

            Messenger.Default.Register<ProcessDeactivated>(this, processDeactivated, true);
        }

        private void processDeactivated(ProcessDeactivated deactivated)
        {
            Activated = false;

            settings.Main.Activated = Activated;
            settings.Save();

            if (deactivated.Message != "") MessageBox.Show(deactivated.Message);
        }

        private void ControlButton_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ProcessSettings.Display(settings);
        }

        private void ControlButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Activated = !Activated;

            settings.Main.Activated = Activated;
            settings.Save();

            Messenger.Default.Send(new ProcessStateChanged(Activated));
        }

        protected void ActivatedChanged()
        {
            if (Activated)
            {
                ControlButton.Foreground = System.Windows.Media.Brushes.OrangeRed;
            }
            else
            {
                ControlButton.Foreground = System.Windows.Media.Brushes.White;
            }
        }
    }
}
