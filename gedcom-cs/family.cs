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

        #region Functions



        /// <summary>Returns the children in this family.</summary>
        /// <returns>The children in this family.</returns>
        public Individual[] getChildren()
        {
            // Build a list of the children.
            List<Individual> children = new List<Individual>();

            // Find the tags for the children.
            Tag[] tags = _tag.children.findAll("CHIL");
            foreach (Tag tag in tags)
            {
                Individual child = _gedcom.individuals.find(Tag.toIdx(tag.value));
                if (child!=null)
                {
                    children.Add(child);
                }
            }

            // Return the children found.
            return children.ToArray();
        }



        #endregion

        #region Properties



        /// <summary>The index of the husband in this family or null.</summary>
        public string husbandIdx
        {
            get
            {
                Tag tagHusband = _tag.children.findOne("HUSB");
                if (tagHusband == null)
                {
                    return null;
                }
                return Tag.toIdx(tagHusband.value);
            }
        }



        /// <summary>The index of the wife in this family or null.</summary>
        public string wifeIdx
        {
            get
            {
                Tag tagWife = _tag.children.findOne("WIFE");
                if (tagWife == null)
                {
                    return null;
                }
                return Tag.toIdx(tagWife.value);
            }
        }



        /// <summary>The individual who is the husband in this family or null.</summary>
        public Individual husband
        {
            get
            {
                string idx = husbandIdx;
                if (idx == null)
                {
                    return null;
                }
                return _gedcom.individuals.find(idx);
            }
        }



        /// <summary>The individual who is the wife in this family or null.</summary>
        public Individual wife

        {
            get
            {
                string idx = wifeIdx;
                if (idx == null)
                {
                    return null;
                }
                return _gedcom.individuals.find(idx);
            }
        }



        public string fullName
        {
            get
            {
                string name = "";
                if (husband != null)
                {
                    name = husband.fullName;
                }

                if (wife != null)
                {
                    if (name != "")
                    {
                        name += " and ";
                    }
                    name += wife.fullName;
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
