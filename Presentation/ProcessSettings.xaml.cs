using GalaSoft.MvvmLight.Messaging;
using Krankenschwester.Domain.Events;
using Krankenschwester.Presentation.Control;
using Krankenschwester.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using WindowsInput.Native;

namespace Krankenschwester.Presentation
{
    /// <summary>
    /// Interaction logic for ProcessSettings.xaml
    /// </summary>
    public partial class ProcessSettings : Window
    {
        private static ProcessSettings instance;

        private ProcessUsageCondition processUsageCondition;
        private readonly AppSettings settings;

        private const string STATIC_PRESET_NAME = "Add new...";

        private MagnifierWindow MagnifierWindow;

        public ProcessSettings(AppSettings settings)
        {
            InitializeComponent();

            this.Closing += ProcessSettings_Closing;
            this.settings = settings;
            
            PresetsList.SelectionChanged += PresetsList_SelectionChanged;
            populatePresetsList();

            processUsageCondition = settings.GetActivePreset() ?? new ProcessUsageCondition("Test");
            DataContext = processUsageCondition;
            PresetsList.SelectedValue = processUsageCondition;
        }

        public static void Display(AppSettings settings)
        {
            if (instance == null) 
            { 
                instance = new ProcessSettings(settings); 
            }

            instance.Show();
            instance.WindowState = WindowState.Normal;
        }

        private void ProcessSettings_Closing(object? sender, EventArgs e)
        {
            instance = null;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            addUsageCondition(processUsageCondition.AddUsageCondition());
        }

        private void addUsageCondition(UsageCondition usageCondition)
        {
            var control = new UsageConditionControl(settings, usageCondition);
            UsageConditionsPanel.Children.Add(control);

            control.RemoveButton.Click += (object sender, RoutedEventArgs e) => {
                UsageConditionsPanel.Children.Remove(control);
                processUsageCondition.UsageConditions.Remove(usageCondition);
            };
        }

        private async void AnchorColorButton_Click(object sender, RoutedEventArgs e)
        {
            //MouseHook.Start();
            //MouseHook.MouseAction += SetAnchorEvent;
            //TmpLogger.WriteLine("Subscribed");

            //if (settings.UseMagnifier() && MagnifierWindow == null) {
            //    MagnifierWindow = new MagnifierWindow();
            //    MagnifierWindow.Show();
            //}
            await Task.Run(() => InitializeAnchorClick());
        }

        public void InitializeAnchorClick()
        {
            MouseHook.Start();
            MouseHook.MouseAction += SetAnchorEvent;

            System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
            {
                if (settings.UseMagnifier() && MagnifierWindow == null)
                {
                    MagnifierWindow = new MagnifierWindow();
                    MagnifierWindow.Show();
                }
            });
        }

        private void SetAnchorEvent(object sender, MouseClickEvent e)
        {
            this.processUsageCondition.Anchor = new PixelColor(e.X, e.Y, PixelColorHelper.GetColor(e.X, e.Y));

            MouseHook.MouseAction -= SetAnchorEvent;
            MouseHook.Stop();

            System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
            {
                if (settings.UseMagnifier())
                {
                    MagnifierWindow.Close();
                    MagnifierWindow = null;
                }
            });
           
        }

        private void populatePresetsList()
        {
            PresetsList.Items.Clear();
            PresetsList.Items.Add(new ProcessUsageCondition(STATIC_PRESET_NAME));
            foreach (var preset in settings.ProcessUsageConditionPresets)
            {
                PresetsList.Items.Add(preset.Value);
            }
        }

        private void PresetsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (ProcessUsageCondition)PresetsList.SelectedItem;
            if (selected == null)
            {
                return;
            }

            NewPresetName.Text = selected.Name;
            if (selected.Name == STATIC_PRESET_NAME)
            {
                NewPresetName.Text = string.Empty;
                DeletePreset.IsEnabled = false;
            }
            else
            {
                DeletePreset.IsEnabled = true;
            }

            //WHY???
            processUsageCondition = SystemExtension.Clone(selected);
            if (settings.Main.CloneAnchor && selected.Name == STATIC_PRESET_NAME)
            {
                processUsageCondition.Anchor = SystemExtension.Clone(settings.GetFirstAnchor());
            }

            DataContext = processUsageCondition;

            UsageConditionsPanel.Children.Clear();
            foreach (var item in processUsageCondition.UsageConditions)
            {
                addUsageCondition(item);
            }
        }

        private void SavePreset_Click(object sender, RoutedEventArgs e)
        {
            SavePresetGrid.Visibility = Visibility.Visible;
            PresetGrid.Visibility = Visibility.Collapsed;
        }

        private void DeletePreset_Click(object sender, RoutedEventArgs e)
        {
            DeletePresetConfirmationGrid.Visibility = Visibility.Visible;
            PresetGrid.Visibility = Visibility.Collapsed;
        }

        private void SaveUpdatePreset_Click(object sender, RoutedEventArgs e)
        {
            SavePresetGrid.Visibility = Visibility.Collapsed;
            PresetGrid.Visibility = Visibility.Visible;

            if (NewPresetName.Text == String.Empty || NewPresetName.Text == STATIC_PRESET_NAME)
            {
                return;
            }

            if (processUsageCondition.Name != NewPresetName.Text)
            {
                var Item = SystemExtension.Clone(processUsageCondition);
                Item.Name = NewPresetName.Text;
                settings.SavePreset(Item);
            }
            else
            {
                settings.SavePreset(processUsageCondition);
            }

            Messenger.Default.Send(new ProcessUpdated());

            Close();
        }

        private void CancelUpdatePreset_Click(object sender, RoutedEventArgs e)
        {
            SavePresetGrid.Visibility = Visibility.Collapsed;
            PresetGrid.Visibility = Visibility.Visible;
        }

        private void DeleteConfirm_Click(object sender, RoutedEventArgs e)
        {
            var selected = (ProcessUsageCondition)PresetsList.SelectedItem;
            if (selected == null || selected.Name == STATIC_PRESET_NAME)
            {
                return;
            }

            settings.RemovePreset(processUsageCondition);

            populatePresetsList();

            processUsageCondition = new ProcessUsageCondition(STATIC_PRESET_NAME);
            if (settings.Main.CloneAnchor)
            {
                processUsageCondition.Anchor = SystemExtension.Clone(settings.GetFirstAnchor());
            }

            DataContext = processUsageCondition;

            DeletePresetConfirmationGrid.Visibility = Visibility.Collapsed;
            PresetGrid.Visibility = Visibility.Visible;

            PresetsList.SelectedIndex = 0;

            Messenger.Default.Send(new ProcessUpdated());
        }

        private void DeleteCancel_Click(object sender, RoutedEventArgs e)
        {
            DeletePresetConfirmationGrid.Visibility = Visibility.Collapsed;
            PresetGrid.Visibility = Visibility.Visible;
        }
    }

    public class ProcessUsageCondition(string name) : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private bool activated = false;
        public bool Activated
        {
            get => activated;
            set
            {
                activated = value;
                OnPropertyChanged(nameof(Activated));
            }
        }

        private string name = name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private PixelColor? anchor;
        public PixelColor? Anchor
        {
            get => anchor;
            set
            {
                anchor = value;
                AnchorLabel = anchor != null ? string.Format("x:{0} y:{1}", anchor.X, anchor.Y) : anchorLabel;
                OnPropertyChanged(nameof(Anchor));
                OnPropertyChanged(nameof(AnchorForeground));
            }
        }

        private string anchorLabel = "x:0 y:0";
        public string AnchorLabel
        {
            get => anchorLabel;
            private set
            {
                anchorLabel = value;
                OnPropertyChanged(nameof(AnchorLabel));
            }
        }

        private SolidColorBrush anchorForeground => AnchorForeground;
        public SolidColorBrush AnchorForeground
        {
            get => anchor?.GetSolidColorBrush() ?? new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
        }

        public List<UsageCondition> UsageConditions { get; private set; } = new() {};
        public UsageCondition AddUsageCondition()
        {
            UsageCondition condition = new UsageCondition();
            UsageConditions.Add(condition);

            return condition;
        }

        public void Activate()
        {
            Activated = true;
        }

        public void Deactivate()
        {
            Activated = false;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class UsageCondition : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private bool activated = false;
        public bool Activated
        {
            get => activated;
            set
            {
                activated = value;
                OnPropertyChanged(nameof(Activated));
            }
        }

        private bool reversed = false;
        public bool Reversed
        {
            get => reversed;
            set
            {
                reversed = value;
                OnPropertyChanged(nameof(Reversed));
            }
        }

        private PixelColor? usage;
        public PixelColor? Usage
        {
            get => usage;
            set
            {
                usage = value;
                UsageLabel = usage != null ? string.Format("x:{0} y:{1}", usage.X, usage.Y) : usageLabel;
                OnPropertyChanged(nameof(Usage));
                OnPropertyChanged(nameof(UsageForeground));
            }
        }

        private string usageLabel = "x:0 y:0";
        public string UsageLabel
        {
            get => usageLabel;
            private set
            {
                usageLabel = value;
                OnPropertyChanged(nameof(UsageLabel));
            }
        }

        private SolidColorBrush usageForeground => UsageForeground;
        public SolidColorBrush UsageForeground
        {
            get => usage?.GetSolidColorBrush() ?? new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
        }

        private VirtualKeyCode key = VirtualKeyCode.VK_1;
        public VirtualKeyCode Key
        {
            get => key;
            set
            {
                key = value;
                OnPropertyChanged(nameof(Key));
            }
        }

        private int timeout = 100;
        public int Timeout
        {
            get => timeout;
            set
            {
                timeout = value;
                OnPropertyChanged(nameof(Timeout));
            }
        }
    }

    public class PixelColor(int x, int y, uint color)
    {
        public int X { get; private set; } = x;

        public int Y { get; private set; } = y;

        public uint Color { get; private set; } = color;

        public SolidColorBrush GetSolidColorBrush()
        {
            return new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(PixelColorHelper.GetHtmlColor(Color)));
        }
    }
}
