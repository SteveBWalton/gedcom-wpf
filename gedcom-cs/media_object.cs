using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;

namespace gedcom
{
    /// <summary>Class to represent a media object, 'OBJE', in a gedcom.</summary>
    public class MediaObject : TopLevel, IComparable<MediaObject>
    {
        #region Member Variables

        #endregion

        #region Constructors



        /// <summary>Create a media object from the specified tag.</summary>
        /// <param name="tag">Specifies the tag to build the media object from.  This is expected to be a 'OBJE' tag.</param>
        public MediaObject(Tag tag) : base(tag)
        {
        }

        #endregion

        #region Properties

        /// <summary>The title for the media object.</summary>
        public string title
        {
            get
            {
                Tag tagFile = _tag.children.findOne("FILE");
                Tag tagTitle = null;
                if (tagFile != null)
                {
                    tagTitle = tagFile.children.findOne("TITL");
                }
                if (tagTitle == null)
                {
                    return _tag.key;
                }
                return tagTitle.value;
            }
        }

        #endregion

        #region IComparable<MediaObject>

        /// <summary>Compare function for sorting inidividuals by last viewed.</summary>
        /// <param name="otherIndividual">Specifies the individual to compare with.</param>
        /// <returns>The comparison of the last viewed date of the two individuals.</returns>
        public int CompareTo(MediaObject otherMediaObject)
        {
            return otherMediaObject.lastViewed.CompareTo(lastViewed);
        }

        #endregion

    }
}
