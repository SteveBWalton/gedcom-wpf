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

        /// <summary>The parent tag of this tag.</summary>
        private readonly Tag _parent;
        /// <summary>The gedcom that contains this tag.</summary>
        private readonly Gedcom _gedcom;
        /// <summary>The original line that created this tag. </summary>
        private string _line;
        /// <summary>The original level of this tag.</summary>
        private int _level;
        /// <summary>The child tags of this tag.</summary>
        private Tags _children;
        /// <summary>The key of the tag.</summary>
        private string _key;
        /// <summary>The value of the tag.</summary>
        private string _value;

        #endregion

        #region Constructors

        /// <summary>Class constructor.</summary>
        /// <param name="gedcom">Specifies the gedcom that contains the tag.</param>
        public Tag(Gedcom gedcom)
        {
            _parent = null;
            _gedcom = gedcom;
            _line = "";
            _level = -1;
            _children = new Tags();
            _key = "";
            _value = "";
        }



        /// <summary>Constructor for a top level tag with no parent tag with the initial values.</summary>
        /// <param name="gedcom">Specifies the gedcom that contains the tag.</param>
        /// <param name="tagKey">Specifies the key of the tag.</param>
        /// <param name="tagValue">Specifies the initial value of the tag.</param>
        /// <param name="tagLevel">Specifies the level of the tag.</param>
        public Tag(Gedcom gedcom, string tagKey, string tagValue) : this(gedcom)
        {
            _key = tagKey;
            _value = tagValue;
            _level = 0;
        }



        /// <summary>Constructor with the initial values.</summary>
        /// <param name="gedcom">Specifies the gedcom that contains the tag.</param>
        /// <param name="tagKey">Specifies the key of the tag.</param>
        /// <param name="tagValue">Specifies the initial value of the tag.</param>
        /// <param name="tagLevel">Specifies the level of the tag.</param>
        public Tag(Tag parent, Gedcom gedcom, string tagKey, string tagValue, int tagLevel) : this(gedcom)
        {
            _parent = parent;
            _key = tagKey;
            _value = tagValue;
            _level = tagLevel;
        }



        /// <summary>Constructor with the initial values.</summary>
        /// <param name="tagKey">Specifies the key of the tag.</param>
        /// <param name="tagValue">Specifies the initial value of the tag.</param>
        /// <param name="tagLevel">Specifies the level of the tag.</param>
        public Tag(Tag parent, string tagKey, string tagValue) : this(parent.gedcom)
        {
            _parent = parent;
            _key = tagKey;
            _value = tagValue;
            _level = parent.level+1;
        }


        #endregion

        #region Properties

        /// <summary>The gedcom that contains this tag.</summary>
        public Gedcom gedcom
        {
            get => _gedcom;
        }



        /// <summary>The whole (original?) line that created this tag.</summary>
        public string line
        {
            get { return _line; }
        }



        /// <summary>The type or key of the gedcom tag.</summary>
        public string key
        {
            get { return _key; }
        }



        /// <summary>The value of the gedcom tag.</summary>
        public string value
        {
            get { return _value; }
            set { _value = value; }
        }



        /// <summary>The child tags of this gedcom tag.</summary>
        public Tags children
        {
            get { return _children; }
        }



        /// <summary>The  level of this tag.</summary>
        public int level
        {
            get => _level;
        }

        #endregion


        /// <summary>Returns the specified key as an index string.</summary>
        /// <param name="key">Specifies the key to convert to an index string.</param>
        public static string toIdx(string key)
        {
            // Really expect this to be true.
            if (key.StartsWith("@") && key.EndsWith("@"))
            {
                return key.Substring(1, key.Length - 2);
            }
            // Don't really expect this.
            return key;
        }



        /// <summary>Add a line from a gedcom file to this tag.</summary>
        /// <param name="line">Specifies the line to add.</param>
        /// <returns>True for success, false otherwise.</returns>
        public bool add(string line)
        {
            // Console.WriteLine(line);
            // Get the level of this line.
            int firstSpace = 1;
            if (line[1] != ' ')
            {
                firstSpace = 2;
            }
            // One character for level.
            int level = line[0] - '0';
            if (firstSpace == 2)
            {
                // Two characters for level.
                level = 10 * level + line[1] - '0';
            }
            if (_line == "")
            {
                // This tag is defined by the line.
                _line = line;
                _level = level;
                // Find the second space.
                int secondSpace = firstSpace + 1;
                while (line[secondSpace] != ' ')
                {
                    secondSpace++;
                    if (secondSpace >= line.Length)
                    {
                        secondSpace = -1;
                        break;
                    }
                }
                if (secondSpace == -1)
                {
                    _key = _line.Substring(firstSpace + 1);
                    _value = "";

                }
                else
                {
                    _key = _line.Substring(firstSpace + 1, secondSpace - firstSpace - 1);
                    _value = _line.Substring(secondSpace + 1);
                }
                return true;
            }
            if (level == _level + 1)
            {
                // A new child tag is defined by the line.
                Tag newChild = new Tag(_gedcom);
                newChild.add(line);
                _children.add(newChild);
                return true;
            }
            // New information to added to the last tag by the line.
            Tag lastTag = (Tag)_children[_children.count - 1];
            return lastTag.add(line);
        }



        /// <summary>Get the line that represents the current value of the tag.</summary>
        /// <returns>The line that represents the current value of the tag.</returns>
        public string getLine()
        {
            if (_value == "")
            {
                return _level.ToString() + " " + _key;
            }
            return _level.ToString() + " " + _key + " " + _value;
        }



        /// <summary>Return the tag for display in gedcom format.</summary>
        public string display(int indent)
        {
            StringBuilder output = new StringBuilder();
            output.Append("".PadRight(indent));
            // output.Append(_line);
            output.Append(getLine());
            // output.Append("\r\n");
            output.Append("\n");
            foreach (Tag child in children)
            {
                output.Append(child.display(indent + 2));
            }
            return output.ToString();
        }



        /// <summary>Return the tag for writing to file and editing.</summary>
        public string toText()
        {
            StringBuilder output = new StringBuilder();
            // output.Append(_line);
            output.Append(getLine());
            // output.Append("\r\n");
            output.Append("\n");
            foreach (Tag child in children)
            {
                output.Append(child.toText());
            }
            return output.ToString();
        }



        #region Functions



        /// <summary>Return the value and any continuations as an array of strings.</summary>
        /// <returns>The complete continued value.</returns>
        public string [] getMultiLineValue()
        {
            // Create a list of lines to hold the result.
            List<string> lines = new List<string>();
            lines.Add(_value);

            // Search for continuations.
            Tag[] tagContinues = _children.findAll("CONT");
            foreach (Tag tag in tagContinues)
            {
                lines.Add(tag.value);
            }

            // Return the calculated value.
            return lines.ToArray();
        }



        /// <summary>Return the value and any continations as a jagged array of strings.</summary>
        /// <returns>The compelete continued value as a grid.</returns>
        public string [][] getGridValue()
        {
            // Create a jagged array to hold the grid.
            List<string[]> grid = new List<string[]>();

            string[] line = _value.Split(':');
            if (_value.StartsWith("GRID:"))
            {
                line = _value.Substring(5).Split(':');
            }
            grid.Add(line);

            // Search for continuations.
            Tag[] tagContinues = _children.findAll("CONT");
            foreach (Tag tag in tagContinues)
            {
                line = tag.value.Split(':');
                grid.Add(line);
            }

            // Return the calculated list.
            return grid.ToArray();
        }



        #endregion

    }
}
