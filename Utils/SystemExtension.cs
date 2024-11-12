using Newtonsoft.Json;
using System.Diagnostics;

namespace Krankenschwester.Utils
{
    public static class SystemExtension
    {
        public const int SLEEP_TIME = 50;

        public static T Clone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);

            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static void ExitIfAlreadyRunning()
        {
            string procName = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(procName);
            if (processes.Length > 1)
            {
                System.Windows.MessageBox.Show(procName + " already running");

                System.Windows.Application.Current.Shutdown();
            }
        }
    }
}
