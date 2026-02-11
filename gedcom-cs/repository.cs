using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;

namespace gedcom
{
    /// <summary>Class to represent an repostory, 'REPO', in a gedcom.</summary>
    public class Repository : TopLevel, IComparable<Repository>
    {
        #region Member Variables

        #endregion

        #region Constructors

        /// <summary>Create an repository from the specified tag.</summary>
        /// <param name="tag">Specifies the tag to build the repository from.  This is expected to be a 'REPO' tag.</param>
        public Repository(Tag tag) : base(tag)
        {
            // Base class does everything!
        }

        #endregion

        #region Properties

        /// <summary>The name of the repository.</summary>
        public string name
        {
            get
            {
                Tag tagName = _tag.children.findOne("NAME");
                if (tagName == null)
                {
                    return _tag.key;
                }
                return tagName.value;
            }
        }

        #endregion

        #region IComparable<Repository>

        /// <summary>Compare function for sorting inidividuals by last viewed.</summary>
        /// <param name="otherIndividual">Specifies the individual to compare with.</param>
        /// <returns>The comparison of the last viewed date of the two individuals.</returns>
        public int CompareTo(Repository otherRepository)
        {
            return otherRepository.lastViewed.CompareTo(lastViewed);
        }

        #endregion



        /// <summary>Show the name.</summary>
        /// <returns>The name of the repository.</returns>
        public override string ToString()
        {
            return name;
        }
    }
}
