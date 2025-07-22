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
        /// <summary>The individuals in this gedcom (INDI).</summary>
        private readonly Individuals _individuals;
        /// <summary>The families in this gedcom (FAM).</summary>
        private readonly Families _families;
        /// <summary>The sources in this gedcom (SOUR).</summary>
        private readonly Sources _sources;
        /// <summary>The media objects in this gedom (OBJE).</summary>
        private readonly MediaObjects _mediaObjects;
        /// <summary>The repositories in this gedom (REPO).</summary>
        private readonly Repositories _repostories;
        /// <summary>True if the gedcom has changed since the last change.</summary>
        private bool _isDirty;

        /// <summary>The places in this gedcom.</summary>
        private readonly Places _places;

        #endregion

        #region Constructors

        /// <summary>Default constructor for empty Gedcom objects.</summary>
        public Gedcom()
        {
            _individuals = new Individuals();
            _families = new Families();
            _sources = new Sources();
            _mediaObjects = new MediaObjects();
            _repostories = new Repositories();
            _places = new Places(null);
            clear();
            _isDirty = false;
        }



        /// <summary>Initialise and empty the gedcom object.</summary>
        public void clear()
        {
            _fileName = "";
            _individuals.clear();
            _families.clear();
            _sources.clear();
            _mediaObjects.clear();
            _repostories.clear();
            _places.clear();
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



        /// <summary>The sources in this gedcom.</summary>
        public Sources sources
        {
            get => _sources;
        }



        /// <summary>The media objects in this gedcom.</summary>
        public MediaObjects mediaObjects
        {
            get => _mediaObjects;
        }



        /// <summary>The repositories in this gedcom.</summary>
        public Repositories repositories
        {
            get => _repostories;
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



        /// <summary>The places in this gedcom.</summary>
        public Places places
        {
            get => _places;
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
            Tag tag = new Tag(this);
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
                                    _individuals.add(individual);
                                }
                                else if (tag.line.EndsWith("FAM"))
                                {
                                    Family family = new Family(tag);
                                    _families.add(family);
                                }
                                else if (tag.line.EndsWith("SOUR"))
                                {
                                    Source source = new Source(tag);
                                    _sources.add(source);
                                }
                                else if (tag.line.EndsWith("OBJE"))
                                {
                                    MediaObject mediaObject = new MediaObject(tag);
                                    _mediaObjects.add(mediaObject);
                                }
                                else if (tag.line.EndsWith("REPO"))
                                {
                                    Repository repository = new Repository(tag);
                                    _repostories.add(repository);
                                }
                            }

                            // Start a new top level tag.
                            tag = new Tag(this);
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

            // Add places from the tags.
            addPlaces();

            // Mark as no save required.
            _isDirty = false;

            // Return success.
            return true;
        }



        /// <summary>Save the gedom to the specified gedcom file.</summary>
        /// <remarks>THIS DOES NOT WORK.  DO NOT WTITE OVER A GOOD GEDCOM FILE.  JUST TESTING FOR NOW.</remarks>
        /// <param name="fileName">Specifies the file name of the gedcom to save.</param>
        /// <returns>True for success, false otherwise.</returns>
        public bool save(string fileName)
        {
            using (StreamWriter streamWriter = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                // Write the gedcom file header.
                streamWriter.WriteLine("0 HEAD");
                streamWriter.WriteLine("1 SOUR gedcom-py");
                streamWriter.WriteLine("2 NAME gedcom-py");
                streamWriter.WriteLine("2 VERS 0.01.00");
                streamWriter.WriteLine("1 DEST DISKETTE");
                streamWriter.WriteLine("1 DATE " + DateTime.Now.ToString("d MMM yyyy").ToUpper());
                streamWriter.WriteLine("2 TIME " + DateTime.Now.ToString("HH:mm:ss").ToUpper());
                streamWriter.WriteLine("1 CHAR UTF-8");
                streamWriter.WriteLine("1 FILE " + Path.GetFileName(fileName));

                // Write the individuals.
                foreach (Individual individual in _individuals)
                {
                    // The toText() includes a line feed.
                    streamWriter.Write(individual.tag.toText());
                }

                // Write the families.
                foreach (Family family in  _families)
                {
                    // The toText() includes a line feed.
                    streamWriter.Write(family.tag.toText());
                }

                // Write the sources.
                foreach (Source source in _sources)
                {
                    // The toText() includes a line feed.
                    streamWriter.Write(source.tag.toText());
                }

                // Write the media objects.
                foreach (MediaObject mediaObject in _mediaObjects)
                {
                    // The toText() includes a line feed.
                    streamWriter.Write(mediaObject.tag.toText());
                }

                // Write the repositories.
                foreach (Repository repository in _repostories)
                {
                    // The toText() includes a line feed.
                    streamWriter.Write(repository.tag.toText());
                }

                // Write the gedcom file footer.
                streamWriter.WriteLine("0 TRLR");
            }

            // Clear the dirty flag.
            _isDirty = false;

            // Return success.
            return true;
        }



        /// <summary>Loop through all the tags and add the places.</summary>
        private void addPlaces()
        {
            // Loop through the individuals.
            foreach (Individual individual in _individuals)
            {
                Tag[] allTags = individual.tag.getAllTags();
                foreach (Tag tag in allTags)
                {
                    if (tag.key == "PLAC")
                    {
                        _places.addTag(tag);
                    }
                }
            }

            // Loop through the families.
            foreach (Family family in _families)
            {
                Tag[] allTags = family.tag.getAllTags();
                foreach (Tag tag in allTags)
                {
                    if (tag.key == "PLAC")
                    {
                        _places.addTag(tag);
                    }
                }
            }
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
