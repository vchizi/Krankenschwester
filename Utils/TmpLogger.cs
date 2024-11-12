
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace Krankenschwester.Utils
{
    public static class TmpLogger
    {

        private static string LogFile = @".\data\logs.txt";

        public static void WriteLine(string message)
        {
            if (System.Windows.Application.Current == null)
            {
                return;
            }

            if (!File.Exists(LogFile))
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    using (File.Create(LogFile)) ;
                });
            }

            try
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:FFFFF") + " - " + message);
                    File.AppendAllText(LogFile, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:FFFFF") + " - " + message + "\n");
                });
            }
            catch (System.NullReferenceException) { }
            catch (Exception) { }
        }
    }
}
