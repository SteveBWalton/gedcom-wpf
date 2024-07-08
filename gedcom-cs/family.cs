using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom
{
    /// <summary>Class to represent a family in a gedcom file.</summary>
    public class Family : TopLevel
    {
        #region Member Variables

        #endregion

        #region Class Constructors

        /// <summary>Empty class constructor.</summary>
        public Family()
        {
        }

        /// <summary>Create an individual from the specified tag.</summary>
        /// <param name="tag">Specifies the tag to build the individual from.</param>
        public Family(Tag tag) : base(tag)
        {
        }

        #endregion

        #region Properties

        public string fullName
        {
            get
            {
                return "Undefined";
            }
        }

        #endregion

    }
}
