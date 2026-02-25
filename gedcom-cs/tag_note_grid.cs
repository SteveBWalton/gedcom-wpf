using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom
{
    /// <summary>Class to represent a note grid tag.</summary>
    public class TagNoteGrid
    {
        #region Member Variables

        /// <summary>A jagged array to hold the grid.</summary>
        List<string[]> _grid;

        #endregion

        #region Constructors

        /// <summary>Constructor for a tag note grid.</summary>
        /// <param name="tag">Specifies the tag to build the tag note grid from.</param>
        public TagNoteGrid(Tag tag)
        {
            // Create a jagged array to hold the grid.
            _grid = new List<string[]>();

            string[] line = tag.value.Split(':');
            if (tag.value.StartsWith("GRID:"))
            {
                line = tag.value.Substring(5).Split(':');
            }
            _grid.Add(line);

            // Search for continuations.
            Tag[] tagContinues = tag.children.findAll("CONT");
            foreach (Tag continueTag in tagContinues)
            {
                line = continueTag.value.Split(':');
                _grid.Add(line);
            }
        }

        #endregion

        /// <summary>Return the contents of the cell at x and y.</summary>
        /// <param name="x">Specifies the x position of the required cell.</param>
        /// <param name="y">Specifies the y position of the required cell.</param>
        /// <returns></returns>
        public string getCell(int x, int y)
        {
            return _grid[x][y];
        }
    }
}
