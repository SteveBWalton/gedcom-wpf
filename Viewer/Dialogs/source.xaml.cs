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
        #region Constructors

        /// <summary>Default constructor for the edit source dialog.</summary>
        public DialogSource()
        {
            InitializeComponent();
        }

        #endregion

        #region Signal Handlers

        /// <summary>Signal handler for the OK button click event.</summary>
        private void buttonOkClick(object sender, RoutedEventArgs e)
        {
            // Close the dialog with okay.
            this.DialogResult = true;
        }



        /// <summary>Signal handler for the cancel button click.</summary>
        private void buttonCancelClick(object sender, RoutedEventArgs e)
        {

        }



        /// <summary>Signal handler for the add tag button click.</summary>
        private void addTagButtonClick(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}
