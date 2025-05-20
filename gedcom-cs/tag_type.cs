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
                case "PLAC":
                    return "Place";
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
                case "OBJE":
                    return "Media";
                }

                // There is no known name, use the key.
                return _tagKey;
            }
        }

        #endregion

        /// <summary>
        /// Could this be a property instead?
        /// </summary>
        /// <returns></returns>
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
                break;
            case "EDUC":
            case "OCCU":
            case "NOTE":
                children.Add(new TagType("DATE"));
                children.Add(new TagType("PLAC"));
                break;
            }

            // For debugging and not completely wrong.
            if (_tagKey != "SOUR" && _tagKey != "CHAN" && _tagKey != "OBJE")
            {
                children.Add(new TagType("SOUR"));
            }

            // Return the child types.
            return children;
        }


    }
}
