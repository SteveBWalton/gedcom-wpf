using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// IEnumerable.
using System.Collections;

namespace gedcom
{
    /// <summary>Class to represent a collection of media objects in a gedcom.</summary>
    public class MediaObjects : IEnumerable<MediaObject>
    {
        #region Member Variables

        /// <summary>The collection of individuals.</summary>
        private readonly List<MediaObject> _mediaObjects;

        #endregion

        #region Constructors

        /// <summary>Empty class constructor.</summary>
        public MediaObjects()
        {
            _mediaObjects = new List<MediaObject>();
        }

        #endregion

        #region List

        /// <summary>Empty the collection.</summary>
        /// <returns>True for success, false otherwise.</returns>
        public bool clear()
        {
            _mediaObjects.Clear();
            return true;
        }



        /// <summary>The numbers of individuals in the collection.</summary>
        public int count
        {
            get { return _mediaObjects.Count; }
        }



        /// <summary>Add an individual to the collection.</summary>
        /// <param name="tag">Specifies the individual to add to the collection.</param>
        /// <returns>True for success, false otherwise.</returns>
        public bool add(MediaObject mediaObject)
        {
            _mediaObjects.Add(mediaObject);
            return true;
        }



        /// <summary>An indexer for this class.</summary>
        /// <param name="idx">Specifies the index of the individual [0..count-1].</param>
        /// <returns>The individual at the specified position.</returns>
        public MediaObject this[int idx]
        {
            get { return (MediaObject)_mediaObjects[idx]; }
        }

        #endregion

        #region IEnumerable<MediaObject>



        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }



        public IEnumerator<MediaObject> GetEnumerator()
        {
            for (int idx = 0; idx < _mediaObjects.Count; idx++)
            {
                yield return (MediaObject)_mediaObjects[idx];
            }
        }



        #endregion

        /// <summary>Return the individual with the specified index.</summary>
        /// <param name="idx">Specifies the index to search for.</param>
        /// <returns>The individual with the specified index or null.</returns>
        public MediaObject find(string idx)
        {
            foreach (MediaObject mediaObject in this)
            {
                if (mediaObject.idx == idx)
                {
                    return mediaObject;
                }
            }
            return null;
        }



        public MediaObject[] inDateOrder()
        {
            // Get an array of the individuals.
            MediaObject[] array = (MediaObject[])_mediaObjects.ToArray();

            // Sort the array.
            Array.Sort(array);

            // Return the sorted array.
            return array;
        }



        /// <summary>Return the next index to use on this collection of media objects.</summary>
        /// <returns>The next index to use on the next media object in this collection.</returns>
        public string getNextIndex()
        {
            const string NO_RECORDS = "A";
            string maxIndex = NO_RECORDS;
            foreach(MediaObject mediaObject in _mediaObjects)
            {
                if (maxIndex.CompareTo(mediaObject.idx) < 0)
                {
                    maxIndex = mediaObject.idx;
                }
            }
            int newIndex = 1;
            if (maxIndex != NO_RECORDS)
            {
                newIndex = int.Parse(maxIndex.Substring(1)) + 1;
            }
            return "M" + newIndex.ToString("0000");
        }
    }
}
