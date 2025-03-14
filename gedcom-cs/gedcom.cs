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
        public Individuals individuals;
        /// <summary>The families in this gedcom.</summary>
        public Families families;
        /// <summary>The sources in this gedcom.</summary>
        public Sources sources;
        /// <summary>True if the gedcom has changed since the last change.</summary>
        private bool _isDirty;
        #endregion

        #region Constructors

        public Gedcom()
        {
            clear();
            _isDirty = false;
        }



        /// <summary>Initialise and empty the gedcom object.</summary>
        public void clear()
        {
            _fileName = "";
            individuals = new Individuals();
            families = new Families();
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
                                    individuals.add(individual);
                                }
                                else if (tag.line.EndsWith("FAM"))
                                {
                                    Family family = new Family(tag, this);
                                    families.add(family);
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
        public int sortIndividualsByBirth(string index1, string index2)
        {
            Individual person1 = individuals.find(index1);
            Individual person2 = individuals.find(index2);

            if (person1==null)
            {
                return -1;
            }
            if (person2==null)
            {
                return +1;
            }

            TagDate person1DoB = person1.dob;
            TagDate person2DoB = person2.dob;

            if (person1DoB==null)
            {
                return -1;
            }
            if(person2DoB==null)
            {
                return +1;
            }

            // Compare the dates of birth.
            return person1DoB.approxDate.CompareTo(person2DoB.approxDate);
        }



        public int sortFamilesByDate(string index1, string index2)
        {
            Family family1 = families.find(index1);
            Family family2 = families.find(index2);

            if(family1==null)
            {
                return -1;
            }
            if (family2==null)
            {
                return +1;
            }

            TagDate family1date = family1.marriageDate;
            TagDate family2date = family2.marriageDate;
            if (family1date==null)
            {
                return -1;
            }
            if(family2date==null)
            {
                return +1;
            }

            // Compare the marriage dates.
            return family1date.approxDate.CompareTo(family2date.approxDate);

        }

        #endregion

    }
}
