using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TerminalLayoutManager.Controls
{
    /// <summary>
    /// Interaction logic for YesNoDialog.xaml
    /// </summary>
    public partial class YesNoDialog : Window
    {
        public YesNoDialog(Window owner, string messageBoxText, string caption)
        {
            InitializeComponent();
            this.Owner = owner;
            this.Title = caption;
            this.DialogMessage.Text = messageBoxText;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            // Handle the click event for the OK button. For example, close the dialog.
            this.DialogResult = true; // Sets the dialog result and closes the window
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            // Handle the click event for the OK button. For example, close the dialog.
            this.DialogResult = false; // Sets the dialog result and closes the window
        }
    }
}
