using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TerminalLayoutManager.Controls;
using TerminalLayoutManager.Services;
using TerminalLayoutManager.Utils;
using Path = System.IO.Path;

namespace TerminalLayoutManager
{
    public partial class MainWindow : Window
    {
        private readonly ITerminalService _terminalService;
        Dictionary<string, TerminalInfo>? _terminalDict;
        private int? TerminalLastSelectedIndex { get; set; }
        private string? TerminalLastSelectedName { get; set; }
        private TerminalInfo? SelectedTerminalInfo { get; set; }
        private int? LayoutInformationLastSelectedIndex { get; set; }
        private string? LayoutInformationLastSelectedFile { get; set; }
        private string? LayoutInformationLastSelectedFilePath { get; set; }
        private string? CurrentLayoutPath { get; set; }

        private readonly Regex _filenamePattern = StateFileNameRegex();

        private TextBox? EditingTextBox { get; set;}

        private bool ShowCustomDialog(string messageBoxText, string caption)
        {
            YesNoDialog customDialog = new(this, messageBoxText, caption)
            {
                Owner = this
            };
            customDialog.ShowDialog();  // ShowDialog makes it modal
            return customDialog.DialogResult == true;
        }

        public MainWindow()
        {
            InitializeComponent();
            _terminalService = new TerminalService();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateTerminalInstances();
        }

        private void PopulateTerminalInstances()
        {
            _terminalDict = _terminalService.FindAllTerminals();
            TerminalInstanceSelector.ItemsSource = _terminalDict.Select(kvp => new
            {
                ImageSource = kvp.Value.LogoAbsoluteUri,
                DisplayName = kvp.Key
            }).ToList();

            if (TerminalInstanceSelector.Items.Count > 0)
            {
                if (TerminalLastSelectedIndex == null)
                {
                    TerminalLastSelectedIndex = 0;
                    TerminalInstanceSelector.SelectedIndex = (int)TerminalLastSelectedIndex;
                }
                else
                {
                    PopulateTerminalInfo();
                }
            }
        }

        private void PopulateTerminalInfo() 
        {
            if (TerminalLastSelectedName != null && _terminalDict != null)
            {
                SelectedTerminalInfo = _terminalDict[TerminalLastSelectedName];
                // Use LINQ to select just the file name from each path
                // Filter out 'state.json', select file names, and convert to list
                List<EditableFileName> fileNames = SelectedTerminalInfo.LocalStateFiles
                    .Where(path => Path.GetFileName(path) != "state.json")
                    .Select(path => new EditableFileName(Path.GetFileName(path)))
                    .ToList();

                LayoutInformation.ItemsSource = fileNames;
            }
        }

        private void TerminalInstanceSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TerminalLastSelectedIndex = TerminalInstanceSelector.SelectedIndex;
            var selectedItem = TerminalInstanceSelector.SelectedItem;
            if (selectedItem != null)
            {
                // Using reflection to access the DisplayName property
                var displayNameProperty = selectedItem.GetType().GetProperty("DisplayName");
                if (displayNameProperty != null)
                {
                    TerminalLastSelectedName = displayNameProperty.GetValue(selectedItem) as string;
                    // Now you can use displayName as needed
                }
            }
 
            if (!string.IsNullOrEmpty(TerminalLastSelectedName) &&
                _terminalDict != null)
            {
                SelectedTerminalInfo = _terminalDict[TerminalLastSelectedName];
                CurrentLayoutPath = SelectedTerminalInfo.LocalStateFiles.FirstOrDefault(path => Path.GetFileName(path).Equals("state.json"));
            }
            PopulateTerminalInfo();
        }

        private void LayoutInformation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (EditableFileName item in e.RemovedItems)
            {
                if (item.IsEditing)
                {
                    item.IsEditing = false;
                    // Ensure the item's FileNameErrorIsOpen is set to false to close any open tooltips
                    item.FileNameErrorIsOpen = false;
                    item.FileNameError = string.Empty;
                    item.FileNameErrorVisibility = Visibility.Collapsed;
                }
            }

            LayoutInformationLastSelectedIndex = LayoutInformation.SelectedIndex;
            if (LayoutInformation.SelectedItem is EditableFileName dataItem)
            {
                LayoutInformationLastSelectedFile = dataItem.FileName;
                if (!string.IsNullOrEmpty(LayoutInformationLastSelectedFile) &&
                    !string.IsNullOrEmpty(TerminalLastSelectedName) &&
                    SelectedTerminalInfo != null)
                {
                    LayoutInformationLastSelectedFilePath = SelectedTerminalInfo.LocalStateFiles
                        .Where(fileName => Path.GetFileName(fileName) == LayoutInformationLastSelectedFile)
                        .ToList()[0];
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (textBox.DataContext is EditableFileName dataItem)
                {
                    if (!_filenamePattern.IsMatch(textBox.Text))
                    {
                        dataItem.FileNameError = "The filename must follow the pattern 'state_[name].json'.";
                        dataItem.FileNameErrorVisibility = Visibility.Visible;
                        dataItem.FileNameErrorIsOpen = true;
                        if (textBox.ToolTip is ToolTip toolTip)
                        {
                            toolTip.Content = "The filename must follow the pattern\n'state_[name].json'";
                            toolTip.IsOpen = true; // Manually open the tooltip
                            toolTip.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        dataItem.FileNameError = string.Empty;
                        dataItem.FileNameErrorVisibility = Visibility.Collapsed;
                        dataItem.FileNameErrorIsOpen = false;
                        if (textBox.ToolTip is ToolTip toolTip)
                        {
                            toolTip.Content = string.Empty;
                            toolTip.IsOpen = false; // Manually close the tooltip
                            toolTip.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
        }

        private void OnTextBoxLostFocus(object sender)
        {
            if (sender is TextBox textBox)
            {
                if (textBox.DataContext is EditableFileName dataItem)
                {
                    if (textBox.ToolTip is ToolTip toolTip)
                    {
                        toolTip.Content = string.Empty;
                        toolTip.IsOpen = false; // Manually close the tooltip
                        toolTip.Visibility = Visibility.Collapsed;
                    }

                    if (dataItem.IsEditing == false)
                    { 
                        return;
                    }

                    if (!_filenamePattern.IsMatch(textBox.Text))
                    {
                        textBox.Text = LayoutInformationLastSelectedFile;
                        dataItem.FileName = LayoutInformationLastSelectedFile;
                        dataItem.IsEditing = false;
                        return;
                    }

                    dataItem.IsEditing = false; // Exit edit mode

                    var directory = Path.GetDirectoryName(LayoutInformationLastSelectedFilePath);
                    var oldFilePath = Path.Combine(directory, LayoutInformationLastSelectedFile);
                    var newFilePath = Path.Combine(directory, dataItem.FileName);

                    // Update the collection
                    int index = SelectedTerminalInfo.LocalStateFiles.FindIndex(
                        fileName => Path.GetFileName(fileName) == LayoutInformationLastSelectedFile);
                    if (index != -1)
                    {
                        SelectedTerminalInfo.LocalStateFiles[index] = newFilePath;
                    }

                    File.Move(oldFilePath, newFilePath);
                }
            }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) 
            {
                OnTextBoxLostFocus(sender);
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                EditingTextBox = textBox;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            OnTextBoxLostFocus(sender);
        }


        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LayoutInformation.SelectedItem is EditableFileName dataItem)
            {
                // Toggle the editing state
                dataItem.IsEditing = true;
            }
        }


        private void DuplicateSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(LayoutInformationLastSelectedFilePath))
            {
                var directory = Path.GetDirectoryName(LayoutInformationLastSelectedFilePath);
                var newFileName = $"state_{DateTime.Now:yyyyMMddHHmmss}.json";
                var newFilePath = Path.Combine(directory, newFileName);

                File.Copy(LayoutInformationLastSelectedFilePath, newFilePath);
                PopulateTerminalInstances();  // Assume this method refreshes the ListView
            }
        }


        private void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(LayoutInformationLastSelectedFilePath) &&
                ShowCustomDialog($"Delete File:\n'{LayoutInformationLastSelectedFile}' ?",
                                 "Delete Selected Layout File"))
            {
                File.Delete(LayoutInformationLastSelectedFilePath);
                PopulateTerminalInstances();
            }
        }


        private void SaveCurrentButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(CurrentLayoutPath))
            {
                var directory = Path.GetDirectoryName(CurrentLayoutPath);
                var newFileName = $"state_{DateTime.Now:yyyyMMddHHmmss}.json";
                var newFilePath = Path.Combine(directory, newFileName);

                File.Copy(CurrentLayoutPath, newFilePath);
                PopulateTerminalInstances();
            }
        }


        private void LoadSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(LayoutInformationLastSelectedFilePath) &&
                !string.IsNullOrEmpty(CurrentLayoutPath) &&
                SelectedTerminalInfo != null)
            {
                File.Copy(LayoutInformationLastSelectedFilePath, CurrentLayoutPath, true);
                string terminalPath = Path.Combine(SelectedTerminalInfo.InstalledLocationPath, "WindowsTerminal.exe");
                var startInfo = new ProcessStartInfo
                {
                    FileName = terminalPath,
                    UseShellExecute = true,
                    Verb = RunAsAdmin.IsChecked == true ? "runas" : "",  
                };
                Process.Start(startInfo);  // Launch Windows Terminal
            }
        }

        [GeneratedRegex(@"^state_[a-zA-Z0-9_\-]+\.json$", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex StateFileNameRegex();
    }
}
