using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ArrayList
using System.Collections;

namespace gedcom
{
    /// <summary>Class to represent a collection of family objects.</summary>
    public class Families : IEnumerable<Family>
    {
        #region Member Variables

        /// <summary>The collection of tags.</summary>
        private ArrayList _families;

        #endregion

        #region Class Constructor

        /// <summary>Empty class constructor.</summary>
        public Families()
        {
            clear();
        }

        #endregion

        #region List



        /// <summary>Empty the collection.</summary>
        /// <returns>True for success, false otherwise.</returns>
        public bool clear()
        {
            _families = new ArrayList();
            return true;
        }



        /// <summary>The numbers of families in the collection.</summary>
        public int count
        {
            get { return _families.Count; }
        }



        /// <summary>Add a family to the collection.</summary>
        /// <param name="tag">Specifies the family to add to the collection.</param>
        /// <returns>True for success, false otherwise.</returns>
        public bool add(Family family)
        {
            _families.Add(family);
            return true;
        }



        /// <summary>An indexer for this class.</summary>
        /// <param name="idx">Specifies the index of the family [0..count-1].</param>
        /// <returns>The tag at the specified position.</returns>
        public Family this[int idx]
        {
            get { return (Family)_families[idx]; }
        }



        #endregion

        #region IEnumerable<Individual>



        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }



        public IEnumerator<Family> GetEnumerator()
        {
            for (int idx = 0; idx < _families.Count; idx++)
            {
                yield return (Family)_families[idx];
            }
        }



        #endregion

        /// <summary>Return the family with the specified index.</summary>
        /// <param name="idx">Specifies the index to search for.</param>
        /// <returns>The family with the specified index or null.</returns>
        public Family find(string idx)
        {
            foreach (Family family in this)
            {
                if (family.idx == idx)
                {
                    return family;
                }
            }
            return null;
        }
    }
}
