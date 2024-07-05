using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom
{
    /// <summary>Class to represent a gedcom tag.</summary>
    public class Tag
    {
        #region Member Variables

        private string key_;
        private string value_;
        private Tags children_;

        #endregion

        #region Class Constructors

        public Tag(string key, string value)
        {
            key_ = key;
            value_ = value;
            children_ = new Tags();
        }

        #endregion

        #region Properties

        public string key
        {
            get { return key_; }
        }
        public string value
        {
            get { return value_; }
            set { value_ = value; }
        }
        public Tags children
        {
            get { return children_; }
        }

        #endregion

    }
}
