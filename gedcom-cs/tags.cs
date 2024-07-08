using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ArrayList
using System.Collections;

namespace gedcom
{
    /// <summary>Class to represent a collection of gedcom tags.</summary>
    public class Tags : IEnumerable<Tag>
    {
        #region Member Variables

        /// <summary>The collection of tags.</summary>
        private ArrayList tags_;

        #endregion

        #region Class Constructor

        /// <summary>Empty class constructor.</summary>
        public Tags()
        {
            clear();
        }

        #endregion

        #region List



        /// <summary>Empty the collection.</summary>
        /// <returns>True for success, false otherwise.</returns>
        public bool clear()
        {
            tags_ = new ArrayList();
            return true;
        }



        /// <summary>The numbers of tags in the collection.</summary>
        public int count
        {
            get { return tags_.Count; }
        }



        /// <summary>Add a tag to the collection.</summary>
        /// <param name="tag">Specifies the tag to add to the collection.</param>
        /// <returns>True for success, false otherwise.</returns>
        public bool add(Tag tag)
        {
            tags_.Add(tag);
            return true;
        }



        /// <summary>An indexer for this class.</summary>
        /// <param name="idx">Specifies the index of the tag [0..count-1].</param>
        /// <returns>The tag at the specified position.</returns>
        public Tag this[int idx]
        {
            get { return (Tag)tags_[idx]; }
        }



        #endregion

        #region IEnumerable<Tag>



        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }



        public IEnumerator<Tag> GetEnumerator()
        {
            for (int idx = 0; idx < tags_.Count; idx++)
            {
                yield return (Tag)tags_[idx];
            }
        }



        #endregion

        #region Public Methods

        public Tag findOne(string tagName)
        {
            foreach (Tag child in this)
            {
                if (child.key == tagName)
                {
                    return child;
                }
            }

            // Return failure
            return null;
        }

        #endregion
    }
}
