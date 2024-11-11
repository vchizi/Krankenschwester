using Krankenschwester.Presentation;
using Krankenschwester.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Krankenschwester.Application.Process
{
    class ThreadBasedProcess : Process
    {
        private volatile bool alive;

        private List<Thread> WorkerThreads = new List<Thread>();

        public void Start(ProcessUsageCondition process)
        {
            Stop();
            alive = true;

            foreach (var usageCondition in process.UsageConditions)
            {
                //var usageCondition = process.UsageConditions[0];
                if (!usageCondition.Activated || usageCondition.Usage == null) continue;
                //if (!usageCondition.Activated || usageCondition.Usage == null) return;

                Thread t = new(() => {
                    TmpLogger.WriteLine("Created Thread!");
                    try
                    {
                        while (alive)
                        {
                            ProcessCondition(process.Anchor, usageCondition);
                        }
                    }
                    catch (ThreadInterruptedException exception)
                    {
                        TmpLogger.WriteLine(exception.ToString());
                    }
                });
                t.IsBackground = true;

                WorkerThreads.Add(t);

                t.Start();
            }
        }

        private void ProcessCondition(PixelColor anchor, UsageCondition usageCondition)
        {
            uint color = PixelColorHelper.GetColor(usageCondition.Usage.X, usageCondition.Usage.Y);

            bool isWindowActive = POEWindow.IsWindowActive() == true;
            bool anchorFound = anchor.Color == PixelColorHelper.GetColor(anchor.X, anchor.Y);
            if (isWindowActive && anchorFound && ((usageCondition.Usage.Color != color && !usageCondition.Reversed) || (usageCondition.Usage.Color == color && usageCondition.Reversed)))
            {
                POEWindow.SendKeyPressToWindow((int)usageCondition.Key);
                //TmpLogger.WriteLine("Pressed key: " + usageCondition.Key);
                Thread.Sleep(usageCondition.Timeout);
                return;
            }

            Thread.Sleep(SystemExtension.SLEEP_TIME);
        }

        public void Stop()
        {
            alive = false;
            foreach (Thread thread in WorkerThreads.ToList())
            {
                thread.Interrupt();
                thread.Join(100);
                TmpLogger.WriteLine("Interrupt Thread!");
            }
            WorkerThreads.Clear();
        }
    }
}
