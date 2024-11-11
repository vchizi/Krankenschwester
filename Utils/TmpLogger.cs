
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

        public static void WriteLine(string message)
        {
            if (System.Windows.Application.Current == null)
            {
                return;
            }

            try
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:FFFFF") + " - " + message);
                });
            }
            catch (System.NullReferenceException) { }
            catch (Exception) { }
        }
    }
}
