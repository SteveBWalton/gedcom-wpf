using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// File
using System.IO;

namespace gedcom
{
    public class Gedcom
    {
        // Just for testing.
        public Tags tags_;

        public Gedcom()
        {
            // Just for testing.
            tags_ = new Tags();

            Tag tag = new Tag("One", "Two");
            tags_.add(tag);
        }

        public bool open(string fileName)
        {
            using (FileStream fileStream = File.OpenRead(fileName))
            {
                using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8, true, 1024))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }
                }
            }

            // Return success.
            return true;
        }
    }
}
