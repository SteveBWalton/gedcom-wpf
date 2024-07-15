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
        // private ArrayList _tags;
        private List<Tag> _tags;

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
            _tags = new List<Tag>();
            return true;
        }



        /// <summary>The numbers of tags in the collection.</summary>
        public int count
        {
            get { return _tags.Count; }
        }



        /// <summary>Add a tag to the collection.</summary>
        /// <param name="tag">Specifies the tag to add to the collection.</param>
        /// <returns>True for success, false otherwise.</returns>
        public bool add(Tag tag)
        {
            _tags.Add(tag);
            return true;
        }



        /// <summary>An indexer for this class.</summary>
        /// <param name="idx">Specifies the index of the tag [0..count-1].</param>
        /// <returns>The tag at the specified position.</returns>
        public Tag this[int idx]
        {
            get { return (Tag)_tags[idx]; }
        }



        #endregion

        #region IEnumerable<Tag>



        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }



        public IEnumerator<Tag> GetEnumerator()
        {
            for (int idx = 0; idx < _tags.Count; idx++)
            {
                yield return (Tag)_tags[idx];
            }
        }



        #endregion

        #region Public Methods



        /// <summary>Returns the first tag with the specified key or null.</summary>
        /// <param name="tagKey">Specifies the key to search for.</param>
        /// <returns>The first tag with the specified key or null.</returns>
        public Tag findOne(string tagKey)
        {
            // Loop the tags in this collection.
            foreach (Tag child in this)
            {
                if (child.key == tagKey)
                {
                    // Return success.
                    return child;
                }
            }

            // Return failure
            return null;
        }



        /// <summary>Returns all the tags with the specified key or null.</summary>
        /// <param name="tagKey">Specifies the key to search for.</param>
        /// <returns>The collection of tags with the specifeid key.</returns>
        public Tag[] findAll(string tagKey)
        {
            // Start a collection of tags.
            List<Tag> tags = new List<Tag>();

            // Loop the tags in this collection.
            foreach (Tag child in this)
            {
                if (child.key == tagKey)
                {
                    tags.Add(child);
                }
            }

            // Return built collection.
            return tags.ToArray();
        }

        #endregion
    }
}
