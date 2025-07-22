using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// IEnumerable.
using System.Collections;

namespace gedcom
{
    /// <summary>Class to represent a collection of repositories in a gedcom.</summary>
    public class Repositories : IEnumerable<Repository>
    {
        #region Member Variables

        /// <summary>The collection of individuals.</summary>
        private readonly List<Repository> _repositories;

        #endregion

        #region Constructors

        /// <summary>Empty class constructor.</summary>
        public Repositories()
        {
            _repositories = new List<Repository>();            
        }

        #endregion

        #region List

        /// <summary>Empty the collection.</summary>
        /// <returns>True for success, false otherwise.</returns>
        public bool clear()
        {
            _repositories.Clear();
            return true;
        }



        /// <summary>The numbers of individuals in the collection.</summary>
        public int count
        {
            get { return _repositories.Count; }
        }



        /// <summary>Add an individual to the collection.</summary>
        /// <param name="tag">Specifies the individual to add to the collection.</param>
        /// <returns>True for success, false otherwise.</returns>
        public bool add(Repository repository)
        {
            _repositories.Add(repository);
            return true;
        }



        /// <summary>An indexer for this class.</summary>
        /// <param name="idx">Specifies the index of the individual [0..count-1].</param>
        /// <returns>The individual at the specified position.</returns>
        public Repository this[int idx]
        {
            get { return (Repository)_repositories[idx]; }
        }

        #endregion

        #region IEnumerable<Repository>



        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }



        public IEnumerator<Repository> GetEnumerator()
        {
            for (int idx = 0; idx < _repositories.Count; idx++)
            {
                yield return (Repository)_repositories[idx];
            }
        }



        #endregion

        /// <summary>Return the individual with the specified index.</summary>
        /// <param name="idx">Specifies the index to search for.</param>
        /// <returns>The individual with the specified index or null.</returns>
        public Repository find(string idx)
        {
            foreach (Repository repository in this)
            {
                if (repository.idx == idx)
                {
                    return repository;
                }
            }
            return null;
        }



        public Repository[] inDateOrder()
        {
            // Get an array of the repositories.
            Repository[] array = (Repository[])_repositories.ToArray();

            // Sort the array.
            Array.Sort(array);

            // Return the sorted array.
            return array;
        }

    }
}
