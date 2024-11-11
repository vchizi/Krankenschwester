using GalaSoft.MvvmLight.Messaging;
using Krankenschwester.Domain.Events;
using Krankenschwester.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace Krankenschwester.Application.LogReader
{
    public class ClientTxtReader : IDisposable
    {
        private FileStream FileStream;
        private StreamReader StreamReader;
        private System.Timers.Timer Timer;

        private LogsListener logsListener = new LogsListener();
        private readonly AppSettings settings;

        private string clientTxtPath = "";

        public ClientTxtReader(AppSettings settings)
        {
            this.settings = settings;
            this.clientTxtPath = settings.Main.ClientTxtPath;

            Messenger.Default.Register<ClientTxtPathChanged>(this, clientTxtPathChanged, true);
            Messenger.Default.Register<ClientTxtReaderUsageChanged>(this, clientTxtReaderUsageChanged, true);

            init();
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

        public void init()
        {
            Dispose();

            if (clientTxtPath != "" && File.Exists(clientTxtPath) && settings.Main.UseClientTxtReader)
            {
                FileStream = new FileStream(clientTxtPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader = new StreamReader(FileStream, Encoding.UTF8);
                while (StreamReader.ReadLine() != null) { }

                StartReading();
            }
        }

        public void StartReading()
        {
            Timer = new System.Timers.Timer(500);

            Timer.Elapsed += OnTimedEvent;
            Timer.AutoReset = true;
            Timer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (FileStream.Position < FileStream.Length)
            {
                string line;
                while ((line = StreamReader.ReadLine()) != null)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        //TmpLogger.WriteLine(line);
                        logsListener.Read(line);
                        //Messenger.Default.Send(new NewLogLineAdded(line));
                        //NewLineAdded?.Invoke(this, new NewLineEvent(line));
                    });
                }
            }
        }

        public void StopReading()
        {
            Timer?.Stop();
            Timer?.Dispose();
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
