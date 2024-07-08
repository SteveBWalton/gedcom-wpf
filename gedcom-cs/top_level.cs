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

        /// <summary>The top level tag that defined this top level element.</summary>
        protected Tag _tag;

        #endregion

        #region Class Constructors

        /// <summary>Empty class constructor.</summary>
        public TopLevel()
        {
            _tag = new Tag();
        }

        /// <summary>Create a top level element from the specified tag.</summary>
        /// <param name="tag">Specifies the top level tag that defines this top level element.</param>
        public TopLevel(Tag tag)
        {
            _tag = tag;
        }

        #endregion

        #region Properties

        /// <summary>The top level that defined this top level element.</summary>
        public Tag tag
        {
            get { return _tag; }
        }

        /// <summary>The index of this top level element.</summary>
        public string idx
        {
            get
            {
                return Tag.toIdx(_tag.key);
            }
        }

        public DateTime lastChanged
        {
            get
            {
                Tag tagChanged = _tag.children.findOne("CHAN");
                if (tagChanged==null)
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

        #endregion

    }
}
