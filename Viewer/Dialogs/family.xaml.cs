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
    /// <summary>Class to represent a dialog to edit a family object.</summary>
    public partial class DialogFamily : Window
    {
        #region Constructors

        /// <summary>Constructor for the edit family dialog to create a new family.</summary>
        /// <param name="gedcom">Specifies the gedcom to add the new family to.</param>
        public DialogFamily(Gedcom gedcom)
        {
            InitializeComponent();
        }

        /// <summary>Constructor for the edit family dialog to edit an existing family.</summary>
        /// <param name="gedcom">Specifies the gedcom that contains the family.</param>
        /// <param name="query">Specifies a query that contains an ID to identify the family.</param>
        public DialogFamily(Gedcom gedcom, string query) : this(gedcom)
        {

        }

        #endregion

        #region Signal Handlers

        /// <summary>Signal handler for the OK button click.</summary>
        private void buttonOkClick(object sender, RoutedEventArgs e)
        {
            // Close the dialog with okay.
            this.DialogResult = true;
        }



        /// <summary>Signal handler for the cancel button click.</summary>
        private void buttonCancelClick(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}
