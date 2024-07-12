using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ArrayList
using System.Collections;

namespace gedcom
{
    /// <summary>Class to represent a collection of sources in a gedcom.</summary>
    public class Sources : IEnumerable<Source>
    {
        #region Member Variables

        /// <summary>The collection of individuals.</summary>
        private List<Source> _sources;

        #endregion

        #region Class Constructor

        /// <summary>Empty class constructor.</summary>
        public Sources()
        {
            clear();
        }

        #endregion

        #region List



        /// <summary>Empty the collection.</summary>
        /// <returns>True for success, false otherwise.</returns>
        public bool clear()
        {
            _sources = new List<Source>();
            return true;
        }



        /// <summary>The numbers of individuals in the collection.</summary>
        public int count
        {
            get { return _sources.Count; }
        }



        /// <summary>Add an individual to the collection.</summary>
        /// <param name="tag">Specifies the individual to add to the collection.</param>
        /// <returns>True for success, false otherwise.</returns>
        public bool add(Source source)
        {
            _sources.Add(source);
            return true;
        }



        /// <summary>An indexer for this class.</summary>
        /// <param name="idx">Specifies the index of the individual [0..count-1].</param>
        /// <returns>The tag at the specified position.</returns>
        public Source this[int idx]
        {
            get { return _sources[idx]; }
        }



        #endregion

        #region IEnumerable<Source>



        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }



        public IEnumerator<Source> GetEnumerator()
        {
            for (int idx = 0; idx < _sources.Count; idx++)
            {
                yield return (Source)_sources[idx];
            }
        }



        #endregion



        /// <summary>Return the source with the specified index.</summary>
        /// <param name="idx">Specifies the index to search for.</param>
        /// <returns>The source with the specified index or null.</returns>
        public Source find(string idx)
        {
            foreach (Source source in this)
            {
                if (source.idx == idx)
                {
                    return source;
                }
            }
            return null;
        }



        /// <summary>Returns an array of the sources in date last edit order.</summary>
        /// <returns>An array of the sources in date last edit order.</returns>
        public Source[] inDateOrder()
        {
            // Get an array of the sources.
            Source[] array = _sources.ToArray();

            // Sort the array.
            Array.Sort(array);

            // Return the sorted array.
            return array;
        }
    }
}
