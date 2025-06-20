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

        /// <summary>Type to represent the On, Before, After.</summary>
        private enum OnFromBetween
        {
            ON,
            FROM,
            BETWEEN
        }

        /// <summary>Type to represent the On, Before, After.</summary>
        private enum OnBeforeAfter
        {
            ON,
            BEFORE,
            AFTER
        }

        /// <summary>The date string to use in the gedcom file.</summary>
        private string _tagDate;

        /// <summary>True when the controls should not update the object or each other.</summary>
        private bool _isNoUpdate;

        /// <summary>The current 'On', 'From', 'Between' status of the dialog.</summary>
        private OnFromBetween _onFromBetween;

        #endregion


        #region Constructors

        public DateDialog()
        {
            _isNoUpdate = true;
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>The date string to use in the gedcom file.</summary>
        /// <remarks>This is not thread safe.</remarks>
        public string tagDate
        {
            get => _tagDate;
            set 
            {
                _tagDate = value;

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
            string workingString = _tagDate;

            _onFromBetween = OnFromBetween.ON;
            if (workingString.Contains("FROM"))
            {
                _onFromBetween = OnFromBetween.FROM;
                workingString.Replace("FROM", "");
            }
            if (workingString.Contains("BETWEEN"))
            {
                _onFromBetween = OnFromBetween.BETWEEN;
                workingString.Replace("BETWEEN", "");
            }

            // Return success.
            return true;
        }



        /// <summary>Transfter the values from the member variables to the dialog.</summary>
        /// <returns>True for success, false otherwise.</returns>
        private bool toDialog()
        {
            // No update from the dialog controls.
            _isNoUpdate = true;

            // Deal with the On From Between status.
            if (_onFromBetween == OnFromBetween.ON)
            {
                // Hide the second date.
                _secondDateOnBeforeAfter.Visibility = Visibility.Hidden;
                _secondDateGrid.Visibility = Visibility.Hidden;

                _radiobuttonOnFromBetween.IsChecked = true;
            }
            else
            {
                // Show the second date.
                _secondDateOnBeforeAfter.Visibility = Visibility.Visible;
                _secondDateGrid.Visibility = Visibility.Visible;

                if (_onFromBetween == OnFromBetween.FROM)
                {
                    _radiobuttonFromOnBetween.IsChecked = true;
                }
                else if (_onFromBetween == OnFromBetween.BETWEEN)
                {
                    _radiobuttonBetweenOnFrom.IsChecked = true;
                }
            }

            // Allow updates from the dialog controls.
            _isNoUpdate = false;

            // Return success.
            return true;
        }

        #endregion

        #region Signal Handlers

        /// <summary>Signal handler for the window loaded event.</summary>
        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            _txtTagDate.Text = _tagDate;

            // Update the dialog.
            toDialog();
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



        /// <summary>Signal handler for the 'On' in the 'On', 'From', 'Between' group is checked.</summary>
        private void radiobuttonOnFromBetweenChecked(object sender, RoutedEventArgs e)
        {
            // Check for updates allowed.
            if (_isNoUpdate)
            {
                return;
            }

            _onFromBetween = OnFromBetween.ON;

            // Hide the second date.
            _secondDateOnBeforeAfter.Visibility = Visibility.Hidden;
            _secondDateGrid.Visibility = Visibility.Hidden;
        }



        /// <summary>Signal handler for the 'From' in the 'On', 'From', 'Between' group is checked.</summary>
        private void radiobuttonFromOnBetweenChecked(object sender, RoutedEventArgs e)
        {
            // Check for updates allowed.
            if (_isNoUpdate)
            {
                return;
            }

            _onFromBetween = OnFromBetween.FROM;

            // Show the second date.
            _secondDateOnBeforeAfter.Visibility = Visibility.Visible;
            _secondDateGrid.Visibility = Visibility.Visible;
        }



        /// <summary>Signal handler for the 'Between' in the 'On', 'From', 'Between' group is checked.</summary>
        private void radiobuttonBetweenOnFromChecked(object sender, RoutedEventArgs e)
        {
            // Check for updates allowed.
            if (_isNoUpdate)
            {
                return;
            }

            _onFromBetween = OnFromBetween.BETWEEN;

            // Show the second date.
            _secondDateOnBeforeAfter.Visibility = Visibility.Visible;
            _secondDateGrid.Visibility = Visibility.Visible;
        }

        #endregion

    }
}
