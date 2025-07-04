using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom
{
    /// <summary>
    /// Class to represent a place used in a gedcom file.
    /// Place is not a gedcom top level tag.
    /// Place tags are used inside top level tags at any level.
    /// </summary>
    public class Place
    {
        #region Member Variables

        /// <summary>The parent place of this place.</summary>
        private readonly Place _parent;

        /// <summary>The child places of this place.</summary>
        private readonly Places _children;

        /// <summary>The name of this place.</summary>
        private string _name;

        /// <summary>True if this place is really just an address.</summary>
        private bool _isAddress;

        /// <summary>The latitude of the place.  Positive is north, negative is south.</summary>
        private double _latitude;

        /// <summary>The longitude of the place.  Positive is east, negative is west.</summary>
        private double _longitude;

        #endregion

        #region Constructors

        /// <summary>Constructor that specified the parent for this place.</summary>
        /// <param name="parent">Specifies the parent for this place.</param>
        /// <param name="name">Specifies the name of this place.</param>
        public Place(Place parent, string name, bool isAddress, double latitude, double longitude)
        {
            _parent = parent;
            _children = new Places(this);
            _name = name;
            _isAddress = isAddress;
            _latitude = latitude;
            _longitude = longitude;
        }


        #endregion

        #region Properties

        /// <summary>The parent place of this place.</summary>
        public Place parent
        {
            get => _parent;
        }



        /// <summary>The child places of this place.</summary>
        public Places children
        {
            get => _children;
        }



        /// <summary>The name of this place.</summary>
        public string name
        {
            get => _name;
        }



        /// <summary>The full name of this place.</summary>
        public string fullName
        {
            get
            {
                if (_parent == null)
                {
                    return _name;
                }
                return _name + ", " + _parent.fullName;
            }
        }



        /// <summary>The latitude of the place.  Positive is north, negative is south.</summary>
        public double latitude
        {
            get => _latitude;
            set { _latitude = value; }
        }



        /// <summary>The longitude of the place.  Positive is east, negative is west.</summary>
        public double longitude
        {
            get => _longitude;
            set { _longitude = value; }
        }

        #endregion

        /// <summary>The total number of child places for this place.</summary>
        public int getTotalCount()
        {
            int count = 1;
            foreach (Place child in _children)
            {
                count += child.getTotalCount();
            }
            return count;
        }
    }
}
