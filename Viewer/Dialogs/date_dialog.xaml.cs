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

        /// <summary>Class to represent the encoded values of a single date on the dialog.</summary>
        private class DialogDate
        {
            #region Member Variables

            /// <summary>The on before after status of this date.</summary>
            public OnBeforeAfter onBeforeAfter;

            #endregion

            #region Constructors

            public DialogDate()
            {
                onBeforeAfter = OnBeforeAfter.ON;
            }

            public DialogDate(string workingString)
            {
                decodeString(workingString);
            }

            #endregion

            #region Encode Decode

            /// <summary>Convert the specified string into class member variables.</summary>
            /// <param name="workingString">Specifies the string that defines the date.</param>
            /// <returns>True for success, false otherwise.</returns>
            public bool decodeString(string workingString)
            {
                onBeforeAfter = OnBeforeAfter.ON;
                if (workingString.Contains("BEF"))
                {
                    onBeforeAfter = OnBeforeAfter.BEFORE;
                    workingString.Replace("BEF", "");
                }
                if (workingString.Contains("AFT"))
                {
                    onBeforeAfter = OnBeforeAfter.AFTER;
                    workingString.Replace("AFT", "");
                }

                // return success.
                return true;
            }



            /// <summary>Convert the class member variables into a string.</summary>
            /// <returns>The string that represents the class member variables.</returns>
            public override string ToString()
            {
                StringBuilder result = new StringBuilder();
                switch(onBeforeAfter)
                {
                case OnBeforeAfter.BEFORE:
                    result.Append("BEF ");
                    break;
                case OnBeforeAfter.AFTER:
                    result.Append("AFT ");
                    break;
                }

                return result.ToString();
            }

            #endregion
        }

        /// <summary>The date string to use in the gedcom file.</summary>
        private string _tagDate;

        /// <summary>True when the controls should not update the object or each other.</summary>
        private bool _isNoUpdate;

        /// <summary>The current 'On', 'From', 'Between' status of the dialog.</summary>
        private OnFromBetween _onFromBetween;

        private DialogDate _firstDate;
        private DialogDate _secondDate;
        
        #endregion

        #region Constructors

        public DateDialog()
        {
            _isNoUpdate = true;
            InitializeComponent();

            _firstDate = new DialogDate();
            _secondDate = new DialogDate();
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

                // Split the working string on "TO".
                int splitPos = workingString.IndexOf("TO");
                if (splitPos > 0)
                {
                    _firstDate.decodeString(workingString.Substring(0, splitPos));
                    _secondDate.decodeString(workingString.Substring(splitPos + 3));
                }
                else
                {
                    // This is not really correct.
                    _firstDate.decodeString(workingString);
                }
            }
            else if (workingString.Contains("BET"))
            {
                _onFromBetween = OnFromBetween.BETWEEN;
                workingString.Replace("BET", "");

                // Split the working string on "AND"
                int splitPos = workingString.IndexOf("AND");
                if (splitPos > 0)
                {
                    _firstDate.decodeString(workingString.Substring(0, splitPos));
                    _secondDate.decodeString(workingString.Substring(splitPos + 4));
                }
                else
                {
                    // This is not really correct.
                    _firstDate.decodeString(workingString);
                }
            }
            else
            {
                _firstDate.decodeString(workingString);
            }

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
            if (_onFromBetween == OnFromBetween.ON)
            {
                // Hide the second date.
                _secondDateOnBeforeAfter.Visibility = Visibility.Hidden;
                _secondDateGrid.Visibility = Visibility.Hidden;

                _radiobuttonOnFromBetween.IsChecked = true;

                updateDate(_firstDate, _radiobuttonFirstOn, _radiobuttonFirstBefore, _radiobuttonFirstAfter);
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

                updateDate(_firstDate, _radiobuttonFirstOn, _radiobuttonFirstBefore, _radiobuttonFirstAfter);
                updateDate(_secondDate, _radiobuttonSecondOn, _radiobuttonSecondBefore, _radiobuttonSecondAfter);
            }

            // Allow updates from the dialog controls.
            _isNoUpdate = false;

            // Return success.
            return true;
        }



        /// <summary>Update the specified dialog controls with the specified date settings.</summary>
        /// <param name="dialogDate">Specifies the date settings.</param>
        /// <param name="radiobuttonOn"></param>
        /// <param name="radiobuttonBefore"></param>
        /// <param name="radiobuttonAfter"></param>
        /// <returns></returns>
        private bool updateDate(DialogDate dialogDate, RadioButton radiobuttonOn, RadioButton radiobuttonBefore, RadioButton radiobuttonAfter)
        {
            switch (dialogDate.onBeforeAfter)
            {
            case OnBeforeAfter.ON:
                radiobuttonOn.IsChecked = true;
                break;
            case OnBeforeAfter.BEFORE:
                radiobuttonBefore.IsChecked = true;
                break;
            case OnBeforeAfter.AFTER:
                radiobuttonAfter.IsChecked = true;
                break;
            }

            // Return success.
            return true;
        }

        #endregion

        #region Signal Handlers

        /// <summary>Signal handler for the window loaded event.</summary>
        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            // Update the date text box.
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



        /// <summary>Signal handler for the date textbox losing the focus.</summary>
        private void txtTagDateLostFocus(object sender, RoutedEventArgs e)
        {
            // Decode the new string value.
            _tagDate = _txtTagDate.Text;
            decodeString();

            // Update the other controls on the dialog.
            toDialog();
        }

        #endregion

    }
}
