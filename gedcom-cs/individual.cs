using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;

namespace gedcom
{
    /// <summary>Class to represent an individual in a gedcom.</summary>
    public class Individual : TopLevel, IComparable<Individual>
    {
        #region Member Variables

        #endregion

        #region Class Constructors



        /// <summary>Empty class constructor.</summary>
        public Individual()
        {
        }



        /// <summary>Create an individual from the specified tag.</summary>
        /// <param name="tag">Specifies the tag to build the individual from.</param>
        /// <param name="gedcom">Specifies the gedcom that contains this top level element.</param>
        public Individual(Tag tag, Gedcom gedcom) : base(tag, gedcom)
        {
        }



        #endregion

        #region IComparable<Individual>



        /// <summary>Impliment a compare function for sorting.</summary>
        /// <param name="otherIndividual">Specifies the individual to compare with.</param>
        /// <returns>The comparison of the last edit date of the two individuals.</returns>
        public int CompareTo(Individual otherIndividual)
        {
            return otherIndividual.lastChanged.CompareTo(lastChanged);
        }



        #endregion

        #region Properties



        /// <summary>The full name of the individual.</summary>
        public string fullName
        {
            get
            {
                Tag tagName = _tag.children.findOne("NAME");
                if (tagName == null)
                {
                    return "Error";
                }

                string fullName = tagName.value;

                // One way (not the best).
                fullName = fullName.Replace("/", "");

                // This is not really correct.
                return fullName;
            }
        }



        /// <summary>True if the individual is male.</summary>
        public bool isMale
        {
            get
            {
                Tag tagSex = _tag.children.findOne("SEX");
                if (tagSex==null)
                {
                    return true;
                }
                return tagSex.value == "M";
            }
        }



        /// <summary>True if the individual is female.</summary>
        public bool isFemale
        {
            get { return !isMale; }
        }



        /// <summary>He for a male, She for a female.</summary>
        public string heShe
        {
            get { return isMale ? "He" : "She"; }
        }

        #endregion
    }
}
