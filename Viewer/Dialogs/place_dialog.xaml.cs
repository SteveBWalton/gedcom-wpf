using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
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
    /// Dialog to allow the user to enter / edit a gedcom place.
    /// </summary>
    public partial class PlaceDialog : Window
    {
        #region Member Variables

        /// <summary>The place string to use in the gedcom file.</summary>
        private string _tagPlace;

        /// <summary>True when the controls should not update the object or each other.</summary>
        private bool _isNoUpdate;
       
        #endregion

        #region Constructors

        public PlaceDialog()
        {
            _isNoUpdate = true;
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>The place string to use in the gedcom file.</summary>
        /// <remarks>This is not thread safe.</remarks>
        public string tagPlace
        {
            get => _tagPlace;
            set 
            {
                _tagPlace = value;

                // Decode the date value onto the dialog.
                decodeString();
            }
        }


        #endregion

        #region Encode Decode

        /// <summary>Decode the tagDate string to member variables.</summary>
        /// <returns>True for success, false otherwise.</returns>
        private bool decodeString()
        {
            string workingString = _tagPlace;



            // Return success.
            return true;
        }



        /// <summary>Transfer the values from the member variables to the dialog.</summary>
        /// <returns>True for success, false otherwise.</returns>
        private bool toDialog()
        {
            // No update from the dialog controls.
            _isNoUpdate = true;

            // Deal with the On From Between status.


            // Allow updates from the dialog controls.
            _isNoUpdate = false;

            // Return success.
            return true;
        }



        /// <summary>Transfer the values from the member variables to the dialog text box.</summary>
        /// <returns>True for success, false otherwise.</returns>
        private bool fromDialog()
        {
            // Build a new date string.
            StringBuilder newDate = new StringBuilder();

            // Return success.
            return true;
        }

        #endregion

        #region Signal Handlers

        /// <summary>Signal handler for the window loaded event.</summary>
        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            // Update the date text box.
            _txtTagPlace.Text = _tagPlace;

            // Update the dialog.
            toDialog();
        }



        /// <summary>Signal handler for the OK button click. </summary>
        private void buttonOkClick(object sender, RoutedEventArgs e)
        {
            // Update the tagDate property.
            _tagPlace = _txtTagPlace.Text;
            // Close the dialog with success.
            this.DialogResult = true;
        }



        /// <summary>Signal handler for the cancel button click.</summary>
        private void buttonCancelClick(object sender, RoutedEventArgs e)
        {

        }



        /// <summary>Signal handler for the date textbox losing the focus.</summary>
        private void txtTagDateLostFocus(object sender, RoutedEventArgs e)
        {
            // Decode the new string value.
            _tagPlace = _txtTagPlace.Text;
            decodeString();

            // Update the other controls on the dialog.
            toDialog();
        }

        #endregion



    }
}
