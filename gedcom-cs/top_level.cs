using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom
{
    /// <summary>The common attributes that all top level elements share.</summary>
    public class TopLevel
    {
        #region Member Variables

        /// <summary>The tag that defined this top level element.</summary>
        protected Tag _tag;

        /// <summary>The date time that this top level element was viewed.</summary>
        protected DateTime _lastViewed;

        #endregion

        #region Class Constructors


        /// <summary>Create a top level element in the specified gedcom.</summary>
        /// <param name="gedcom">Specifies the gedcom that contains this top level element.</param>
        public TopLevel(Gedcom gedcom)
        {
            _tag = new Tag(gedcom);
            _lastViewed = DateTime.MinValue;
        }



        /// <summary>Create a top level element from the specified tag.</summary>
        /// <param name="tag">Specifies the top level tag that defines this top level element.</param>
        public TopLevel(Tag tag)
        {
            _tag = tag;
            _lastViewed = DateTime.MinValue;
        }

        #endregion

        #region Properties



        /// <summary>The top level tag that defined this top level element.</summary>
        public Tag tag
        {
            get => _tag;
        }



        /// <summary>The index of this top level element.</summary>
        public string idx
        {
            get => Tag.toIdx(_tag.key);
        }



        /// <summary>The gedcom that contains this top level element.</summary>
        public Gedcom gedcom
        {
            get => _tag.gedcom;
        }



        /// <summary>The datetime that this top level element was last changed.</summary>
        public DateTime lastChanged
        {
            get
            {
                Tag tagChanged = _tag.children.findOne("CHAN");
                if (tagChanged == null)
                {
                    return DateTime.MinValue;
                }
                Tag tagDate = tagChanged.children.findOne("DATE");
                if (tagDate == null)
                {
                    return DateTime.MinValue;
                }
                Tag tagTime = tagDate.children.findOne("TIME");
                if (tagTime == null)
                {
                    return DateTime.Parse(tagDate.value);
                }
                return DateTime.Parse(tagDate.value + " " + tagTime.value);
            }
        }



        /// <summary>The datetime that this top level element was last viewed.</summary>
        /// <remarks>Really only expecting set to be lastViewed = DateTime.Now.</remarks>
        public DateTime lastViewed
        {
            get
            {
                if (_lastViewed == DateTime.MinValue)
                {
                    return lastChanged;
                }
                return _lastViewed;
            }
            set
            {
                _lastViewed = value;
            }
        }

        #endregion

        /// <summary>Set the last changed to now.</summary>
        /// <remarks>Might add the user name as a parameter for this?</remarks>
        /// <returns>True for a new tag, false for update the existing tag.</returns>
        public bool setLastChanged()
        {
            // The default return value.
            bool isNewTag = false;

            // Get the changed tag,
            Tag tagChanged = _tag.children.findOne("CHAN");
            if (tagChanged == null)
            {
                // Add a new tag.
                isNewTag = true;
                tagChanged = new Tag(_tag, "CHAN", "Y");
                _tag.children.add(tagChanged);
            }

            // Get the date tag.
            Tag tagDate = tagChanged.children.findOne("DATE");
            if (tagDate == null)
            {
                // Add a new tag.
                isNewTag = true;
                tagDate = new Tag(tagChanged, "DATE", DateTime.Now.ToString("d MMM yyyy").ToUpper());
                tagChanged.children.add(tagDate);
            }
            else
            {
                tagDate.value = DateTime.Now.ToString("d MMM yyyy").ToUpper();
            }

            // Get the time tag.
            Tag tagTime = tagDate.children.findOne("TIME");
            if (tagTime == null)
            {
                // Add a new tag.
                isNewTag = true;
                tagTime = new Tag(tagDate, "TIME", DateTime.Now.ToString("HH:mm"));
                tagDate.children.add(tagTime);
            }
            else
            {
                tagTime.value = DateTime.Now.ToString("HH:mm");
            }

            // Get the changed by tag.
            System.Security.Principal.WindowsIdentity windowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();
            string userName = windowsIdentity.Name;
            Tag tagEditBy = tagChanged.children.findOne("_PGVU");
            if (tagEditBy == null)
            {
                // Add a new tag.
                isNewTag = true;
                tagEditBy = new Tag(tagChanged, "_PGVU", userName);
                tagChanged.children.add(tagEditBy);
            }
            else
            {
                // Update the existing tag.
                tagEditBy.value = userName;
            }

            // Mark the gedcom as dirty.
            _tag.gedcom.isDirty = true;

            // Return the new tag status.
            return isNewTag;
        }
    }
}
