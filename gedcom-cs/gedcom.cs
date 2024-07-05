using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
