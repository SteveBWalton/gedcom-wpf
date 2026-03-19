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
    /// <summary>Class to represent the options dialog.</summary>
    public partial class DialogOptions : Window
    {
        #region Constructors

        /// <summary>Constructor for the options dialog.</summary>
        public DialogOptions()
        {
            InitializeComponent();
        }

        #endregion

        #region Signal Handlers

        /// <summary>Signal handler for the OK button click.</summary>
        private void buttonOkClick(object sender, RoutedEventArgs e)
        {
            // Close the dialog with okay.
            this.DialogResult = true;
        }

        #endregion
    }
}
