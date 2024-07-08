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

        // Just for testing.
        public Tags tags_;

        public Gedcom()
        {
            clear();

            // Just for testing.
            tags_ = new Tags();

            //Tag tag = new Tag("One", "Two");
            //tags_.add(tag);
        }

        /// <summary>Initialise and empty the gedcom object.</summary>
        public void clear()
        {
            individuals = new Individuals();
        }

        public bool open(string fileName)
        {
            // Remove any existing data.
            clear();

            Tag block = new Tag();
            int count = 0;
            using (FileStream fileStream = File.OpenRead(fileName))
            {
                using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8, true, 1024))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (line.StartsWith("0"))
                        {
                            if (block.line != "")
                            {
                                if (block.line.EndsWith("INDI"))
                                {
                                    Individual individual = new Individual(block);
                                    individuals.add(individual);
                                }
                                // Deal with previous block.
                                // Console.WriteLine(block.line);
                            }

                            count++;
                            if (count == 3)
                            {
                                Console.WriteLine("Test Block");
                                Console.WriteLine(block.display(0));
                            }

                            // Start a new block.
                            block = new Tag();
                        }

                        block.add(line);
                    }
                }
            }
            if (block.line != "")
            {
                // Deal with final block.
            }

            // Return success.
            return true;
        }
    }
}
