using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// IEnumerable.
using System.Collections;

namespace gedcom
{
    /// <summary>Class to represent a collection of individuals in a gedcom.</summary>
    public class Individuals : IEnumerable<Individual>
    {
        #region Member Variables

        /// <summary>The collection of individuals.</summary>
        private readonly List<Individual> _individuals;

        #endregion

        #region Constructors

        /// <summary>Empty class constructor.</summary>
        public Individuals()
        {
            _individuals = new List<Individual>();
        }

        #endregion

        #region List

        /// <summary>Empty the collection.</summary>
        /// <returns>True for success, false otherwise.</returns>
        public bool clear()
        {
            _individuals.Clear();
            return true;
        }



        /// <summary>The numbers of individuals in the collection.</summary>
        public int count
        {
            get { return _individuals.Count; }
        }



        /// <summary>Add an individual to the collection.</summary>
        /// <param name="tag">Specifies the individual to add to the collection.</param>
        /// <returns>True for success, false otherwise.</returns>
        public bool add(Individual individual)
        {
            _individuals.Add(individual);
            return true;
        }



        /// <summary>An indexer for this class.</summary>
        /// <param name="idx">Specifies the index of the individual [0..count-1].</param>
        /// <returns>The individual at the specified position.</returns>
        public Individual this[int idx]
        {
            get { return (Individual)_individuals[idx]; }
        }

        #endregion

        #region IEnumerable<Individual>



        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }



        public IEnumerator<Individual> GetEnumerator()
        {
            for (int idx = 0; idx < _individuals.Count; idx++)
            {
                yield return (Individual)_individuals[idx];
            }
        }



        #endregion

        /// <summary>Return the individual with the specified index.</summary>
        /// <param name="idx">Specifies the index to search for.</param>
        /// <returns>The individual with the specified index or null.</returns>
        public Individual find(string idx)
        {
            foreach (Individual individual in this)
            {
                if (individual.idx == idx)
                {
                    return individual;
                }
            }
            return null;
        }



        public Individual[] inDateOrder()
        {
            // Get an array of the individuals.
            // Individual[] array = (Individual[])_individuals.ToArray(typeof(Individual));
            Individual[] array = (Individual[])_individuals.ToArray();

            // Sort the array.
            Array.Sort(array);

            // Return the sorted array.
            return array;
        }



        /// <summary>Return the next index to use on this collection of individuals.</summary>
        /// <returns>The next index to use on the next individual in this collection of individuals.</returns>
        public string getNextIndex()
        {
            const string NO_RECORDS = "A";
            string maxIndex = NO_RECORDS;
            foreach (Individual individual in _individuals)
            {
                if (maxIndex.CompareTo(individual.idx) < 0)
                {
                    maxIndex = individual.idx;
                }
            }
            int newIndex = 1;
            if (maxIndex != NO_RECORDS)
            {
                newIndex = int.Parse(maxIndex.Substring(1)) + 1;
            }
            return "I" + newIndex.ToString("0000");
        }

    }
}
