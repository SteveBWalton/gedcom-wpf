using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom
{
    /// <summary>Class to represent a source, 'SOUR', in a gedcom file.</summary>
    public class Source : TopLevel, IComparable<Source>
    {
        #region Member Variables

        #endregion

        #region Class Constructors

        // <summary>Empty class constructor.</summary>
        //public Source()
        //{
        //}

        /// <summary>Create an source from the specified tag.</summary>
        /// <param name="tag">Specifies the tag to build the source from.  This is expected to be a 'SOUR' tag.</param>
        public Source(Tag tag) : base(tag)
        {
        }

        #endregion

        #region IComparable<Source>

        /// <summary>Impliment a compare function for sorting.</summary>
        /// <param name="otherSource">Specifies the source to compare with.</param>
        /// <returns>The comparison of the last viewed date of the two sources.</returns>
        public int CompareTo(Source otherSource)
        {
            // return otherSource.lastChanged.CompareTo(lastChanged);
            return otherSource.lastViewed.CompareTo(lastViewed);
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



        /// <summary>The human readable description of the source.</summary>
        /// <returns>The full name of the source.</returns>
        public override string ToString()
        {
            return fullName;
        }



        /// <summary>Returns true if this source has a connection to the specified repository.</summary>
        /// <param name="repository">Specifies the repository to test for a connection.</param>
        /// <returns>True if this source has a connection to the specified repository, false otherwise..</returns>
        public bool hasConnection(Repository repository)
        {
            // The repository will be a level 1 tag.
            foreach (Tag tag in _tag.children)
            {
                if (tag.key == "REPO")
                {
                    if (tag.value == repository.tag.key)
                    {
                        return true;
                    }
                }
            }

            // No connection was found.
            return false;
        }

    }
}
