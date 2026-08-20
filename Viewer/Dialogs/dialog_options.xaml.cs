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
        #region Member Variables

        /// <summary>The user options to show on the dialog.</summary>
        private UserOptions _userOptions;

        #endregion

        #region Constructors

        /// <summary>Constructor for the options dialog.</summary>
        /// <param name="userOptions">Specifies the user options to display.</param>
        /// <param name="ownerWindow">Specifies the window that owns this dialog.</param>
        public DialogOptions(UserOptions userOptions, Window ownerWindow)
        {
            InitializeComponent();

            // Save the parameters.
            _userOptions = userOptions;
            Owner = ownerWindow;
        }

        #endregion

        #region Signal Handlers

        /// <summary>The signal handler for the window loaded event.</summary>
        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            // Load the user options values onto the dialog.
            _chkShowGedcom.IsChecked = _userOptions.isShowGedcom;
        }



        /// <summary>Signal handler for the OK button click.</summary>
        private void buttonOkClick(object sender, RoutedEventArgs e)
        {
            // Load the user options from the dialog.
            _userOptions.isShowGedcom = _chkShowGedcom.IsChecked == true;

            // Save the user options.
            _userOptions.saveSettings();

            // Close the dialog with okay.
            this.DialogResult = true;
        }

        #endregion

    }
}
