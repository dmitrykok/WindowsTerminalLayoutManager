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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set the application theme to Dark
            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
        }
    }

}
