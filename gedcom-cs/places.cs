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
    class Places : IEnumerable<Place>
    {
        #region Member Variables

        /// <summary>The collection of individuals.</summary>
        private readonly List<Place> _places;

        /// <summary>The parent place for this collection.  Can be null.</summary>
        private Place _parent;

        #endregion

        #region Construtors

        /// <summary>Constructor for the places collection.</summary>
        public Places()
        {
            _places = new List<Place>();
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
    }
}
