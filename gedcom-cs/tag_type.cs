using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom
{
    /// <summary>Class to represent the a type of gedcom tag.</summary>
    public class TagType
    {
        #region Member Variables

        /// <summary>The actual tag string that will appear in a gedcom file.</summary>
        private string _tagKey;

        // <summary>
        // The collection of child tag types that this tag type can own.
        // </summary>
        // private List<TagType> _children;

        #endregion

        #region Constructors

        public TagType(string tagKey)
        {
            _tagKey = tagKey;
        }

        #endregion

        #region Properties

        /// <summary>The actual tag string that will appear in a gedcom file.</summary>
        public string tagKey
        {
            get => _tagKey;
        }



        /// <summary>The human readable name of this tag type.</summary>
        public string tagName
        {
            get
            {
                switch (_tagKey)
                {
                case "INDI":
                    return "Individual";
                case "SOUR":
                    return "Source";
                case "DATE":
                    return "Date";
                case "TIME":
                    return "Time";
                case "PLAC":
                    return "Place";
                case "NAME":
                    return "Name";
                case "SEX":
                    return "Sex";
                case "BIRT":
                    return "Birth";
                case "DEAT":
                    return "Death";
                case "FAMC":
                    return "Family Parents";
                case "FAMS":
                    return "Family Marriage";
                case "EDUC":
                    return "Education";
                case "OCCU":
                    return "Occupation";
                case "NOTE":
                    return "Note";
                case "CHAN":
                    return "Last Change";
                case "OBJE":
                    return "Media";
                case "_TODO":
                    return "To Do";
                }

                // There is no known name, use the key.
                return _tagKey;
            }
        }



        /// <summary>True if mulitple tags of this type are allowed at the same level, false if only 1 is allowed per level.</summary>
        public bool isMultiple
        {
            get
            {
                switch (_tagKey)
                {
                case "INDI":
                case "SOUR":
                case "FAMS":
                case "EDUC":
                case "OCCU":
                case "NOTE":
                case "OBJE":
                case "_TODO":
                    return true;

                case "DATE":
                case "TIME":
                case "PLAC":
                case "NAME":
                case "SEX":
                case "BIRT":
                case "DEAT":
                case "FAMC":
                case "CHAN":
                    return false;
                }

                // Not more what to have as the default!
                return true;
            }
        }



        /// <summary>The priority of the tag in gedcom files.</summary>
        /// <remarks>The default / uknown priority is 1000.</remarks>
        public int sortOrder
        {
            get
            {
                switch (_tagKey)
                {
                case "SOUR":
                    return 1500;
                case "_TODO":
                    return 1600;
                case "CHAN":
                    return 2000;
                }

                // Default
                return 1000;
            }
        }

        #endregion

        /// <summary>The default string representation of the tag type.</summary>
        /// <returns>The name of the tag type.</returns>
        public override string ToString()
        {
            return tagName;
        }



        /// <summary>
        /// The tag types that are valid children of this tag type.
        /// Could this be a property instead?
        /// </summary>
        /// <returns>A list of tag types that are valid children of this tag type.</returns>
        public List<TagType> getChildren()
        {
            List<TagType> children = new List<TagType>();
            switch(_tagKey)
            {
            case "INDI":
                children.Add(new TagType("NAME"));
                children.Add(new TagType("SEX"));
                children.Add(new TagType("BIRT"));
                children.Add(new TagType("DEAT"));
                children.Add(new TagType("FAMC"));
                children.Add(new TagType("FAMS"));
                children.Add(new TagType("EDUC"));
                children.Add(new TagType("OCCU"));
                children.Add(new TagType("NOTE"));
                children.Add(new TagType("_TODO"));
                children.Add(new TagType("CHAN"));
                break;
            case "EDUC":
            case "OCCU":
            case "NOTE":
            case "BIRT":
            case "DEAT":
                children.Add(new TagType("DATE"));
                children.Add(new TagType("PLAC"));
                break;
            case "DATE":
                children.Add(new TagType("TIME"));
                break;
            }

            // For debugging and not completely wrong.
            if (_tagKey != "SOUR" && _tagKey != "CHAN" && _tagKey != "OBJE" && _tagKey != "_TODO")
            {
                children.Add(new TagType("SOUR"));
            }

            // Return the child types.
            return children;
        }
    }
}
