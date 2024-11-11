
using GalaSoft.MvvmLight.Messaging;
using Krankenschwester.Domain.Events;
using Krankenschwester.Utils;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;

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

        private bool Activated = false;
        private readonly AppSettings settings;

        public MainButton(AppSettings settings)
        {
            InitializeComponent();
            this.settings = settings;

            Activated = settings.Main.Activated;

            ControlButton.DataContext = new ButtonBackground(Activated ? ActivatedIco : DeactivatedIco);
            ControlButton.MouseEnter += ControlButton_MouseEnter;
            ControlButton.MouseLeave += ControlButton_MouseLeave;

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

            ControlButton.DataContext = new ButtonBackground(Activated ? ActivatedIco : DeactivatedIco);

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

        private void ControlButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ControlButton.DataContext = new ButtonBackground(Activated ? ActivatedIco : DeactivatedIco);
        }

        private void ControlButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ControlButton.DataContext = new ButtonBackground(HoveredIco);
        }
    }

    public class ButtonBackground : INotifyPropertyChanged
    {
        public ButtonBackground(string imageSource)
        {
            ImageSource = imageSource;
        }

        private string _imageSource;
        public string ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
