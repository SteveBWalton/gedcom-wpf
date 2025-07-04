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
    public class Places : IEnumerable<Place>
    {
        #region Member Variables

        /// <summary>The collection of individuals.</summary>
        private readonly List<Place> _places;

        /// <summary>The parent place for this collection.  Can be null.</summary>
        private Place _parent;

        #endregion

        #region Construtors

        /// <summary>Constructor for the places collection.</summary>
        /// <param name="parent">Specifies the parent place that owns this collection of places.</param>
        public Places(Place parent)
        {
            _places = new List<Place>();
            _parent = parent;
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

        /// <summary>Return the place in the collection with the specified name.</summary>
        /// <remarks>If the place name contains a comma then a deep search.</remarks>
        /// <param name="placeName">Specifies the name of the place to find.</param>
        /// <returns>The place with the specified name or null.</returns>
        public Place getPlace(string placeName)
        {
            // Replace any '%20' with actual spaces.
            placeName = placeName.Replace("%20", " ");
            
            // Standard search or deep search.
            if (placeName.Contains(","))
            {
                // Deep search of the child places.
                int lastPos = placeName.LastIndexOf(",");
                string right = placeName.Substring(lastPos + 1).Trim();
                string left = placeName.Substring(0, lastPos).Trim();
                Place parent = getPlace(right);
                if (parent == null)
                {
                    return null;
                }
                return parent.children.getPlace(left);
            }
            else
            {
                // Standard search of theses places.
                foreach (Place place in _places)
                {
                    if (place.name == placeName)
                    {
                        return place;
                    }
                }
            }
            return null;
        }


        
        /// <summary>
        /// Might change this add tag???
        /// </summary>
        /// <returns></returns>
        private bool addString(string text, bool isAddress, double latitude, double longitude)
        {
            Place place = getPlace(text);
            if (place == null)
            {
                // Add this place.
                // Split off the final place.
                if (text.Contains(","))
                {
                    int lastPos = text.LastIndexOf(",");
                    string right = text.Substring(lastPos + 1).Trim();
                    string left = text.Substring(0, lastPos).Trim();

                    // Add or find the right most single place.
                    place = getPlace(right);
                    if (place == null)
                    {
                        place = new Place(_parent, right, false, 0, 0);
                        add(place);
                    }

                    // Add all the left places to the children of the right most place.
                    place.children.addString(left, isAddress, latitude, longitude);
                }
                else
                {
                    // Add this single place to this collection.
                    place = new Place(_parent, text, isAddress, latitude, longitude);
                    add(place);
                    Console.WriteLine(text + " at " + latitude.ToString() + ", " + longitude.ToString());
                }
            }
            else
            {
                // Update the existing place.
                if (latitude != 0.0 && longitude != 0.0)
                {
                    place.latitude = latitude;
                    place.longitude = longitude;
                }
            }

            // return success
            return true;
        }
    
    

        /// <summary>Add the place in the tag to this collection.</summary>
        /// <param name="tag">Specifies the PLAC tag to add the place from.</param>
        /// <returns>True for success, false otherwise.</returns>
        public bool addTag(Tag tag)
        {
            double longitude = 0.0;
            double latitude = 0.0;

            Tag tagMap = tag.children.findOne("MAP");
            if (tagMap != null)
            {
                Tag tagLatitude = tagMap.children.findOne("LATI");
                if (tagLatitude != null)
                {
                    double.TryParse(tagLatitude.value.Substring(1), out latitude);
                    if (tagLatitude.value.Substring(0, 1) == "S")
                    {
                        latitude = -latitude;
                    }
                }

                Tag tagLongitude = tagMap.children.findOne("LONG");
                if (tagLongitude != null)
                {
                    double.TryParse(tagLongitude.value.Substring(1), out longitude);
                    if (tagLongitude.value.Substring(0, 1) == "W")
                    {
                        longitude = -longitude;
                    }
                }
            }

            Tag tagAddress = tag.children.findOne("ADDR");

            if (tagAddress == null)
            {
                // Add the basic place.
                return addString(tag.value, false, latitude, longitude);
            }
            else
            {
                // Add the place plus the address.
                return addString(tagAddress.value + ", " + tag.value, true, latitude, longitude);
            }
        }
    }
}
