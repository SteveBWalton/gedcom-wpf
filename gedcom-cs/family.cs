using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom
{
    /// <summary>Class to represent a family, 'FAM', in a gedcom file.</summary>
    public class Family : TopLevel, IComparable<Family>
    {
        #region Member Variables

        #endregion

        #region Class Constructors

        /// <summary>Empty class constructor.</summary>
        public Family(Gedcom gedcom) : base(gedcom)
        {
            // Create an key for the individual.
            string newKey = "@F" + gedcom.families.getNextIndex() + "@";

            // An empty family top level tag.
            _tag = new Tag(gedcom, newKey, "FAM");
        }



        /// <summary>Create an individual from the specified tag.</summary>
        /// <param name="tag">Specifies the tag to build the individual from.  This is expected to be a 'FAM' tag.</param>
        public Family(Tag tag) : base(tag)
        {
        }

        #endregion

        #region Properties

        /// <summary>The index of the husband in this family or empty string.</summary>
        public string husbandIdx
        {
            get
            {
                Tag tagHusband = _tag.children.findOne("HUSB");
                if (tagHusband == null)
                {
                    return "";
                }
                return Tag.toIdx(tagHusband.value);
            }
        }



        /// <summary>The index of the wife in this family or empty string.</summary>
        public string wifeIdx
        {
            get
            {
                Tag tagWife = _tag.children.findOne("WIFE");
                if (tagWife == null)
                {
                    return "";
                }
                return Tag.toIdx(tagWife.value);
            }
        }



        /// <summary>The individual who is the husband in this family or null.</summary>
        public Individual husband
        {
            get
            {
                string idx = husbandIdx;
                if (idx == null)
                {
                    return null;
                }
                return _tag.gedcom.individuals.find(idx);
            }
        }



        /// <summary>The individual who is the wife in this family or null.</summary>
        public Individual wife

        {
            get
            {
                string idx = wifeIdx;
                if (idx == null)
                {
                    return null;
                }
                return _tag.gedcom.individuals.find(idx);
            }
        }



        /// <summary>The full name description of the family.</summary>
        public string fullName
        {
            get
            {
                string name = "";
                if (husband != null)
                {
                    name = husband.fullName;
                }

                if (wife != null)
                {
                    if (name != "")
                    {
                        name += " and ";
                    }
                    name += wife.fullName;
                }

                if (name == "")
                {
                    return "Undefined";
                }
                return name;
            }
        }



        /// <summary>True if the relationship is a marriage.</summary>
        public bool isMarriage
        {
            get
            {
                Tag tagMarriage = _tag.children.findOne("MARR");
                if (tagMarriage != null)
                {
                    return true;
                }
                return false;
            }
        }



        /// <summary>The date of the marriage or null.</summary>
        public TagDate marriageDate
        {
            get
            {
                Tag tagMarriage = _tag.children.findOne("MARR");
                if (tagMarriage != null)
                {
                    Tag tag = tagMarriage.children.findOne("DATE");
                    if (tag != null)
                    {
                        TagDate tagDate = new TagDate(tag);
                        return tagDate;
                    }
                }
                // Return an empty TagDate object.
                return null;
            }
        }



        /// <summary>The date of the relationship or null.</summary>
        /// <remarks>This will be the marriage date if available.  Otherwise the date on the family tag, if available.</remarks>
        public TagDate relationshipDate
        {
            get
            {
                TagDate tagDate = marriageDate;
                if (tagDate != null)
                {
                    return marriageDate;
                }
                Tag tag = _tag.children.findOne("DATE");
                if (tag != null)
                {
                    tagDate = new TagDate(tag);
                    return tagDate;
                }
                // Return an empty TagDate object.
                return null;
            }
        }



        /// <summary>True if the relationship has a divorce.</summary>
        public bool isDivorce
        {
            get
            {
                Tag tagDivorce = _tag.children.findOne("DIV");
                if (tagDivorce != null)
                {
                    return true;
                }
                return false;
            }
        }



        /// <summary>The date of the divorce or null.</summary>
        public TagDate divorceDate
        {
            get
            {
                Tag tagDivorce = _tag.children.findOne("DIV");
                if (tagDivorce != null)
                {
                    Tag tag = tagDivorce.children.findOne("DATE");
                    if (tag != null)
                    {
                        TagDate tagDate = new TagDate(tag);
                        return tagDate;
                    }
                }
                // Return an empty TagDate object.
                return null;
            }
        }


        #endregion

        /// <summary>The description of the family for the framework.</summary>
        /// <returns>The full name of the family.</returns>
        public override string ToString()
        {
            return fullName;
        }


        #region IComparable<Family>

        /// <summary>Impliment a compare function for sorting.</summary>
        /// <param name="otherIndividual">Specifies the individual to compare with.</param>
        /// <returns>The comparison of the last viewed date of the two familes.</returns>
        public int CompareTo(Family otherFamily)
        {
            // return otherFamily.lastChanged.CompareTo(lastChanged);
            return otherFamily.lastViewed.CompareTo(lastViewed);
        }

        #endregion

        #region Functions



        /// <summary>Returns the children in this family.</summary>
        /// <returns>The children in this family.</returns>
        public Individual[] getChildren()
        {
            // Build a list of the children.
            List<Individual> children = new List<Individual>();

            // Find the tags for the children.
            Tag[] tags = _tag.children.findAll("CHIL");
            foreach (Tag tag in tags)
            {
                Individual child = _tag.gedcom.individuals.find(Tag.toIdx(tag.value));
                if (child != null)
                {
                    children.Add(child);
                }
            }

            // Return the children found.
            return children.ToArray();
        }



        /// <summary>Apply the changes in the family object to the individuals in the gedcom.</summary>
        /// <returns>The number of changes made to individuals.</returns>
        public int commitChanges()
        {
            int numChanges = 0;

            // Identify this family.
            string keyThis = Tag.toKey(_tag.key);

            // Identify the all the children in this family.
            Individual[] familyChildren = getChildren();
            List<string> childrenKeys = new List<string>();
            foreach(Individual individual in familyChildren)
            {
                childrenKeys.Add(individual.idx);
            }

            // Loop through the individuals.
            foreach(Individual individual in _tag.gedcom.individuals)
            {
                // Check for parents.
                if (individual.idx == husbandIdx || individual.idx == wifeIdx)
                {
                    // Force this individual to have a family tag to this family.
                    bool isHasTag = false;
                    Tag[] families = individual.tag.children.findAll("FAMS");
                    foreach (Tag family in families)
                    {
                        if (family.value == keyThis)
                        {
                            isHasTag = true;
                        }
                    }
                    if (!isHasTag)
                    {
                        // Add a tag to this individual.
                        Tag tag = new Tag(individual.tag, "FAMS", keyThis);
                        individual.tag.children.add(tag);
                    }
                }
                else
                {
                    // Force this individual to not have a family tag to this family.
                    bool isHasTag = false;
                    Tag[] families = individual.tag.children.findAll("FAMS");
                    foreach (Tag family in families)
                    {
                        if (family.value == keyThis)
                        {
                            individual.tag.children.remove(family);
                        }
                    }
                }

                // Check for children.
                if (childrenKeys.Contains(individual.idx))
                {
                    // Force this individual to have this family as parents.
                    Tag parents = individual.tag.children.findOne("FAMC");
                    if (parents == null)
                    {
                        // Add a parents tag.
                        Tag tag = new Tag(_tag.gedcom, "FAMC", keyThis);
                        individual.tag.children.add(tag);
                    }
                    else if (parents.value != keyThis)
                    {
                        // Update the existing parents tag.
                        parents.value = keyThis;
                    }
                }
                else
                {
                    // Force this individual to not have this family as parents.
                    Tag parents = individual.tag.children.findOne("FAMC");
                    if (parents != null && parents.value == keyThis)
                    {
                        individual.tag.children.remove(parents);
                    }
                }
            }

            // Return the number of changes.
            return numChanges;
        }

        #endregion


    }
}
