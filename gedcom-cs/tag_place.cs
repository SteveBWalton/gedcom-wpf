using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom
{
    /// <summary>
    /// Class to procide extra functionality for 'PLAC' tags.
    /// </summary>
    public class TagPlace
    {
        #region Member Variables

        /// <summary>The actual place tag.</summary>
        private readonly Tag _tag;

        #endregion

        #region Class Constructors

        /// <summary>Creates a TagPlace object from the specified Tag object.</summary>
        /// <param name="tag">Specifies the Tag object which is really expected to be of PLAC type.</param>
        public TagPlace(Tag tag)
        {
            _tag = tag;
        }

        #endregion

        #region Properties

        /// <summary>The short description of the place.</summary>
        /// <remarks>This is the first element of the place but not including the address.</remarks>
        public string shortPlace
        {
            get
            {
                string place = _tag.value;

                // Check for an address.
                Tag tagAddress = _tag.children.findOne("ADDR");
                if (tagAddress != null)
                {
                    string address = tagAddress.value;
                    if (place.StartsWith(address))
                    {
                        place = place.Substring(address.Length);
                        if (place.StartsWith(","))
                        {
                            place = place.Substring(1).Trim();
                        }
                    }
                }
                
                int commaLoc = place.IndexOf(",");
                if (commaLoc > 0)
                {
                    return place.Substring(0, commaLoc);
                }
                return place;
            }
        }

        #endregion

    }
}
