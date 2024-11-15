

using Krankenschwester.Utils;
using System;
using System.Windows;
using System.Windows.Threading;

namespace Krankenschwester
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Handle UI thread exceptions
            this.DispatcherUnhandledException += OnDispatcherUnhandledException;

            // Handle non-UI thread exceptions
            AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;

            // Handle task-based exceptions
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            HandleCrash();

            // Log the exception or show a message to the user
            System.Windows.Forms.MessageBox.Show($"An unhandled UI exception occurred: {e.Exception.Message}");
            TmpLogger.WriteLine($"An unhandled UI exception occurred: {e.Exception.Message}");
           // e.Handled = true; // Prevents the application from crashing
        }

        private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleCrash();

            // Log the exception or show a message to the user
            var exception = e.ExceptionObject as Exception;
            System.Windows.Forms.MessageBox.Show($"A non-UI thread exception occurred: {exception?.Message}");
            TmpLogger.WriteLine($"A non-UI thread exception occurred: {exception?.Message}");
            // Note: App may still crash depending on the severity of the exception
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            HandleCrash();

            // Log the exception or show a message to the user
            System.Windows.Forms.MessageBox.Show($"An unobserved task exception occurred: {e.Exception.Message}");
            TmpLogger.WriteLine($"An unobserved task exception occurred: {e.Exception.Message}");
            //e.SetObserved(); // Prevents the application from crashing due to unobserved task exceptions
        }

        private void HandleCrash()
        {
            MouseHook.Stop(); // Ensure hooks are cleaned up
            Console.WriteLine("App crashed. Cleanup completed.");
        }
    }

}
