using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom
{
    /// <summary>Class to supply extra functionality of data tags.</summary>
    public class TagDate
    {
        #region Member Variables

        /// <summary>The actual date tag.</summary>
        private readonly Tag _tag;

        #endregion

        #region Class Constructors

        /// <summary>Creates a TagDate object from the specified Tag object.</summary>
        /// <param name="tag">Specifies the Tag object which is really expected to be of DATE type.</param>
        public TagDate(Tag tag)
        {
            _tag = tag;
        }



        /// <summary>Create an empty TagDate object which represents missing date information.</summary>
        public TagDate()
        {
            _tag = null;
        }

        #endregion

        /// <summary>A long description of the specified date.  This will usually start 'on'.</summary>
        /// <returns>A long description of the specifeid date.  This will usually start 'on'.</returns>
        public string getLongDate()
        {
            // Check for an empty / missing tag.
            if (_tag==null)
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
                html.Append("before ");
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
            html.Replace("ABT", "about");
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

            return html.ToString();
        }


    }
}
