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
                Tag[] additional = continueTag.children.findAll("CONT");
                foreach (Tag more in additional)
                {
                    string[] extraLine = more.value.Split(':');
                    for (int i = 0; i < extraLine.Length; i++)
                    {
                        if (line.Length > i)
                        {
                            // line[i] += "\n" + extraLine[i];
                            line[i] += "<br/>" + extraLine[i];
                        }
                    }
                }
                _grid.Add(line);
            }
        }

        #endregion

        #region Properties

        /// <summary>The number of rows defined on the grid.</summary>
        public int numRows
        {
            get => _grid.Count;
        }

        #endregion

        /// <summary>Return the contents of the cell at x and y.</summary>
        /// <param name="x">Specifies the x position of the required cell.</param>
        /// <param name="y">Specifies the y position of the required cell.</param>
        /// <returns>The contents of the cell at x and y or an error code.</returns>
        public string getCell(int x, int y)
        {
            // Add some range checking.
            if (x >= _grid.Count)
            {
                return $"x = {x} is out of range";
            }
            if (y >= _grid[x].Length)
            {
                return $"y = {y} is out of range";
            }

            // Return the contents of the cell.
            return _grid[x][y];
        }



        /// <summary>Return the grid as a general html table contents.</summary>
        /// <returns>The grid as a general html table contents.</returns>
        public string toHtml()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string[] row in _grid)
            {
                stringBuilder.Append("<tr>");
                int column = 0;
                foreach (string cell in row)
                {
                    if (column % 2 == 0)
                    {
                        stringBuilder.Append("<td style=\"font-size: 8pt; color: grey; font-family: Tahoma;\">");
                    }
                    else
                    {
                        stringBuilder.Append("<td>");
                    }
                    stringBuilder.Append(cell);
                    stringBuilder.Append("</td>");
                    column++;
                }
                stringBuilder.Append("</tr>");
            }
            return stringBuilder.ToString();
        }
    }
}
