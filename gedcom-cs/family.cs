using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom
{
    /// <summary>Class to represent a family in a gedcom file.</summary>
    public class Family : TopLevel, IComparable<Family>
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

        #region IComparable<Family>

        /// <summary>Impliment a compare function for sorting.</summary>
        /// <param name="otherIndividual">Specifies the individual to compare with.</param>
        /// <returns>The comparison of the last edit date of the two individuals.</returns>
        public int CompareTo(Family otherFamily)
        {
            return otherFamily.lastChanged.CompareTo(lastChanged);
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
