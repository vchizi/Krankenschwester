using GalaSoft.MvvmLight.Messaging;
using Krankenschwester.Domain.Events;
using Krankenschwester.Presentation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Krankenschwester
{
    public class AppSettings
    {
        [JsonIgnore]
        private static string SettingsFolder = @".\data";
        private static string SettingsFile = SettingsFolder + @"\settings.json";

        public Main Main = new();

        public Dictionary<string, ProcessUsageCondition> ProcessUsageConditionPresets { get; private set; } = new Dictionary<string, ProcessUsageCondition>();

        public static AppSettings Load() => File.Exists(SettingsFile) ? JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(SettingsFile)) : new AppSettings();

        public void Save() {
            //if (!Directory.Exists(SettingsFolder))
            //{
            //    Directory.CreateDirectory(SettingsFolder);
            //}
            File.WriteAllText(SettingsFile, JsonConvert.SerializeObject(this));
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            //if (ImmortalitySettings != null)
            //    ImmortalitySettings.settings = this;
        }

        internal ProcessUsageCondition? GetActivePreset() => ProcessUsageConditionPresets.Where(e => e.Value.Activated).Select(e => e.Value).FirstOrDefault();
        internal PixelColor? GetFirstAnchor() => ProcessUsageConditionPresets.Where(e => e.Value.Anchor != null).Select(e => e.Value.Anchor).FirstOrDefault();

        internal void SavePreset(ProcessUsageCondition processUsageCondition)
        {
            processUsageCondition.Activate();
            ProcessUsageConditionPresets[processUsageCondition.Name] = processUsageCondition;

            foreach (var preset in ProcessUsageConditionPresets)
            {
                if (preset.Value.Name != processUsageCondition.Name)
                {
                    preset.Value.Deactivate();
                }
            }

            this.Save();
        }

        internal void RemovePreset(ProcessUsageCondition processUsageCondition)
        {
            ProcessUsageConditionPresets.Remove(processUsageCondition.Name);

            this.Save();
        }

        public bool UseMagnifier()
        {
            return Main.UseMagnifier;
        }
    }

    public class Main : ICloneable, INotifyPropertyChanged
    {
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

        private bool useMagnifier = false;
        public bool UseMagnifier
        {
            get => useMagnifier;
            set
            {
                useMagnifier = value;
                OnPropertyChanged(nameof(UseMagnifier));
            }

        }

        private bool cloneAnchor = true;
        public bool CloneAnchor
        {
            get => cloneAnchor;
            set
            {
                cloneAnchor = value;
                OnPropertyChanged(nameof(CloneAnchor));
            }

        }

        private Position position = new Position(0,0);
        public Position Position
        {
            get => position;
            set
            {
                position = value;
                OnPropertyChanged(nameof(Position));
            }

        }

        private string clientTxtPath = "";
        public string ClientTxtPath
        {
            get => clientTxtPath;
            set
            {
                var oldValue = clientTxtPath;
                clientTxtPath = value;

                if (string.IsNullOrEmpty(clientTxtPath))
                {
                    useClientTxtReader = false;
                }

                if (oldValue != clientTxtPath)
                {
                    Messenger.Default.Send(new ClientTxtPathChanged(clientTxtPath));
                }
                OnPropertyChanged(nameof(ClientTxtPath));
            }

        }

        private bool useClientTxtReader = false;
        public bool UseClientTxtReader
        {
            get => useClientTxtReader;
            set
            {
                var oldValue = useClientTxtReader;
                if (string.IsNullOrEmpty(clientTxtPath))
                {
                    useClientTxtReader = false;
                } else
                {
                    useClientTxtReader = value;
                }

                if (oldValue != useClientTxtReader)
                {
                    Messenger.Default.Send(new ClientTxtReaderUsageChanged(useClientTxtReader));
                }

                OnPropertyChanged(nameof(UseClientTxtReader));
            }

        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class Position(double top, double left): INotifyPropertyChanged
    {
        private double top = top;
        public double Top
        {
            get => top;
            set
            {
                top = value;
                OnPropertyChanged(nameof(Top));
            }

        }
        private double left = left;
        public double Left
        {
            get => left;
            set
            {
                left = value;
                OnPropertyChanged(nameof(Left));
            }

        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
