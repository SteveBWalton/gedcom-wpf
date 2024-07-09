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
        /// <param name="gedcom">Specifies the gedcom that contains this top level element.</param>
        public Family(Tag tag, Gedcom gedcom) : base(tag, gedcom)
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
                string name = "";
                Tag tagHusband = _tag.children.findOne("HUSB");
                if (tagHusband != null)
                {
                    Individual husband = _gedcom.individuals.find(Tag.toIdx(tagHusband.value));
                    if (husband != null)
                    {
                        name = husband.fullName;
                    }
                }

                Tag tagWife = _tag.children.findOne("WIFE");
                if (tagWife != null)
                {
                    Individual wife = _gedcom.individuals.find(Tag.toIdx(tagWife.value));
                    if (wife != null)
                    {
                        if (name != "")
                        {
                            name += " and ";
                        }
                        name += wife.fullName;
                    }
                }
                if (name == "")
                {
                    return "Undefined";
                }
                return name;
            }
        }

        #endregion

    }
}
