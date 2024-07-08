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

        // Block stuff that we want to get rid of.
        private string _line;
        private int _level;


        private string key_;
        private string value_;
        private Tags _children;

        #endregion

        #region Class Constructors

        public Tag()
        {
            _line = "";
            _level = -1;

            _children = new Tags();
        }

        /*
        public Tag(string key, string value)
        {

            key_ = key;
            value_ = value;
        }
        */

        #endregion

        public bool add(string line)
        {
            // Console.WriteLine(line);
            int level = line[0] - '0';
            if (_line == "")
            {
                _line = line;
                _level = level;
                return true;
            }
            if (level == _level + 1)
            {
                Tag newChild = new Tag();
                newChild.add(line);
                _children.add(newChild);
                return true;
            }
            Tag lastBlock = (Tag)_children[_children.count - 1];
            return lastBlock.add(line);
        }

        public string display(int indent)
        {
            StringBuilder output = new StringBuilder();
            output.Append("".PadRight(indent));
            output.Append(_line);
            output.Append("\r\n");
            foreach (Tag child in children)
            {
                output.Append(child.display(indent + 2));
            }
            return output.ToString();
        }

        public string line
        {
            get { return _line; }
        }

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
            get { return _children; }
        }

        #endregion

    }
}
