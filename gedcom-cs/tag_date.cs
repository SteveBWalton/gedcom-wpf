using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom
{
    /// <summary>Class to supply extra functionality for 'DATE' tags.</summary>
    public class TagDate
    {
        #region Member Variables

        /// <summary>The actual date tag.</summary>
        private readonly Tag _tag;

        /// <summary>An approximate date time object for sorting etc.</summary>
        private readonly DateTime _dateTime;

        #endregion

        #region Class Constructors

        /// <summary>Creates a TagDate object from the specified Tag object.</summary>
        /// <param name="tag">Specifies the Tag object which is really expected to be of DATE type.</param>
        public TagDate(Tag tag)
        {
            // Record the actual tag.
            _tag = tag;

            // Try to get the approximate month.
            int month;
            int monthPos;
            getMonth(out month, out monthPos);
            if (monthPos == -1)
            {
                // No month information.
                string yearString = getDigits(0, 1);
                int year;
                if (!int.TryParse(yearString, out year))
                {
                    year = 1990;
                }
                _dateTime = new DateTime(year, 1, 1);
            }
            else
            {
                // Search forward for year information and backwards for day information.
                string yearString = getDigits(monthPos, 1);
                string dayString = getDigits(monthPos, -1);
                // Console.WriteLine("'" + _tag.value + "' => '" + dayString + "', " + month.ToString() + ", '" + yearString + "'");
                int year;
                if (!int.TryParse(yearString,out year))
                {
                    year = 1990;
                }
                int day;
                if(!int.TryParse(dayString,out day))
                {
                    day = 1;
                }
                _dateTime = new DateTime(year, month, day);
            }
            // Console.WriteLine("'" + _tag.value + "' => " + _dateTime.ToString());
        }

        #endregion

        #region Properties

        /// <summary>The actual tag that created the TagDate object.</summary>
        public Tag tag
        {
            get { return _tag; }
        }



        /// <summary>The approximate DateTime for this TagDate.</summary>
        public DateTime approxDate
        {
            get { return _dateTime; }
        }



        /// <summary>Returns the year in the date.</summary>
        public string yearDisplay
        {
            get
            {
                if (_dateTime == null)
                {
                    return null;
                }
                if (_tag.value.Contains("ABT"))
                {
                    return "(" + _dateTime.Year.ToString() + ")";
                }
                return _dateTime.Year.ToString();
            }
        }

        #endregion

        #region Generate DateTime object.

        /// <summary>Returns a value to use for the month and the position of this month information in the tag.</summary>
        /// <param name="month">Returns the value to use for the month.</param>
        /// <param name="monthPos">Returns the position in the tag where this month value from taken from.</param>
        private void getMonth(out int month, out int monthPos)
        {
            const int MISSING = 9999;

            // Default to no month information.
            month = 1;
            monthPos = -1;

            // Search for a standard month name.
            int firstMonthPos = MISSING;
            int firstMonthValue = MISSING;
            int pos;
            month = 1;
            foreach (string monthName in new string[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" })
            {
                if ((pos = _tag.value.ToUpper().IndexOf(monthName)) > 0)
                {
                    if (pos < firstMonthPos)
                    {
                        firstMonthPos = pos;
                        firstMonthValue = month;
                    }                    
                }
                month++;
            }

            // Search for other month type information.
            // Might find an earlier month marker in the string.

            // Return the first piece of month information.
            if (firstMonthPos != MISSING)
            {
                month = firstMonthValue;
                monthPos = firstMonthPos;
            }
        }


        /// <summary>Search fowards or backwards in the tag value string for digits.</summary>
        /// <param name="position">Specifies the position to search from.</param>
        /// <param name="direction">Specifies the search direction.</param>
        /// <returns></returns>
        private string getDigits(int position, int direction)
        {
            // Find the starting position.
            int startPosition = position;
            while (!Char.IsDigit(_tag.value[startPosition]))
            {
                startPosition += direction;
                if (startPosition < 0)
                {
                    startPosition = 0;
                    break;
                }
                if (startPosition >= _tag.value.Length)
                {
                    startPosition = _tag.value.Length - 1;
                    break;
                }
            }

            // Find the ending position.
            int endPosition = startPosition;
            while (Char.IsDigit(_tag.value[endPosition]))
            {
                endPosition += direction;
                if (endPosition < 0)
                {
                    endPosition = 0;
                    break;
                }
                if (endPosition >= _tag.value.Length)
                {
                    endPosition = _tag.value.Length - 1;
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
            return _tag.value.Substring(startPosition, endPosition - startPosition + 1);
        }

        #endregion

        /// <summary>A long description of the specified date.  This will usually start 'on'.</summary>
        /// <returns>A long description of the specifeid date.  This will usually start 'on'.</returns>
        public string getLongDate()
        {
            // Check for an empty / missing tag.
            if (_tag == null)
            {
                return "";
            }

            // Build a long description of the date tag.
            StringBuilder html = new StringBuilder();

            // Examine the date value.
            string dateValue = _tag.value;

            if (dateValue.Contains("BEF"))
            {
                // Before date.
                dateValue = dateValue.Replace("BEF", "");
                html.Append("before ");
            }
            if (dateValue.Contains("AFT"))
            {
                // After date.
                dateValue = dateValue.Replace("AFT", "");
                html.Append("after ");
            }
            else if (dateValue.Contains("BET"))
            {
                // Between date.
                dateValue = dateValue.Replace("BET", "");
                html.Append("between ");
            }
            else if (dateValue.Contains("FROM"))
            {
                // Between date.
                dateValue = dateValue.Replace("FROM", "");
                html.Append("from ");
            }
            else
            {
                // Default date.
                html.Append("on ");
            }

            // Add the unformated date information.
            html.Append(dateValue);

            // Dealt with about, and.
            html.Replace("ABT", "about");
            html.Replace("AND", "and");
            html.Replace("TO", "to");

            // Dealt with months.
            html.Replace("JAN", "January");
            html.Replace("FEB", "February");
            html.Replace("MAR", "March");
            html.Replace("APR", "April");
            html.Replace("MAY", "May");
            html.Replace("JUN", "June");
            html.Replace("JUL", "July");
            html.Replace("AUG", "August");
            html.Replace("SEP", "September");
            html.Replace("OCT", "October");
            html.Replace("NOV", "November");
            html.Replace("DEC", "December");

            return html.ToString();
        }


        /// <summary>A short description of the specified date.</summary>
        /// <returns>A short description of the specifeid date.</returns>

        public string getShortDate()
        {
            // Check for an empty / missing tag.
            if (_tag == null)
            {
                return "";
            }

            // Build a long description of the date tag.
            StringBuilder html = new StringBuilder();

            // Examine the date value.
            string dateValue = _tag.value;

            if (dateValue.Contains("BEF"))
            {
                // Before date.
                dateValue = dateValue.Replace("BEF", "");
                html.Append("< ");
            }
            if (dateValue.Contains("AFT"))
            {
                // After date.
                dateValue = dateValue.Replace("AFT", "");
                html.Append("> ");
            }
            else if (dateValue.Contains("BET"))
            {
                // Between date.
                dateValue = dateValue.Replace("BET", "");
                html.Append("between ");
            }
            else if (dateValue.Contains("FROM"))
            {
                // Between date.
                dateValue = dateValue.Replace("FROM", "");
                html.Append("from ");
            }
            else
            {
                // Default date.
                // html.Append("on ");
            }

            // Add the unformated date information.
            html.Append(dateValue);

            // Dealt with about, and.
            bool isAboutBracket = false;
            if (dateValue.Contains("ABT"))
            {
                html.Replace("ABT ", "(");
                html.Replace("ABT", "(");
                isAboutBracket = true;
            }
            html.Replace("AND", "and");
            html.Replace("TO", "to");

            // Dealt with months.
            html.Replace("JAN", "Jan");
            html.Replace("FEB", "Feb");
            html.Replace("MAR", "Mar");
            html.Replace("APR", "Apr");
            html.Replace("MAY", "May");
            html.Replace("JUN", "Jun");
            html.Replace("JUL", "Jul");
            html.Replace("AUG", "Aug");
            html.Replace("SEP", "Sep");
            html.Replace("OCT", "Oct");
            html.Replace("NOV", "Nov");
            html.Replace("DEC", "Dec");

            if(isAboutBracket)
            {
                html.Append(")");
            }

            return html.ToString();
        }


    }
}
