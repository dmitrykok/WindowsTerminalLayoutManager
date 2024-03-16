// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using Windows.Management.Deployment;

namespace TerminalLayoutManager.Services
{
    [Serializable]
    public class TerminalInfo(
    string terminalName,
    string familyName,
    string displayName,
    string logoAbsoluteUri,
    string installedLocationPath)
    {
        public string TerminalName { get; set; } = terminalName;
        public string FamilyName { get; set; } = familyName;
        public string DisplayName { get; set; } = displayName;
        public string LogoAbsoluteUri { get; set; } = logoAbsoluteUri;
        public string InstalledLocationPath { get; set; } = installedLocationPath;
        public List<string> LocalStateFiles { get; set; } = new List<string>();
        // public Process? Process { get; set; }
    }

    internal class Program
    {
        public static Dictionary<string, TerminalInfo> FindInstalledTerminals()
        {
            var terminals = new Dictionary<string, TerminalInfo>();
            var packageManager = new PackageManager();

            var packages = packageManager.FindPackages();

            foreach (var package in packages)
            {
                if (package.Id.FamilyName.Contains("WindowsTerminal"))
                {
                    terminals[package.DisplayName] = new TerminalInfo(
                        package.Id.FamilyName,
                        package.Id.FamilyName,
                        package.DisplayName,
                        package.Logo.AbsoluteUri,
                        package.InstalledLocation.Path);
                }
            }

            return terminals;
        }
        private static void Main(string[] args)
        {
            Dictionary<string, TerminalInfo> packages = FindInstalledTerminals();
            string jsonString = JsonSerializer.Serialize(packages);
            Console.WriteLine(jsonString);
        }
    }
}