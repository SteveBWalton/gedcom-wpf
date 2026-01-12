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

namespace gedcom.viewer
{
    /// <summary>Class to represent the edit source dialog.</summary>
    public partial class DialogSource : Window
    {
        /// <summary>Default constructor for the edit source dialog.</summary>
        public DialogSource()
        {
            InitializeComponent();
        }

        private void buttonOkClick(object sender, RoutedEventArgs e)
        {
            // Close the dialog with okay.
            this.DialogResult = true;
        }

        private void buttonCancelClick(object sender, RoutedEventArgs e)
        {

        }

        private void addTagButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
