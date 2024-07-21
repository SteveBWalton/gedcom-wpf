using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom
{
    /// <summary>Class to represent a source in a gedcom file.</summary>
    public class Source : TopLevel, IComparable<Source>
    {
        #region Member Variables

        #endregion

        #region Class Constructors

        /// <summary>Empty class constructor.</summary>
        public Source()
        {
        }

        /// <summary>Create an source from the specified tag.</summary>
        /// <param name="tag">Specifies the tag to build the source from.</param>
        /// <param name="gedcom">Specifies the gedcom that contains this top level element.</param>
        public Source(Tag tag, Gedcom gedcom) : base(tag, gedcom)
        {
        }

        #endregion

        #region IComparable<Source>

        /// <summary>Impliment a compare function for sorting.</summary>
        /// <param name="otherSource">Specifies the source to compare with.</param>
        /// <returns>The comparison of the last edit date of the two sources.</returns>
        public int CompareTo(Source otherSource)
        {
            return otherSource.lastChanged.CompareTo(lastChanged);
        }

        #endregion

        #region Properties
        
        /// <summary>The name of the source.</summary>
        public string fullName
        {
            get
            {
                Tag tagName = _tag.children.findOne("TITL");
                if (tagName == null)
                {
                    return "Error";
                }

                // Return the name of the source.
                return tagName.value;
            }
        }

        #endregion

    }
}
