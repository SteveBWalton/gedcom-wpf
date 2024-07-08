using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom
{
    /// <summary>Class to represent an individual in a gedcom.</summary>
    public class Individual
    {
        #region Member Variables

        /// <summary>The gedcom tags for this individual.</summary>
        private Tags _tags;

        /// <summary>
        /// The gedcom tag for this individual.
        /// </summary>
        private Tag _tag;

        #endregion

        #region Class Constructors

        /// <summary>Empty class constructor.</summary>
        public Individual()
        {
            _tags = new Tags();
        }

        /// <summary>Create an individual from the specified tag.</summary>
        /// <param name="tag">Specifies the tag to build the individual from.</param>
        public Individual(Tag tag)
        {
            _tags = tag.children;
            _tag = tag;
        }

        #endregion

        #region Properties

        /// <summary>The gedcom tags on this individual. </summary>
        public Tags tags
        {
            get { return _tags; }
        }

        public Tag tag
        {
            get { return _tag; }
        }

        #endregion
    }
}
