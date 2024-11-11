using GalaSoft.MvvmLight.Messaging;
using Krankenschwester.Application.Process;
using Krankenschwester.Domain.Events;
using Krankenschwester.Presentation;
using Krankenschwester.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krankenschwester.Application
{
    public class ProcessHandler
    {
        private readonly AppSettings settings;

        private readonly TaskBasedProcess process;

        private bool activated = false;
        private bool paused = false;

        public ProcessHandler(AppSettings settings) {

            Messenger.Default.Register<ProcessStateChanged>(this, StateChanged, true);
            Messenger.Default.Register<ProcessUpdated>(this, Updated, true);
            Messenger.Default.Register<EnteredZone>(this, ZoneEntered, true);

            this.settings = settings;
            activated = settings.Main.Activated;
            process = new TaskBasedProcess();

            System.Windows.Application.Current.Exit += Current_Exit;
        }

        private void Current_Exit(object sender, System.Windows.ExitEventArgs e)
        {
            process.Stop();
        }

        private void ZoneEntered(EnteredZone zone)
        {
            paused = !zone.WithFlasks;

            if (!paused)
            {
                TmpLogger.WriteLine("Unpause process");
                SartProcess();
            } else
            {
                TmpLogger.WriteLine("Pause process");
                process.Stop();
            }
        }

        private void SartProcess()
        {
            try
            {
                process.Start(getProcess());
            }
            catch (Exception ex)
            {
                TmpLogger.WriteLine(ex.Message);
            }
        }

        private void Updated(ProcessUpdated updated)
        {
            if (activated && !paused)
            {
                SartProcess();
            }
        }

        private void StateChanged(ProcessStateChanged changed)
        {
            if (changed.Activated) {
                activated = true;
                paused = false;

                TmpLogger.WriteLine("Starting process");
                SartProcess();
            } else
            {
                activated = false;
                paused = false;
                TmpLogger.WriteLine("Stop process");
                process.Stop();
            }
        }

        public ProcessUsageCondition getProcess()
        {
            var process = settings.GetActivePreset();

            if (process == null || process.UsageConditions.Where(e => e.Activated).FirstOrDefault() == null)
            {
                Messenger.Default.Send(new ProcessDeactivated("No active process to start!"));
                throw new Exception("No active process to start!");
            }

            if (process.Anchor == null)
            {
                Messenger.Default.Send(new ProcessDeactivated("Anchor is not set!"));
                throw new Exception("Anchor is not set!");
            }

            return process;
        }
    }
}
