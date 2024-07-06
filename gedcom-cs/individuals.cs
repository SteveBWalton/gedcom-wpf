using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ArrayList
using System.Collections;

namespace gedcom
{
    /// <summary>Class to represent a collection of individuals in a gedcom.</summary>
    public class Individuals : IEnumerable<Individual>
    {
        #region Member Variables

        /// <summary>The collection of tags.</summary>
        private ArrayList individuals_;

        #endregion

        #region Class Constructor

        /// <summary>Empty class constructor.</summary>
        public Individuals()
        {
            clear();
        }

        #endregion

        #region List



        /// <summary>Empty the collection.</summary>
        /// <returns>True for success, false otherwise.</returns>
        public bool clear()
        {
            individuals_ = new ArrayList();
            return true;
        }



        /// <summary>The numbers of tags in the collection.</summary>
        public int count
        {
            get { return individuals_.Count; }
        }



        /// <summary>Add a tag to the collection.</summary>
        /// <param name="tag">Specifies the tag to add to the collection.</param>
        /// <returns>True for success, false otherwise.</returns>
        public bool add(Individual individual)
        {
            individuals_.Add(individual);
            return true;
        }



        /// <summary>An indexer for this class.</summary>
        /// <param name="idx">Specifies the index of the tag [0..count-1].</param>
        /// <returns>The tag at the specified position.</returns>
        public Individual this[int idx]
        {
            get { return (Individual)individuals_[idx]; }
        }



        #endregion

        #region IEnumerable<Individual>



        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }



        public IEnumerator<Individual> GetEnumerator()
        {
            for (int idx = 0; idx < individuals_.Count; idx++)
            {
                yield return (Individual)individuals_[idx];
            }
        }



        #endregion

    }
}
