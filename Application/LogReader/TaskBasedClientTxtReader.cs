using GalaSoft.MvvmLight.Messaging;
using Krankenschwester.Domain.Events;
using Krankenschwester.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace Krankenschwester.Application.LogReader
{
    public class TaskBasedClientTxtReader
    {
        private FileStream FileStream;
        private StreamReader StreamReader;
        private System.Timers.Timer Timer;

        private LogsListener logsListener = new LogsListener();
        private readonly AppSettings settings;

        private string clientTxtPath = "";

        private CancellationTokenSource _cancellationTokenSource;

        public TaskBasedClientTxtReader(AppSettings settings)
        {
            this.settings = settings;
            this.clientTxtPath = settings.Main.ClientTxtPath;

            Messenger.Default.Register<ClientTxtPathChanged>(this, clientTxtPathChanged, true);
            Messenger.Default.Register<ClientTxtReaderUsageChanged>(this, clientTxtReaderUsageChanged, true);

            System.Windows.Application.Current.Exit += Current_Exit;

            init();
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            Dispose();
            TmpLogger.WriteLine("Exit in TaskBasedClientTxtReader");
        }

        private void clientTxtReaderUsageChanged(ClientTxtReaderUsageChanged changed)
        {
            init();
        }

        private void clientTxtPathChanged(ClientTxtPathChanged changed)
        {
            if (clientTxtPath != changed.Path)
            {
                init();
            }
        }

        public async void init()
        {
            Dispose();

            if (!string.IsNullOrEmpty(clientTxtPath) && File.Exists(clientTxtPath) && settings.Main.UseClientTxtReader)
            {
                FileStream = new FileStream(clientTxtPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader = new StreamReader(FileStream, Encoding.UTF8);

                //TmpLogger.WriteLine("Start preloading Client.txt the file!");
                //while (StreamReader.ReadLine() != null) { }
                //TmpLogger.WriteLine("Client.txt has been read till end. Start monitoring new lines!");

                StartReading();
            }
        }

        public void StartReading()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            _ = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    await OnReadLoopAsync();
                    await Task.Delay(500, token); // Wait 1 second between checks
                }
            }, token);
        }

        private async Task OnReadLoopAsync()
        {
            if (FileStream.Position == 0)
            {
                FileStream.Seek(0, SeekOrigin.End);
            }

            if (FileStream.Position < FileStream.Length)
            {
                string line;
                while ((line = await StreamReader.ReadLineAsync()) != null)
                {
                    await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        try
                        {
                            logsListener.Read(line);
                        }
                        catch (Exception ex)
                        {
                            TmpLogger.WriteLine(ex.ToString());
                        }
                    });
                }
            }
        }

        public void StopReading()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        public void Dispose()
        {
            StopReading();

            StreamReader?.Close();
            StreamReader?.Dispose();
            FileStream?.Close();
            FileStream?.Dispose();
        }
    }
}
