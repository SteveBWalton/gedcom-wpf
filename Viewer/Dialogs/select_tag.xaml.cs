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
    /// <summary>Class to represent a dialog to select tag.</summary>
    public partial class DialogSelectTag : Window
    {
        #region Member Variables

        #endregion

        #region Constructor

        /// <summary>Constructor for the select tag dialog.</summary>
        public DialogSelectTag()
        {
            InitializeComponent();
        }

        #endregion

        #region Signal Handlers

        /// <summary>Signal handler for the cancel button click.</summary>
        private void buttonCancelClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }



        /// <summary>Signal handler for the Ok button click.</summary>
        private void buttonOkClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        #endregion
    }
}
