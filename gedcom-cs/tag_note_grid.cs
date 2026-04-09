using System;
using System.Collections.Generic;
using System.IO;
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
        List<List<string>> _grid;

        #endregion

        #region Constructors

        /// <summary>Constructor for a tag note grid.</summary>
        /// <param name="tag">Specifies the tag to build the tag note grid from.</param>
        public TagNoteGrid(Tag tag)
        {
            // Create a jagged array to hold the grid.
            _grid = new List<List<string>>();

            string[] line = tag.value.Split(':');
            if (tag.value.StartsWith("GRID:"))
            {
                line = tag.value.Substring(5).Split(':');
            }
            _grid.Add(line.ToList<string>());

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
                            if (extraLine[i].Trim() != "")
                            {
                                line[i] += "|" + extraLine[i];
                            }
                        }
                    }
                }
                _grid.Add(line.ToList<string>());
            }
        }



        /// <summary>Constructor for a tag note grid from a string.</summary>
        /// <remarks>This is not exactly what a tag note grid was suppose to be.
        /// But it could work well for this and not damage the original usage.</remarks>
        /// <param name="text">Specifies the text to build into the tag note grid.</param>
        public TagNoteGrid(string text)
        {
            // Create a jagged array to hold the grid.
            _grid = new List<List<string>>();

            // Set the initial cells from the specified string.
            using (StringReader stringReader = new StringReader(text))
            {
                string line;
                while ((line = stringReader.ReadLine()) != null)
                {
                    string[] cells = line.Split(':');
                    _grid.Add(cells.ToList<string>());
                }
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

        #region Cells

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
            if (y >= _grid[x].Count)
            {
                return $"y = {y} is out of range";
            }

            // Return the contents of the cell.
            return _grid[x][y];
        }



        /// <summary>Set the contents of the cell at x and y.</summary>
        /// <param name="x">Specifies the x position of the required cell.</param>
        /// <param name="y">Specifies the y position of the required cell.</param>
        /// <param name="newValue">Specifies the new value for the cell.</param>
        /// <returns>True for success, false otherwise.</returns>
        public bool setCell(int x, int y, string newValue)
        {
            while (x >= _grid.Count)
            {
                // Add an extra row.
                _grid.Add(new List<string>());
            }
            while (y >= _grid[x].Count)
            {
                _grid[x].Add("");
            }

            // Set the cell value.
            _grid[x][y] = newValue;

            // Return success.
            return true;
        }

        #endregion

        #region Render

        /// <summary>Return the grid as a general html table contents.</summary>
        /// <returns>The grid as a general html table contents.</returns>
        public string toHtml()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (List<string> row in _grid)
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
                    stringBuilder.Append(cell.Replace("|", "<br/>"));
                    stringBuilder.Append("</td>");
                    column++;
                }
                stringBuilder.Append("</tr>");
            }
            return stringBuilder.ToString();
        }



        /// <summary>Return the grid as a single multi line string with ':' separators.</summary>
        /// <returns>The grid as a single multi line string with ':' separators.</returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (List<string> row in _grid)
            {
                int column = 0;
                foreach (string cell in row)
                {
                    if (column > 0)
                    {
                        stringBuilder.Append(":");
                    }
                    stringBuilder.Append(cell);
                    column++;
                }
                stringBuilder.AppendLine();
            }
            return stringBuilder.ToString();
        }

        #endregion
    }
}
