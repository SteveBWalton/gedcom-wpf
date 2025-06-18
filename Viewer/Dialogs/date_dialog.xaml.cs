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
    /// <summary>
    /// Dialog to allow the user to enter / edit a gedcom date.
    /// </summary>
    public partial class DateDialog : Window
    {
        #region Member Variables

        /// <summary>The date string to use in the gedcom file.</summary>
        private string _tagDate;

        #endregion


        #region Constructors

        public DateDialog()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>The date string to use in the gedcom file.</summary>
        public string tagDate
        {
            get => _tagDate;
            set { _tagDate = value; }
        }


        #endregion

        #region Signal Handlers

        /// <summary>Signal handler for the window loaded event.</summary>
        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            _txtTagDate.Text = _tagDate;
        }



        /// <summary>Signal handler for the OK button click. </summary>
        private void buttonOkClick(object sender, RoutedEventArgs e)
        {
            // Update the tagDate property.
            _tagDate = _txtTagDate.Text;
            // Close the dialog with success.
            this.DialogResult = true;
        }



        /// <summary>Signal handler for the cancel button click.</summary>
        private void buttonCancelClick(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}
