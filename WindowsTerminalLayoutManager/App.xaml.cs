﻿using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Windows;
using ModernWpf;
//using MahApps.Metro;

namespace WindowsTerminalLayoutManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            if (identity == null)
            {
                return false;
            }
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static void RestartAsAdministrator()
        {
            try
            {
                // Get the path to the executable file
                string? exeName = Environment.ProcessPath;

                if (exeName == null)
                {
                    MessageBox.Show("The application requires administrator privileges to run.\n" +
                                "Please restart the application and grant the required privileges.",
                                "Elevation Required",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                    return; 
                }

                var startInfo = new ProcessStartInfo(fileName: exeName)
                {
                    UseShellExecute = true,
                    Verb = "runas"
                };
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                // The user refused the elevation request or an error occurred
                MessageBox.Show("The application requires administrator privileges to run.\n" +
                                "Please restart the application and grant the required privileges.",
                                "Elevation Required",
                                MessageBoxButton.OK, MessageBoxImage.Warning);

                // Optionally log the exception or handle it as needed
                // For example: LogException(ex);
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!IsAdministrator())
            {
                // Attempt to restart the application with elevated privileges
                RestartAsAdministrator();
                // Shutdown the current instance
                Shutdown();
                return;
            }

            base.OnStartup(e);

            // Set the application theme to Dark
            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
        }
    }

}
