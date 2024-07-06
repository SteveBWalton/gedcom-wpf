using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom
{
    /// <summary>Class to represent an individual in a gedcom.</summary>
    public class Individual
    {
        #region Member Variables

        /// <summary>The gedcom tags for this individual.</summary>
        private Tags tags_;

        #endregion

        #region Class Constructors

        /// <summary>Empty class constructor.</summary>
        public Individual()
        {
            tags_ = new Tags();
        }

        #endregion

    }
}
