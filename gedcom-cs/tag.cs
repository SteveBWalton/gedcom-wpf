using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom
{
    /// <summary>Class to represent a gedcom tag.</summary>
    public class Tag : IComparable<Tag>
    {
        #region Member Variables

        /// <summary>The parent tag of this tag.</summary>
        private readonly Tag _parent;
        /// <summary>The gedcom that contains this tag.</summary>
        /// <remarks>This is only for the case when _parent is null.</remarks>
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
        /// <summary>The type of the key of the tag.</summary>
        private TagType _tagType;

        #endregion

        #region Constructors

        /// <summary>Constructor for an empty tag in a gedcom..</summary>
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
            _tagType = null;
        }



        /// <summary>Constructor for an empty child tag of a parent tag.</summary>
        /// <param name="parent">Specifies the tag that that contains this tag.</param>
        public Tag(Tag parent):this(parent.gedcom)
        {
            _parent = parent;
            _level = parent.level + 1;
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
            _line = "0 " + tagKey + " " + tagValue;
        }



        /// <summary>Constructor of child tag with the initial values.</summary>
        /// <param name="parent">Specifies the tag that that contains this tag.</param>
        /// <param name="tagKey">Specifies the key of the tag.</param>
        /// <param name="tagValue">Specifies the initial value of the tag.</param>
        public Tag(Tag parent, string tagKey, string tagValue) : this(parent.gedcom, tagKey, tagValue)
        {
            _parent = parent;
            _level = parent.level + 1;
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
            get => _line;
        }



        /// <summary>The type or key of the gedcom tag.</summary>
        public string key
        {
            get => _key;
        }



        /// <summary>The value of the gedcom tag.</summary>
        public string value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;

                    if (_key == "SURN" || _key == "GIVN")
                    {
                        _parent.refreshValue();
                    }
                }
            }
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



        /// <summary>The tag type of the key.</summary>
        /// <remarks>This assumes that the key does not change after this property is first used.</remarks>
        public TagType tagType
        {
            get
            {
                if (_tagType == null)
                {
                    _tagType = new TagType(_key);
                }
                return _tagType;
            }
        }

        #endregion

        #region IComparable Interface

        /// <summary>Define a sort order for tags.</summary>
        /// <remarks>This allows a list of tags to be sorted.</remarks>
        /// <param name="other">Specifies another tag to compare against.</param>
        /// <returns>+1 if this should be before other, -1 if this should after other and zero is this should be level with other.</returns>
        public int CompareTo(Tag other)
        {
            // Validate the inputs.
            if (other == null)
            {
                return 1;
            }
            // If the tags are the same then use dates with the tags.
            if (_key == other.key)
            {
                // Do something more complex with dates inside the tags.
            }
            // Sort on the sort orders of the tag types.
            return tagType.sortOrder.CompareTo(other.tagType.sortOrder);
        }

        #endregion

        /// <summary>Returns the specified key as an index string.</summary>
        /// <param name="key">Specifies the key to convert to an index string.</param>
        /// <returns>The specified key as an index string.</returns>
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



        /// <summary>Returns the specified index as a key string.</summary>
        /// <param name="idx">Specifies the index string to convert to an key string.</param>
        /// <returns>The specified index as a key string.</returns>
        public static string toKey(string idx)
        {
            // Really expect this to be false.
            if (idx.StartsWith("@") && idx.EndsWith("@"))
            {
                return idx;
            }
            // Return the index as a key.
            return "@" + idx + "@";
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
                Tag newChild = new Tag(this);
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
            // Variable to hold the text.
            StringBuilder output = new StringBuilder();

            // Output this tag without the children.
            output.Append(getLine());
            output.Append("\n");

            // Source the tags info file order.
            Tag[] tags = children.ToArray();
            Array.Sort(tags, compareTagsFileOrder);

            // Write the tags into the text.
            foreach (Tag child in tags)
            {
                output.Append(child.toText());
            }

            // Return the built text.
            return output.ToString();
        }



        /// <summary>Compare function to sort tags by file order.</summary>
        /// <param name="tag1">Specifies the first tag.</param>
        /// <param name="tag2">Specifies the second tag.</param>
        /// <returns></returns>
        public static int compareTagsFileOrder(Tag tag1, Tag tag2)
        {
            TagType tagType1 = new TagType(tag1._key);
            TagType tagType2 = new TagType(tag2._key);

            return tagType1.sortOrder.CompareTo(tagType2.sortOrder);
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
        public TagNoteGrid getTagNoteGrid()
        {
            TagNoteGrid tagNoteGrid = new TagNoteGrid(this);
            return tagNoteGrid;

            /*
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
            */
        }



        /// <summary>Child tags call this when they want their parent tag to update its value.</summary>
        /// <returns>True for a value change false otherwise.</returns>
        private bool refreshValue()
        {
            if (_key == "NAME")
            {
                string newName = "";
                Tag givenNames = _children.findOne("GIVN");
                if (givenNames != null)
                {
                    newName = givenNames.value;
                }

                Tag surName = _children.findOne("SURN");
                if (surName != null)
                {
                    newName += " /" + surName.value + "/";
                }

                if (newName !=_value)
                {
                    _value = newName;
                    return true;
                }
            }

            // Return no change.
            return false;
        }



        /// <summary>Get all the child tags of this tag.</summary>
        /// <returns>List of all the child tags of this tag.</returns>
        public Tag[] getAllTags()
        {
            // Build  a list of all the child tag.
            List<Tag> allTags = new List<Tag>();

            // Add this tag to
            allTags.Add(this);

            // Add all the children tags.
            foreach (Tag child in _children)
            {
                allTags.AddRange(child.getAllTags());
            }

            // Return the list of child tags.
            return allTags.ToArray();
        }

        #endregion

    }
}
