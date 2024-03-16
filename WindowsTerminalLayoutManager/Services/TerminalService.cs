using System.IO;
using System.Diagnostics;
using System.Text.Json;

namespace TerminalLayoutManager.Services
{
    public partial class TerminalService : ITerminalService
    {
        private static readonly string StateFileName = "state.json";
        private static readonly string LocalAppDataPackages = @"%LOCALAPPDATA%\Packages";
        // private static readonly Regex terminalPattern = WTRegex();

        public static Dictionary<string, TerminalInfo> FindInstalledTerminals()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "TerminalPackages.exe",
                UseShellExecute = false,
                Verb = "runas",  // Request elevated privileges
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };

            string receivedJson = string.Empty;
            using (var process = Process.Start(startInfo))
            {
                while (!process.StandardOutput.EndOfStream)
                {
                    receivedJson += process.StandardOutput.ReadLine();
                }
            }
            var receivedDict = JsonSerializer.Deserialize<Dictionary<string, TerminalInfo>>(receivedJson);
            return receivedDict;
        }

        // Method to find all Windows Terminal instances and their state.json locations
        public Dictionary<string, TerminalInfo> FindAllTerminals()
        {
            Dictionary<string, TerminalInfo> packages = FindInstalledTerminals();
            var appDataPackagesPath = Environment.ExpandEnvironmentVariables(LocalAppDataPackages);

            if (Directory.Exists(appDataPackagesPath))
            {
                foreach (var dirPath in Directory.EnumerateDirectories(appDataPackagesPath))
                {
                    var dirName = Path.GetFileName(dirPath);
                    var terminalInfo = packages.Select(p => p.Value).FirstOrDefault(p => p.FamilyName == dirName);
                    if (terminalInfo != null)
                    {
                        var localStatePath = Path.Combine(dirPath, "LocalState");

                        if (Directory.Exists(localStatePath))
                        {
                            foreach (var filePath in Directory.EnumerateFiles(localStatePath, "state*.json"))
                            {
                                terminalInfo.LocalStateFiles.Add(filePath);
                            }
                        }
                    }
                }
            }

            return packages;
        }


        // Method to save the current layout to a file
        public void SaveCurrentLayout(string terminalFolderPath, string savePath)
        {
            string sourcePath = Path.Combine(terminalFolderPath, StateFileName);
            File.Copy(sourcePath, savePath, true);
        }

        // Method to load a layout from a file
        public void LoadLayout(string savePath, string terminalFolderPath)
        {
            string destinationPath = Path.Combine(terminalFolderPath, StateFileName);
            File.Copy(savePath, destinationPath, true);

            // Launch the terminal after updating its layout
            Process.Start("wt.exe");
        }

        /*[GeneratedRegex(@"WindowsTerminal(?:|Canary|Preview|Dev)_")]
        private static partial Regex WTRegex();*/
    }
}
