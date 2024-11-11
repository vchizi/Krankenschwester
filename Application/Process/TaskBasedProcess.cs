using Krankenschwester.Presentation;
using Krankenschwester.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace Krankenschwester.Application.Process
{
    internal class TaskBasedProcess
    {
        private volatile bool alive;

        private List<Task> tasks = new List<Task>();

        CancellationTokenSource cancellationTokenSource = new();

        async public Task Start(ProcessUsageCondition process)
        {
            await Stop();
            alive = true;

            foreach (var usageCondition in process.UsageConditions)
            {
                if (!usageCondition.Activated || usageCondition.Usage == null)
                {
                    TmpLogger.WriteLine("There is no Active condition or Usage is not defined");
                    continue;
                }

                var task = Task.Run(async () =>
                {
                    TmpLogger.WriteLine("Created Task!");
                    try
                    {
                        while (alive && !cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            var executed = ProcessCondition(process.Anchor, usageCondition);

                            await Task.Delay(executed && usageCondition.Timeout >= SystemExtension.SLEEP_TIME ? usageCondition.Timeout : SystemExtension.SLEEP_TIME, cancellationTokenSource.Token);
                        }
                    }
                    catch (Exception exception)
                    {
                        TmpLogger.WriteLine(exception.ToString());
                    }
                }, cancellationTokenSource.Token);

                tasks.Add(task);
            }
        }

        private bool ProcessCondition(PixelColor anchor, UsageCondition usageCondition)
        {
            uint color = PixelColorHelper.GetColor(usageCondition.Usage.X, usageCondition.Usage.Y);

            bool isWindowActive = POEWindow.IsWindowActive() == true;
            bool anchorFound = anchor.Color == PixelColorHelper.GetColor(anchor.X, anchor.Y);
            if (isWindowActive && anchorFound && ((usageCondition.Usage.Color != color && !usageCondition.Reversed) || (usageCondition.Usage.Color == color && usageCondition.Reversed)))
            {
                POEWindow.SendKeyPressToWindow((int)usageCondition.Key);

                return true;
                //TmpLogger.WriteLine("Pressed key: " + usageCondition.Key);
                //Thread.Sleep(usageCondition.Timeout);
            }

            return false;
        }

        async public Task Stop()
        {
            // Signal all tasks to stop
            alive = false;
            cancellationTokenSource.Cancel();

            try
            {
                // Wait for all tasks to complete without blocking the main thread
                //Task.WaitAll(tasks.ToArray());
                await Task.WhenAll(tasks);
                //Thread.Sleep(1000);
            }
            catch (OperationCanceledException)
            {
                TmpLogger.WriteLine("Tasks were canceled.");
            }
            catch (Exception exception)
            {
                TmpLogger.WriteLine(exception.ToString());
            }

            TmpLogger.WriteLine("Stopping all");
            // Clean up and prepare for new tasks

            tasks.Clear();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
        }
    }
}
