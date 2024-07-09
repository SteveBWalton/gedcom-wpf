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

        #endregion

        #region Constructors

        public Gedcom()
        {
            clear();
        }

        /// <summary>Initialise and empty the gedcom object.</summary>
        public void clear()
        {
            _fileName = "";
            individuals = new Individuals();
            families = new Families();
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
                            }

                            // Start a new top level tag.
                            tag = new Tag();
                        }

                        // Add the data from the gedcom file to the tag.
                        tag.add(line);
                    }
                }
            }
            if (tag.line != "")
            {
                // Deal with final block.
            }

            // Return success.
            return true;
        }

        #endregion

        #region Properties

        /// <summary>The file name of this gedcom file.</summary>
        public string fileName
        {
            get { return _fileName; }
        }

        #endregion
    }
}
