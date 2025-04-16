using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// File
using System.IO;
// ArrayList
using System.Collections;


namespace gedcom
{
    public class Gedcom
    {
        #region Member Variables

        /// <summary>The filename of this gedcom file.</summary>
        private string _fileName;
        /// <summary>The individuals in this gedcom.</summary>
        private readonly Individuals _individuals;
        /// <summary>The families in this gedcom.</summary>
        private readonly Families _families;
        /// <summary>The sources in this gedcom.</summary>
        public Sources sources;
        /// <summary>True if the gedcom has changed since the last change.</summary>
        private bool _isDirty;
        #endregion

        #region Constructors

        /// <summary>Default constructor for empty Gedcom objects.</summary>
        public Gedcom()
        {
            _individuals = new Individuals();
            _families = new Families();
            clear();
            _isDirty = false;
        }



        /// <summary>Initialise and empty the gedcom object.</summary>
        public void clear()
        {
            _fileName = "";
            _individuals.clear();
            _families.clear();
            sources = new Sources();
            _isDirty = false;
        }

        #endregion

        #region Properties

        /// <summary>The file name of this gedcom file.</summary>
        public string fileName
        {
            get { return _fileName; }
        }



        /// <summary>The individuals in this gedcom.</summary>
        public Individuals individuals
        {
            get => _individuals;
        }



        /// <summary>The families in this gedcom.</summary>
        public Families families
        {
            get => _families;
        }



        /// <summary>True if the gedcom has changed since the last change.</summary>
        public bool isDirty
        {
            get => _isDirty;
            set
            {
                if (value)
                {
                    _isDirty = true;
                }
                else
                {
                    throw (new Exception("Can't clear the dirty status here, sorry."));
                }
            }
        }

        #endregion

        #region File IO

        /// <summary>Load the gedcom from the specified gedcom file.</summary>
        /// <param name="fileName">Specifies the file name of the gedcom to load.</param>
        /// <returns>True for success, false otherwise.</returns>
        public bool open(string fileName)
        {
            // Remove any existing data.
            clear();

            // Save the file name.
            _fileName = fileName;

            // Read the lines in the gedcom file.
            Tag tag = new Tag();
            using (FileStream fileStream = File.OpenRead(fileName))
            {
                using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8, true, 1024))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        // Check for top level tag.
                        if (line.StartsWith("0"))
                        {
                            // Deal with the previous top level tag, if any.
                            if (tag.line != "")
                            {
                                if (tag.line.EndsWith("INDI"))
                                {
                                    Individual individual = new Individual(tag, this);
                                    _individuals.add(individual);
                                }
                                else if (tag.line.EndsWith("FAM"))
                                {
                                    Family family = new Family(tag, this);
                                    _families.add(family);
                                }
                                else if (tag.line.EndsWith("SOUR"))
                                {
                                    Source source = new Source(tag, this);
                                    sources.add(source);
                                }
                            }

                            // Start a new top level tag.
                            tag = new Tag();
                        }

                        // Add the data from the gedcom file to the current tag.
                        tag.add(line);
                    }
                }
            }
            if (tag.line != "")
            {
                // Deal with final block.
            }

            // Mark as no save required.
            _isDirty = false;

            // Return success.
            return true;
        }

        #endregion

        #region Sorting Functions

        /// <summary>Function to sort individuals by date of birth.</summary>
        /// <param name="index1">Specifies individual 1.</param>
        /// <param name="index2">Specifies individual 2.</param>
        /// <returns>+1 if individual 1 should be before individual 2, -1 if individual 2 should be before individual 1 and 0 if they should be in the same position.</returns>
        public int sortIndividualsByBirth(string index1, string index2)
        {
            Individual person1 = _individuals.find(index1);
            Individual person2 = _individuals.find(index2);

            if (person1 == null)
            {
                return -1;
            }
            if (person2 == null)
            {
                return +1;
            }

            TagDate person1DoB = person1.dob;
            TagDate person2DoB = person2.dob;

            if (person1DoB == null)
            {
                return -1;
            }
            if (person2DoB == null)
            {
                return +1;
            }

            // Compare the dates of birth.
            return person1DoB.approxDate.CompareTo(person2DoB.approxDate);
        }



        /// <summary>Function to sort familys by relationship date.</summary>
        /// <param name="index1">Specifies family 1.</param>
        /// <param name="index2">Specifies family 2.</param>
        /// <returns>+1 if family 1 should be before family 2, -1 if family 2 should be before family 1 and 0 if they should be in the same position.</returns>
        public int sortFamilesByDate(string index1, string index2)
        {
            Family family1 = _families.find(index1);
            Family family2 = _families.find(index2);

            if (family1 == null)
            {
                return -1;
            }
            if (family2 == null)
            {
                return +1;
            }

            TagDate family1date = family1.relationshipDate;
            TagDate family2date = family2.relationshipDate;
            if (family1date == null)
            {
                return -1;
            }
            if (family2date == null)
            {
                return +1;
            }

            // Compare the marriage dates.
            return family1date.approxDate.CompareTo(family2date.approxDate);
        }

        #endregion

    }
}
