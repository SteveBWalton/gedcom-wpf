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
                case "FAM":
                    return "Family";
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
                case "SURN":
                    return "Surname";
                case "GIVN":
                    return "Given Names";
                case "CHAN":
                    return "Last Change";
                case "OBJE":
                    return "Media";
                case "MAP":
                    return "Map";
                case "LATI":
                    return "Latitude";
                case "LONG":
                    return "Longitude";
                case "HUSB":
                    return "Husband";
                case "WIFE":
                    return "Wife";
                case "CHIL":
                    return "Child";
                case "MARR":
                    return "Marriage";
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
                case "CHIL":
                    return true;

                case "DATE":
                case "TIME":
                case "PLAC":
                case "NAME":
                case "SEX":
                case "BIRT":
                case "DEAT":
                case "FAMC":
                case "SURN":
                case "GIVN":
                case "CHAN":
                case "HUSB":
                case "WIFE":
                case "MARR":
                    return false;
                }

                // Not sure what to have as the default!
                return true;
            }
        }



        /// <summary>The priority of the tag in gedcom files.</summary>
        /// <remarks>The default / uknown priority is 1000.  The values have no significance it is just the order.</remarks>
        public int sortOrder
        {
            get
            {
                switch (_tagKey)
                {
                // These should be towards the start of the top level tag.
                case "NAME":
                case "HUSB":
                    return 10;
                case "SEX":
                case "WIFE":
                    return 20;
                case "BIRT":
                case "MARR":
                    return 30;
                case "DEAT":
                    return 40;
                case "FAMC":
                    return 50;

                case "FAMS":
                case "CHIL":
                    return 100;
                case "EDUC":
                    return 110;
                case "OCCU":
                    return 120;
                case "NOTE":
                    return 130;

                // These are sub tags that rary appear at level 1.
                case "DATE":
                    return 300;
                case "TIME":
                    return 310;
                case "PLAC":
                    return 320;
                case "GIVN":
                    return 210;
                case "SURN":
                    return 220;

                // These should be towards the end of the top level tag.
                case "OBJE":
                    return 1400;
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
            switch (_tagKey)
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
            case "FAM":
                children.Add(new TagType("HUSB"));
                children.Add(new TagType("WIFE"));
                children.Add(new TagType("CHIL"));
                children.Add(new TagType("MARR"));
                break;
            case "NAME":
                children.Add(new TagType("GIVN"));
                children.Add(new TagType("SURN"));
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
