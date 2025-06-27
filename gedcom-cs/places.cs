using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// IEnumerable.
using System.Collections;

namespace gedcom
{
    /// <summary>
    /// Class to represent a collection of places.
    /// </summary>
    public class Places : IEnumerable<Place>
    {
        #region Member Variables

        /// <summary>The collection of individuals.</summary>
        private readonly List<Place> _places;

        /// <summary>The parent place for this collection.  Can be null.</summary>
        private Place _parent;

        #endregion

        #region Construtors

        /// <summary>Constructor for the places collection.</summary>
        /// <param name="parent">Specifies the parent place that owns this collection of places.</param>
        public Places(Place parent)
        {
            _places = new List<Place>();
            _parent = parent;
        }

        #endregion

        #region Properties

        /// <summary>The parent place for this collection.  Can be null.</summary>
        public Place parent
        {
            get => _parent;
        }

        #endregion

        #region List

        /// <summary>Empty the collection.</summary>
        /// <returns>True for success, false otherwise.</returns>
        public bool clear()
        {
            _places.Clear();
            return true;
        }



        /// <summary>The numbers of individuals in the collection.</summary>
        public int count
        {
            get { return _places.Count; }
        }



        /// <summary>Add a place to the collection.</summary>
        /// <param name="tag">Specifies the individual to add to the collection.</param>
        /// <returns>True for success, false otherwise.</returns>
        public bool add(Place place)
        {
            _places.Add(place);
            return true;
        }



        /// <summary>An indexer for this class.</summary>
        /// <param name="idx">Specifies the index of the place[0..count-1].</param>
        /// <returns>The place at the specified position.</returns>
        public Place this[int idx]
        {
            get { return (Place)_places[idx]; }
        }

        #endregion

        #region IEnumerable<Place>



        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }



        public IEnumerator<Place> GetEnumerator()
        {
            for (int idx = 0; idx < _places.Count; idx++)
            {
                yield return (Place)_places[idx];
            }
        }



        #endregion

        /// <summary>Return the place in the collection with the specified name.</summary>
        /// <remarks>If the place name contains a comma then a deep search.</remarks>
        /// <param name="placeName">Specifies the name of the place to find.</param>
        /// <returns>The place with the specified name or null.</returns>
        public Place getPlace(string placeName)
        {
            if (placeName.Contains(","))
            {
                // Deep search of the child places.
                int lastPos = placeName.LastIndexOf(",");
                string right = placeName.Substring(lastPos + 1).Trim();
                string left = placeName.Substring(0, lastPos).Trim();
                Place parent = getPlace(right);
                if (parent == null)
                {
                    return null;
                }
                return parent.children.getPlace(left);
            }
            else
            {
                // Standard search of theses places.
                foreach (Place place in _places)
                {
                    if (place.name == placeName)
                    {
                        return place;
                    }
                }
            }
            return null;
        }


        
        /// <summary>
        /// Might change this add tag???
        /// </summary>
        /// <returns></returns>
        public bool addString(string text)
        {
            // Split off the final place.
            string right = text;
            string left = "";
            if (text.Contains(","))
            {
                int lastPos = text.LastIndexOf(",");
                right = text.Substring(lastPos + 1).Trim();
                left = text.Substring(0, lastPos).Trim();
            }

            Place place = getPlace(right);
            if (place ==null)
            {
                place = new Place(_parent, right);
                add(place);
            }
            if (left != "")
            {
                place.children.addString(left);
            }

            // return success
            return true;
        }
    }
}
