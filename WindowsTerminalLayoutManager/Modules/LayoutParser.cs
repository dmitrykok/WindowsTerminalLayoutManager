using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TerminalLayoutManager
{
    public class LayoutInfo
    {
        public List<string> DismissedMessages { get; set; }
        public List<string> GeneratedProfiles { get; set; }
        public List<PersistedWindowLayout> PersistedWindowLayouts { get; set; }
        public string SettingsHash { get; set; }
    }

    public class PersistedWindowLayout
    {
        public string InitialPosition { get; set; }
        public InitialSize InitialSize { get; set; }
        public string LaunchMode { get; set; }
        public List<TabLayout> TabLayout { get; set; }
    }

    public class InitialSize
    {
        public double Height { get; set; }
        public double Width { get; set; }
    }

    public class TabLayout
    {
        public string Action { get; set; }
        public string Commandline { get; set; }
        public string Profile { get; set; }
        public string StartingDirectory { get; set; }
        public bool SuppressApplicationTitle { get; set; }
        public string TabTitle { get; set; }
        public List<Pane> PaneLayout { get; set; } // Handle tab splits
    }

    public class Pane
    {
        public string Commandline { get; set; }
        public string Profile { get; set; }
        public string StartingDirectory { get; set; }
        public bool SuppressApplicationTitle { get; set; }
        public string TabTitle { get; set; }
    }

    public class LayoutParser
    {
        public static LayoutInfo ParseLayout(string jsonContent)
        {
            return JsonConvert.DeserializeObject<LayoutInfo>(jsonContent);
        }

        public static string GenerateLayoutDescription(LayoutInfo layoutInfo)
        {
            StringBuilder description = new StringBuilder();

            foreach (var windowLayout in layoutInfo.PersistedWindowLayouts)
            {
                description.AppendLine($"Window Position: {windowLayout.InitialPosition}");
                description.AppendLine($"Window Size: {windowLayout.InitialSize.Width}x{windowLayout.InitialSize.Height}");
                description.AppendLine($"Launch Mode: {windowLayout.LaunchMode}");
                description.AppendLine("Tabs:");

                foreach (var tab in windowLayout.TabLayout)
                {
                    description.AppendLine($"  - Tab Title: {tab.TabTitle}");
                    description.AppendLine($"    Profile: {tab.Profile}");
                    description.AppendLine($"    Command: {tab.Commandline}");
                    description.AppendLine($"    Starting Directory: {tab.StartingDirectory ?? "Default"}");
                    description.AppendLine($"    Suppress Application Title: {tab.SuppressApplicationTitle}");
                    description.AppendLine();

                    // Handle tab splits
                    if (tab.PaneLayout != null)
                    {
                        description.AppendLine($"    Splits:");
                        foreach (var pane in tab.PaneLayout)
                        {
                            description.AppendLine($"      - Tab Title: {pane.TabTitle}");
                            description.AppendLine($"        Profile: {pane.Profile}");
                            description.AppendLine($"        Command: {pane.Commandline}");
                            description.AppendLine($"        Starting Directory: {pane.StartingDirectory ?? "Default"}");
                            description.AppendLine($"        Suppress Application Title: {pane.SuppressApplicationTitle}");
                            description.AppendLine();
                        }
                    }
                }
            }

            return description.ToString();
        }
    }
}
