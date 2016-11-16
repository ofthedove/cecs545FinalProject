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

namespace BoardBuilder
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class DialogOkCancel : Window
    {
        public DialogOkCancel(string message, string title)
        {
            InitializeComponent();

            Title = title;
            messageLabel.Text = message;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
