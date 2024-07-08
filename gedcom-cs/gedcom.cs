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
        /// <summary>The individuals in this gedcom.</summary>
        public Individuals individuals;
        /// <summary>The families in this gedcom.</summary>
        public Families families;

        public Gedcom()
        {
            clear();
        }

        /// <summary>Initialise and empty the gedcom object.</summary>
        public void clear()
        {
            individuals = new Individuals();
            families = new Families();
        }

        public bool open(string fileName)
        {
            // Remove any existing data.
            clear();

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
                                    Individual individual = new Individual(tag);
                                    individuals.add(individual);
                                }
                                else if (tag.line.EndsWith("FAM"))
                                {
                                    Family family = new Family(tag);
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
    }
}
