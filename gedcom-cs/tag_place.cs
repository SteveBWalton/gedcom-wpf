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
                // Get the full places.
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

                // Return the first part of the address.
                int commaLoc = place.IndexOf(",");
                if (commaLoc > 0)
                {
                    return place.Substring(0, commaLoc);
                }
                return place;
            }
        }

        #endregion


        /// <summary>The long one line description of the place, including the address, if available.</summary>
        /// <returns>The long one line description of the place, including the address, if available.</returns>
        public override string ToString()
        {
            if (_tag is null)
            {
                return "Error: Missing Tag";
            }

            // Check for an address.
            Tag tagAddress = _tag.children.findOne("ADDR");
            if (tagAddress != null)
            {
                // Return the address and the place.
                string address = tagAddress.value;
                return address + ", " + _tag.value;
            }

            // Return the full place.
            return _tag.value;
        }

    }
}
