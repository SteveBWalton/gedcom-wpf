using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom
{
    /// <summary>Class to represent the a type of gedcom tag.</summary>
    public class TagType
    {
        #region Member Variables

        /// <summary>The actual tag string that will appear in a gedcom file.</summary>
        private string _tagKey;

        #endregion

        #region Constructors

        public TagType(string tagKey)
        {
            _tagKey = tagKey;
        }

        #endregion

        #region Properties

        /// <summary>The actual tag string that will appear in a gedcom file.</summary>
        public string tagKey
        {
            get => _tagKey;
        }

        public string tagName
        {
            get
            {
                switch(_tagKey)
                {
                case "SOURCE":
                    return "Source";
                }

                // There is no known name, use the key.
                return _tagKey;
            }
        }

        #endregion

    }
}
