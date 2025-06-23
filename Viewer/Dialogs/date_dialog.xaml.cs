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
        /// <remarks>Should this class get it's own file, possibly in gedcom-cs?</remarks>
        private class DialogDate
        {
            #region Member Variables

            /// <summary>The on before after status of this date.</summary>
            public OnBeforeAfter onBeforeAfter;

            /// <summary>True if the date only about, false otherwise.</summary>
            public bool isAbout;

            /// <summary>The day of the month.</summary>
            public int day;
            /// <summary>The month of the year, 1 based.</summary>
            public int month;
            /// <summary>The 4 digit year.</summary>
            public int year;

            /// <summary>True if the day of the month is unknown, false if known.</summary>
            public bool isDayUnknown;
            /// <summary>True if the month of the year is unknown, false if known.</summary>
            public bool isMonthUnknown;
            /// <summary>True if the 4 digit year is unknown, false if known.</summary>
            public bool isYearUnknown;

            #endregion

            #region Constructors

            /// <summary>The empty default constructor.</summary>
            public DialogDate()
            {
                onBeforeAfter = OnBeforeAfter.ON;
                isAbout = false;
                isDayUnknown = false;
                isMonthUnknown = false;
                isYearUnknown = false;
                day = 1;
                month = 1;
                year = 2024;
            }



            /// <summary>The constructor to take initial values from a string.</summary>
            /// <param name="workingString">Specifies the string that contains the initial values.</param>
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
                // On Before After.
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

                // Optional About.
                if (workingString.Contains("ABT"))
                {
                    isAbout = true;
                }
                else
                {
                    isAbout = false;
                }

                // Try to get the month.
                int monthPos = 9999;
                int monthIdx = 1;
                foreach (string monthName in new string[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" })
                {
                    int pos;
                    if ((pos = workingString.ToUpper().IndexOf(monthName)) >= 0)
                    {
                        if (pos < monthPos)
                        {
                            monthPos = pos;
                            month = monthIdx;
                        }
                    }
                    monthIdx++;
                }

                // Try to get the day and year from around the month.
                if (monthPos == 9999)
                {
                    // No month information.
                    isMonthUnknown = true;
                    isDayUnknown = true;
                    string yearString = getDigits(workingString, 0, 1);
                    isYearUnknown = false;
                    if (!int.TryParse(yearString, out year))
                    {
                        year = 1990;
                        isYearUnknown = true;
                    }
                }
                else
                {
                    isMonthUnknown = false;
                    // Search forward for year information and backwards for day information.
                    string yearString = getDigits(workingString, monthPos, 1);
                    string dayString = getDigits(workingString, monthPos, -1);
                    // Console.WriteLine("'" + _tag.value + "' => '" + dayString + "', " + month.ToString() + ", '" + yearString + "'");
                    isYearUnknown = false;
                    if (!int.TryParse(yearString, out year))
                    {
                        year = 1990;
                        isYearUnknown = true;
                    }
                    isDayUnknown = false;
                    if (!int.TryParse(dayString, out day))
                    {
                        day = 1;
                        isDayUnknown = true;
                    }
                }

                // return success.
                return true;
            }

            /// <summary>Search fowards or backwards in the tag value string for digits.</summary>
            /// <param name="position">Specifies the position to search from.</param>
            /// <param name="direction">Specifies the search direction.</param>
            /// <returns></returns>
            private string getDigits(string workingString, int position, int direction)
            {
                // Find the starting position.
                int startPosition = position;
                while (!Char.IsDigit(workingString[startPosition]))
                {
                    startPosition += direction;
                    if (startPosition < 0)
                    {
                        startPosition = 0;
                        break;
                    }
                    if (startPosition >= workingString.Length)
                    {
                        startPosition = workingString.Length - 1;
                        break;
                    }
                }

                // Find the ending position.
                int endPosition = startPosition;
                while (Char.IsDigit(workingString[endPosition]))
                {
                    endPosition += direction;
                    if (endPosition < 0)
                    {
                        endPosition = 0;
                        break;
                    }
                    if (endPosition >= workingString.Length)
                    {
                        endPosition = workingString.Length - 1;
                        break;
                    }
                }

                // Switch start and end into assending order.
                if (startPosition > endPosition)
                {
                    int swap = startPosition;
                    startPosition = endPosition;
                    endPosition = swap;
                }

                // Return the sub string identified.
                return workingString.Substring(startPosition, endPosition - startPosition + 1);
            }

            /// <summary>Convert the class member variables into a string.</summary>
            /// <returns>The string that represents the class member variables.</returns>
            public override string ToString()
            {
                StringBuilder result = new StringBuilder();
                switch (onBeforeAfter)
                {
                case OnBeforeAfter.BEFORE:
                    result.Append("BEF ");
                    break;
                case OnBeforeAfter.AFTER:
                    result.Append("AFT ");
                    break;
                }

                if (isAbout)
                {
                    result.Append("ABT ");
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

                updateDate(_firstDate, _radiobuttonFirstOn, _radiobuttonFirstBefore, _radiobuttonFirstAfter, _checkboxFirstAbout, _txtFirstDay, _cboFirstMonth, _txtFirstYear, _chkFirstDayUnknown, _chkFirstMonthUnknown, _chkFirstYearUnknown);
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

                updateDate(_firstDate, _radiobuttonFirstOn, _radiobuttonFirstBefore, _radiobuttonFirstAfter, _checkboxFirstAbout, _txtFirstDay, _cboFirstMonth, _txtFirstYear, _chkFirstDayUnknown, _chkFirstMonthUnknown, _chkFirstYearUnknown);
                updateDate(_secondDate, _radiobuttonSecondOn, _radiobuttonSecondBefore, _radiobuttonSecondAfter, _checkboxSecondAbout, _txtSecondDay, _cboSecondMonth, _txtSecondYear, _chkSecondDayUnknown, _chkSecondMonthUnknown, _chkSecondYearUnknown);
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
        private bool updateDate(DialogDate dialogDate, RadioButton radiobuttonOn, RadioButton radiobuttonBefore, RadioButton radiobuttonAfter, CheckBox checkboxAbout, TextBox txtDay, ComboBox cboMonth, TextBox txtYear, CheckBox chkDayUnknown, CheckBox chkMonthUnknown, CheckBox chkYearUnknown)
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

            checkboxAbout.IsChecked = dialogDate.isAbout;

            txtDay.Text = dialogDate.day.ToString();
            txtYear.Text = dialogDate.year.ToString();
            cboMonth.SelectedIndex = dialogDate.month;

            chkDayUnknown.IsChecked = dialogDate.isDayUnknown;
            chkMonthUnknown.IsChecked = dialogDate.isMonthUnknown;
            chkYearUnknown.IsChecked = dialogDate.isYearUnknown;

            txtDay.Visibility = dialogDate.isDayUnknown ? Visibility.Hidden : Visibility.Visible;
            cboMonth.Visibility = dialogDate.isMonthUnknown ? Visibility.Hidden : Visibility.Visible;
            txtYear.Visibility = dialogDate.isYearUnknown ? Visibility.Hidden : Visibility.Visible;

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
