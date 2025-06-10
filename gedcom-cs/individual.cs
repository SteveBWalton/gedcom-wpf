using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;

namespace gedcom
{
    /// <summary>Class to represent an individual in a gedcom.</summary>
    public class Individual : TopLevel, IComparable<Individual>
    {
        #region Member Variables

        #endregion

        #region Class Constructors



        // <summary>Empty class constructor.</summary>
        //public Individual()
        //{
        //}



        /// <summary>Empty class constructor.</summary>
        public Individual(Gedcom gedcom) : base(gedcom)
        {
            // Create an key for the individual.
            string newKey = "@I" + (gedcom.individuals.Count() + 1).ToString("0000") + "@";

            // An empty individual top level tag.
            _tag = new Tag(gedcom, newKey, "INDI");
        }



        /// <summary>Create an individual from the specified tag.</summary>
        /// <param name="tag">Specifies the tag to build the individual from.</param>
        public Individual(Tag tag) : base(tag)
        {
        }

        #endregion

        #region Properties



        /// <summary>The full name of the individual.</summary>
        public string fullName
        {
            get
            {
                Tag tagName = _tag.children.findOne("NAME");
                if (tagName == null)
                {
                    return "Error";
                }

                string fullName = tagName.value;

                // One way (not the best).
                fullName = fullName.Replace("/", "");

                // This is not really correct.
                return fullName;
            }
        }



        /// <summary>True if the individual is male.</summary>
        public bool isMale
        {
            get
            {
                Tag tagSex = _tag.children.findOne("SEX");
                if (tagSex == null)
                {
                    return true;
                }
                return tagSex.value == "M";
            }
        }



        /// <summary>True if the individual is female.</summary>
        public bool isFemale
        {
            get { return !isMale; }
        }



        /// <summary>The date of birth for the individual.</summary>
        /// <remarks>This will be null if the date of birth is unknown.</remarks>
        public TagDate dob
        {
            get
            {
                Tag tagBirth = _tag.children.findOne("BIRT");
                if (tagBirth != null)
                {
                    Tag tag = tagBirth.children.findOne("DATE");
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



        /// <summary>The age of individual at death or now.</summary>
        public string age
        {
            get
            {
                TagDate birthDate = dob;
                if (birthDate == null)
                {
                    return "";
                }
                TagDate deathDate = dod;
                DateTime endDate = ((deathDate == null) ? DateTime.Now : deathDate.approxDate);
                TimeSpan timeSpan = endDate - birthDate.approxDate;
                if (timeSpan < TimeSpan.FromDays(365))
                {
                    return timeSpan.TotalDays.ToString() + " days";
                }
                // Calculate the age in years.
                int years = endDate.Year - dob.approxDate.Year - 1;
                if (years > 110 && deathDate == null)
                {
                    // Not likely.
                    return "";
                }
                if (endDate.Month > birthDate.approxDate.Month)
                {
                    // Birthday in final year.
                    years++;
                }
                else if (endDate.Month == birthDate.approxDate.Month && DateTime.Now.Day >= birthDate.approxDate.Month)
                {
                    // Birthday in final month.
                    years++;
                }
                return years.ToString("##0");
            }
        }



        /// <summary>The location of the the individual birth.</summary>
        /// <remarks>This will be null if the place of birth is unknown.</remarks>
        public TagPlace birthPlace
        {
            get
            {
                Tag tagBirth = _tag.children.findOne("BIRT");
                if (tagBirth != null)
                {
                    Tag tag = tagBirth.children.findOne("PLAC");
                    if (tag != null)
                    {
                        TagPlace tagPlace = new TagPlace(tag);
                        return tagPlace;
                    }
                }
                // Return missing place.
                return null;
            }
        }



        /// <summary>The parent family for this individual.</summary>
        /// <remarks>This will be null if the family is not known.</remarks>
        public Family parentsFamily
        {
            get
            {
                Tag tagFamily = _tag.children.findOne("FAMC");
                if (tagFamily != null)
                {
                    string familyIdx = Tag.toIdx(tagFamily.value);
                    Family family = _tag.gedcom.families.find(familyIdx);
                    return family;
                }
                // Return family unknown.
                return null;
            }
        }



        /// <summary>The father for this individual or null if unknown.</summary>
        public Individual father
        {
            get
            {
                Family family = parentsFamily;
                if (family != null)
                {
                    return family.husband;
                }
                // Father unknown.
                return null;
            }
        }



        /// <summary>The mother for this individual or null if unknown.</summary>
        public Individual mother
        {
            get
            {
                Family family = parentsFamily;
                if (family != null)
                {
                    return family.wife;
                }
                // Mother unknown.
                return null;
            }
        }



        /// <summary>The father for this individual or empty string if unknown.</summary>
        public string fatherIdx
        {
            get
            {
                Family family = parentsFamily;
                if (family != null)
                {
                    return family.husbandIdx;
                }
                // Father unknown.
                return "";
            }
        }



        /// <summary>The mother for this individual or null if unknown.</summary>
        public string motherIdx
        {
            get
            {
                Family family = parentsFamily;
                if (family != null)
                {
                    return family.wifeIdx;
                }
                // Mother unknown.
                return "";
            }
        }



        /// <summary>The date of birth for the individual.</summary>
        /// <remarks>This will be null if the date of birth is unknown.</remarks>
        public TagDate dod
        {
            get
            {
                Tag tagDeath = _tag.children.findOne("DEAT");
                if (tagDeath != null)
                {
                    Tag tag = tagDeath.children.findOne("DATE");
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

        #region IComparable<Individual>



        /// <summary>Compare function for sorting inidividuals by last changed.</summary>
        /// <param name="otherIndividual">Specifies the individual to compare with.</param>
        /// <returns>The comparison of the last edit date of the two individuals.</returns>
        public int CompareTo(Individual otherIndividual)
        {
            return otherIndividual.lastChanged.CompareTo(lastChanged);
        }



        #endregion

        /// <summary>Get an array of indexes for families that this individual has created.</summary>
        /// <returns>An array of families that this person created.</returns>
        public string[] getFamilyIdxes()
        {
            Tag [] tagFamilies = _tag.children.findAll("FAMS");
            List<string> familyIdxes = new List<string>();
            foreach (Tag tag in tagFamilies)
            {
                familyIdxes.Add(Tag.toIdx(tag.value));
            }

            // Convert into an array.
            string[] families = familyIdxes.ToArray();

            // Sort the siblings into birth order.
            Array.Sort(families, tag.gedcom.sortFamilesByDate);

            // Return the array of siblings.
            return families;
        }



        /// <summary>Get an array of indexes for siblings for this individual.</summary>
        /// <returns>An array of indexes of siblings for this individual.</returns>
        public string[] getSiblingsIdxes()
        {
            List<string> siblingIdxes = new List<string>();
            string sharedFatherIdx = fatherIdx;
            if (sharedFatherIdx == "")
            {
                sharedFatherIdx = "NoMatch";
            }
            string sharedMotherIdx = motherIdx;
            if (sharedMotherIdx == "")
            {
                sharedMotherIdx = "NoMatch";
            }
            foreach(Individual individual in _tag.gedcom.individuals)
            {
                if ((individual.fatherIdx == sharedFatherIdx || individual.motherIdx == sharedMotherIdx) && individual.idx != idx)
                {
                    siblingIdxes.Add(individual.idx);
                }
            }

            // Convert into an array.
            string[] siblings = siblingIdxes.ToArray();

            // Sort the siblings into birth order.
            Array.Sort(siblings, _tag.gedcom.sortIndividualsByBirth); // (IComparer<string>)

            // Return the array of siblings.
            return siblings;
        }
    }
}
