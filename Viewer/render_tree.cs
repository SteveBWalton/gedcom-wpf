using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom.viewer
{
    /// <summary>Class to represent trees in svg format.</summary>
    /// <remarks>This was initiaily an extension to the Render as a partial class.</remarks>
    public class RenderTree
    {
        #region Member Variables

        #region Constants

        /// <summary>The width of individuals.</summary>
        private const int INDIVIDUAL_WIDTH = 150;

        /// <summary>The height of individuals.</summary>
        private const int INDIVIDUAL_HEIGHT = 66;

        /// <summary>The horizontal space between individuals.</summary>
        /// <remarks>This was previously the horizontal space for family markers.  Please rename.</remarks>
        private const int FAMILY_WIDTH = 10;

        /// <summary>The position of the relationship (marriage) bar below individuals.</summary>
        const int BAR_POSITION = 14;

        /// <summary>The position of the relationship year between individuals.</summary>
        const int RELATIONSHIP_YEAR = 11;

        /// <summary>The vertical space between individuals.</summary>
        private const int VERTICAL_SPACE = 40;

        /// <summary>The height of of a row of individuals.</summary>
        private const int ROW_HEIGHT = INDIVIDUAL_HEIGHT + VERTICAL_SPACE;

        /// <summary>The background colour for boys.</summary>
        private const string BOY_COLOUR = "LightSkyBlue";

        /// <summary>The background colour for girls.</summary>
        private const string GIRL_COLOUR = "LightPink";

        /// <summary>The posible alternative line positions.</summary>
        private enum LinePosition
        {
            HIGHER,
            LOWER,
        }


        #endregion

        /// <summary>The gedcom to render.</summary>
        private readonly Gedcom _gedcom;

        /// <summary>The grid of individuals and families to show on the tree.</summary>
        /// <remarks>
        /// Even column positions 0,2,4 are individuals.
        /// Odd column positions 1,3,5 are the families.
        /// Rows 0 Grandparents, 1 parents, 2 individual, 3 children, 4 grand-children
        ///</remarks>
        private List<string>[] _grid;

        /// <summary>The height of lines to the familes on each row.</summary>
        /// <remarks>Do something better than an encoded string!</remarks>
        private List<TreeFamilyLine>[] _familyLines;

        /// <summary>The number of lower lines that are taken.</summary>
        private int [] _lowerLines;
        /// <summary>The number of higher lines that are taken.</summary>
        private int [] _higherLines;

        #endregion

        #region Constructors

        /// <summary>Constructor to generate a tree for an individual.</summary>
        /// <param name="individual">Specifies the individual to render the tree for.</param>
        public RenderTree(Individual individual)
        {
            // Store the parameters.
            _gedcom = individual.gedcom;

            // Generate the grid.
            getIndividualTree(individual);
        }



        /// <summary>Constructor to generate a tree for a family.</summary>
        /// <remarks>This is a much similer tree compared to an individual, just the immediate family members.</remarks>
        /// <param name="family">Specifies the family to render the tree for.</param>
        public RenderTree(Family  family)
        {
            // Store the gedcom.
            _gedcom = family.gedcom;

            // Generate the grid.
            getFamilyTree(family);
        }

        #endregion

        #region Render to svg



        /// <summary>Converts the specified grid into a tree in svg format.</summary>
        /// <param name="grid">Specifies the individuals and families in the tree.</param>
        /// <returns>A tree in svg format.</returns>
        public string getTree()
        {
            return getTree(0, 5);
        }
        /// <summary>Converts the specified grid into a small tree of parents and children only in svg format.</summary>
        /// <returns>A tree in svg format.</returns>
        public string getSmallTree()
        {
            return getTree(2, 4);
        }
        /// <summary>Converts the specified rows of the grid into a tree in svg format.</summary>
        /// <param name="startRow">Specifies the starting row of the grid.  For the full grid specify 0.</param>
        /// <param name="endRow">Specifies the ending row of the grid.  This row is not displayed.  For the full grid specify 5.</param>
        /// <returns>A tree in svg format.</returns>
        private string getTree(int startRow, int endRow)
        {
            StringBuilder html = new StringBuilder();

            int maxPeople = 1;
            for (int row = startRow; row < endRow; row++)
            {
                int people = (1 + _grid[row].Count) / 2;
                if (people > maxPeople)
                {
                    maxPeople = people;
                }
            }

            // Calculate the height and width.
            int height = ROW_HEIGHT * (endRow - startRow - 1) + INDIVIDUAL_HEIGHT;
            int width = (INDIVIDUAL_WIDTH + FAMILY_WIDTH) * (maxPeople - 1) + INDIVIDUAL_WIDTH;

            html.AppendLine("<svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\"  width=\"" + width.ToString() + "\" height=\"" + height.ToString() + "\" style=\"text-alignment: center; border: 1px solid black;\">");

            int y = 0;
            int x = 0;

            for (int row = startRow; row < endRow; row++)
            {
                x = 0;
                for (int col = 0; col < _grid[row].Count; col++)
                {
                    if (col % 2 == 0)
                    {
                        // Individual.
                        html.Append(drawIndividual(_grid[row][col], x, y));
                        x += INDIVIDUAL_WIDTH;
                    }
                    else
                    {
                        // Family.
                        html.Append(drawFamily(_grid[row][col], x, y));
                        x += FAMILY_WIDTH;
                    }
                }
                y += ROW_HEIGHT;
            }

            // Calculate the connecting lines.
            for (int row = startRow + 1; row < endRow; row++)
            {
                x = 0;
                for (int col = 0; col < _grid[row].Count; col += 2)
                {
                    if (_grid[row][col] != null && _grid[row][col] != "")
                    {
                        Individual individual = _gedcom.individuals.find(_grid[row][col]);

                        // Try and connect the individual with their parents.
                        if (individual.parentsFamily != null)
                        {
                            bool isLinked = false;

                            // Search for a family.
                            int familyColumn = -1;
                            for (int i = 1; i < _grid[row - 1].Count; i += 2)
                            {
                                if (_grid[row - 1][i] == individual.parentsFamily.idx)
                                {
                                    familyColumn = i;
                                    break;
                                }
                            }
                            if (familyColumn > 0)
                            {
                                isLinked = true;
                                getFamilyBarHeight(row - 1, col, familyColumn);
                            }

                            // Check for a father, if no family.
                            if (!isLinked)
                            {
                                // Search for a father.    
                                string fatherIdx = individual.fatherIdx;
                                if (fatherIdx != "")
                                {
                                    int fatherColumn = -1;
                                    for (int i = 0; i < _grid[row - 1].Count; i += 2)
                                    {
                                        if (_grid[row - 1][i] == fatherIdx)
                                        {
                                            fatherColumn = i;
                                            break;
                                        }
                                    }

                                    if (fatherColumn >= 0)
                                    {
                                        isLinked = true;
                                        getFamilyBarHeight(row - 1, col, fatherColumn);
                                    }
                                }
                            }
                            // Check for a mother, if no family.
                            if (!isLinked)
                            {
                                // Search for a father.    
                                string motherIdx = individual.motherIdx;
                                if (motherIdx != "")
                                {
                                    int motherColumn = -1;
                                    for (int i = 0; i < _grid[row - 1].Count; i += 2)
                                    {
                                        if (_grid[row - 1][i] == motherIdx)
                                        {
                                            motherColumn = i;
                                            break;
                                        }
                                    }

                                    if (motherColumn >= 0)
                                    {
                                        isLinked = true;
                                        getFamilyBarHeight(row - 1, col, motherColumn);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Adjust overlapping connecting lines.
            const int DEBUG_ROW = 111;
            for (int row = startRow; row < endRow; row++)
            {
                if (row >= DEBUG_ROW)
                {
                    Console.WriteLine("row " + row.ToString() + " count = " + _familyLines[row].Count.ToString());
                }

                for (int family1Index = 0; family1Index < _familyLines[row].Count; family1Index++)
                {
                    for (int family2Index = family1Index + 1; family2Index < _familyLines[row].Count; family2Index++)
                    {
                        TreeFamilyLine family1 = _familyLines[row][family1Index];
                        TreeFamilyLine family2 = _familyLines[row][family2Index];
                        if (row >= DEBUG_ROW)
                        {
                            Console.WriteLine("family " + family1Index.ToString() + " left = " + family1.leftPos.ToString() + ", right = " + family1.rightPos.ToString() + ", parent = " + family1.parentJoinPosition.ToString() + ", lineHeight = " + family1.lineHeight.ToString());
                            Console.WriteLine("family " + family2Index.ToString() + " left = " + family2.leftPos.ToString() + ", right = " + family2.rightPos.ToString() + ", parent = " + family2.parentJoinPosition.ToString() + ", lineHeight = " + family1.lineHeight.ToString());
                        }

                        if (family2.lineHeight == family1.lineHeight && family1.children.Count > 0 && family2.children.Count > 0)
                        {
                            // Does the range of children overlap.
                            if (family2.leftPos > family1.rightPos)
                            {
                                // No overlap, family to the right.
                                if (row >= DEBUG_ROW)
                                {
                                    Console.WriteLine("Family " + family2Index.ToString() + " is to right of family " + family1Index.ToString() + ".");
                                }
                            }
                            else if (family2.rightPos < family1.leftPos)
                            {
                                // No overlap, family to the left.
                                if (row >= DEBUG_ROW)
                                {
                                    Console.WriteLine("Family" + family2Index.ToString() + " is to left of family " + family1Index.ToString() + ".");
                                }
                            }
                            else
                            {
                                // Some kind of overlap.
                                family2.lineHeight -= 4;

                                if (row >= DEBUG_ROW)
                                {
                                    Console.WriteLine("family " + family1Index.ToString() + " left = " + family1.leftPos.ToString() + ", right = " + family1.rightPos.ToString() + ", parent = " + family1.parentJoinPosition.ToString() + ", lineHeight = " + family1.lineHeight.ToString());
                                    Console.WriteLine("family " + family2Index.ToString() + " left = " + family2.leftPos.ToString() + ", right = " + family2.rightPos.ToString() + ", parent = " + family2.parentJoinPosition.ToString() + ", lineHeight = " + family1.lineHeight.ToString());
                                }
                            }
                        }
                    }
                }
            }

            // Draw the connecting lines.
            y = INDIVIDUAL_HEIGHT;
            for (int row = startRow; row < endRow; row++)
            {
                foreach(TreeFamilyLine familyTree in _familyLines[row])
                {
                    // Draw line down from family / person.
                    int connectionY = y + BAR_POSITION + familyTree.lineHeight;
                    if (familyTree.parentJoinPosition % 2 == 0)
                    {
                        // Individual.
                        x = (familyTree.parentJoinPosition / 2) * (INDIVIDUAL_WIDTH + FAMILY_WIDTH) + INDIVIDUAL_WIDTH / 2 + 4;

                        html.AppendLine("<line x1=\"" + x.ToString() + "\" y1=\"" + y.ToString() + "\" x2=\"" + x.ToString() + "\" y2=\"" + connectionY.ToString() + "\" stroke=\"black\" />");
                    }
                    else
                    {
                        // Family.
                        x = ((familyTree.parentJoinPosition - 1) / 2) * (INDIVIDUAL_WIDTH + FAMILY_WIDTH) + INDIVIDUAL_WIDTH + FAMILY_WIDTH / 2;
                        html.AppendLine("<line x1=\"" + x.ToString() + "\" y1=\"" + (y + BAR_POSITION).ToString() + "\" x2=\"" + x.ToString() + "\" y2=\"" + connectionY.ToString() + "\" stroke=\"black\" />");
                    }

                    // The width of the horizontal connecting line.
                    int xMin = x;
                    int xMax = x;

                    // Draw a line up from each individual in the family.
                    foreach (int individualPos in familyTree.children)
                    {
                        x = (individualPos / 2) * (INDIVIDUAL_WIDTH + FAMILY_WIDTH) + INDIVIDUAL_WIDTH / 2;
                        html.AppendLine("<line x1=\"" + x.ToString() + "\" y1=\"" + connectionY.ToString() + "\" x2=\"" + x.ToString() + "\" y2=\"" + (y + VERTICAL_SPACE).ToString() + "\" stroke=\"black\" />");
                        if (x < xMin)
                        {
                            xMin = x;
                        }
                        if (x > xMax)
                        {
                            xMax = x;
                        }
                    }

                    // Draw a horizontal connecting line.
                    html.AppendLine("<line x1=\"" + xMin.ToString() + "\" y1=\"" + connectionY.ToString() + "\" x2=\"" + xMax.ToString() + "\" y2=\"" + connectionY.ToString() + "\" stroke=\"black\" />");
                }

                y += ROW_HEIGHT;
            }

            html.AppendLine("</svg>");

            // Return the built string.
            return html.ToString();
        }



        /// <summary>Return the svg code to draw the specified individual.</summary>
        /// <param name="idx">Specifies the index of the individal.</param>
        /// <param name="x">Specifies the x position of the individual.</param>
        /// <param name="y">Specifies the y position of the individual.</param>
        /// <param name="parentsLevel">Specifies the row of the grid to search for parents for a joint line.</param>
        /// <returns>The svg code to draw the specified individual including a final line feed.</returns>
        private string drawIndividual(string idx, int x, int y) //, int parentsLevel)
        {
            // The height of text line 1.
            const int LINE1 = 14;
            // The height of text line 2.
            const int LINE2 = 32;
            // The height of text line 3.
            const int LINE3 = 46;
            // The height of text line 4.
            const int LINE4 = 60;

            if (idx == null || idx == "")
            {
                return "";
            }

            // Find the specified individual.
            Individual individual = _gedcom.individuals.find(idx);
            if (individual == null)
            {
                return "";
            }

            // Create a string (builder) to hold the svg code.
            StringBuilder svg = new StringBuilder();

            // Add a link for the individual.
            svg.AppendLine("<a xlink:href=\"app://individual?id=" + individual.idx + "\">");

            // Show a box for the individual.
            svg.Append("<rect width=\"" + INDIVIDUAL_WIDTH.ToString() + "\" height=\"" + INDIVIDUAL_HEIGHT.ToString() + "\" x=\"" + x.ToString() + "\" y=\"" + y.ToString() + "\"");
            if (individual.isMale)
            {
                svg.Append(" fill=\"" + BOY_COLOUR + "\" />");
            }
            else
            {
                svg.Append(" rx=\"6\" ry=\"6\" fill=\"" + GIRL_COLOUR + "\" />");
            }

            // Show the individual name.
            svg.Append("<text x=\"" + (x + INDIVIDUAL_WIDTH / 2).ToString() + "\" y=\"" + (y + LINE1).ToString() + "\" text-anchor=\"middle\" font-family=\"Arial, Helvetica\" font-size=\"9pt\">");
            svg.Append(individual.fullName);
            svg.Append("</text>");

            // Show the date of birth.
            svg.Append("<text x=\"" + (x + 2).ToString() + "\" y=\"" + (y + LINE2).ToString() + "\" text-anchor=\"left\" font-family=\"Arial, Helvetica\" font-size=\"9pt\">");
            svg.Append("b. ");
            if (individual.dob != null)
            {
                svg.Append(individual.dob.getShortDate());
            }
            svg.Append("</text>");

            // Show the location of birth.
            svg.Append("<text x=\"" + (x + 2).ToString() + "\" y=\"" + (y + LINE3).ToString() + "\" text-anchor=\"left\" font-family=\"Arial, Helvetica\" font-size=\"9pt\">");
            svg.Append("b. ");
            if (individual.birthPlace != null)
            {
                svg.Append(individual.birthPlace.shortPlace);
            }
            svg.Append("</text>");

            // Show the date of death.
            svg.Append("<text x=\"" + (x + 2).ToString() + "\" y=\"" + (y + LINE4).ToString() + "\" text-anchor=\"left\" font-family=\"Arial, Helvetica\" font-size=\"9pt\">");
            if (individual.dod != null)
            {
                svg.Append("d. ");
                svg.Append(individual.dod.getShortDate());
                string age = individual.age;
                if (age != "")
                {
                    svg.Append(" (");
                    svg.Append(individual.age);
                    svg.Append(")");
                }
            }
            else
            {
                svg.Append(individual.age);
            }
            svg.Append("</text>");

            // Close the link.
            svg.AppendLine();
            svg.AppendLine("</a>");

            // Return the svg.
            return svg.ToString();
        }



        /// <summary>Return the svg code to draw the speicifed family without the individuals in the family.</summary>
        /// <param name="idx">Specifies the index of the family.</param>
        /// <param name="x">Specifies the x position of the family.</param>
        /// <param name="y">Specifies the y position of the family.</param>
        /// <returns>The svg code to draw the family without the individuals in the family.</returns>
        private string drawFamily(string idx, int x, int y)
        {
            // Check if a family is specified.
            if (idx == "")
            {
                return "";
            }

            // Find the family.
            Family family = _gedcom.families.find(idx);
            if (family == null)
            {
                return "";
            }

            // Create a string (builder) to hold the svg code.
            StringBuilder svg = new StringBuilder();

            string strokeDashArray = "";
            if (!family.isMarriage)
            {
                strokeDashArray = "stroke-dasharray=\"5,2\" ";
            }

            // Draw a relationship symbol.
            svg.Append("<line x1=\"" + (x - INDIVIDUAL_WIDTH / 2).ToString() + "\" y1=\"" + (y + INDIVIDUAL_HEIGHT).ToString() + "\" x2=\"" + (x - INDIVIDUAL_WIDTH / 2).ToString() + "\" y2=\"" + (y + INDIVIDUAL_HEIGHT + BAR_POSITION).ToString() + "\" stroke=\"black\" " + strokeDashArray + "/>");
            svg.Append("<line x1=\"" + (x + FAMILY_WIDTH + INDIVIDUAL_WIDTH / 2).ToString() + "\" y1=\"" + (y + INDIVIDUAL_HEIGHT).ToString() + "\" x2=\"" + (x + FAMILY_WIDTH + INDIVIDUAL_WIDTH / 2).ToString() + "\" y2=\"" + (y + INDIVIDUAL_HEIGHT + BAR_POSITION).ToString() + "\" stroke=\"black\" " + strokeDashArray + "/>");
            svg.Append("<line x1=\"" + (x - INDIVIDUAL_WIDTH / 2).ToString() + "\" y1=\"" + (y + INDIVIDUAL_HEIGHT + BAR_POSITION).ToString() + "\" x2=\"" + (x + FAMILY_WIDTH + INDIVIDUAL_WIDTH / 2).ToString() + "\" y2=\"" + (y + INDIVIDUAL_HEIGHT + BAR_POSITION).ToString() + "\" stroke=\"black\" " + strokeDashArray + "/>");

            // Show marriage year or relationship start.
            if (family.relationshipDate != null)
            {
                int horizontalOffset = 0;
                if (family.isDivorce)
                {
                    horizontalOffset = -25;
                }
                svg.Append("<text x=\"" + (x + FAMILY_WIDTH / 2 + horizontalOffset).ToString() + "\" y=\"" + (y + INDIVIDUAL_HEIGHT + RELATIONSHIP_YEAR).ToString() + "\" text-anchor=\"middle\" font-family=\"Arial, Helvetica\" font-size=\"8pt\">");
                svg.Append(family.relationshipDate.yearDisplay);
                svg.Append("</text>");
            }

            if (family.isDivorce)
            {
                // Show divorce symbol.
                svg.Append("<line x1=\"" + (x + FAMILY_WIDTH - 5 - 2).ToString() + "\" y1=\"" + (y + INDIVIDUAL_HEIGHT + BAR_POSITION + 5).ToString() + "\" x2=\"" + (x + FAMILY_WIDTH + 5 - 2).ToString() + "\" y2=\"" + (y + INDIVIDUAL_HEIGHT + BAR_POSITION - 5).ToString() + "\" stroke=\"black\" />");
                svg.Append("<line x1=\"" + (x + FAMILY_WIDTH - 5 + 2).ToString() + "\" y1=\"" + (y + INDIVIDUAL_HEIGHT + BAR_POSITION + 5).ToString() + "\" x2=\"" + (x + FAMILY_WIDTH + 5 + 2).ToString() + "\" y2=\"" + (y + INDIVIDUAL_HEIGHT + BAR_POSITION - 5).ToString() + "\" stroke=\"black\" />");

                // Show divorce year.
                if (family.divorceDate != null)
                {
                    svg.Append("<text x=\"" + (x + FAMILY_WIDTH / 2 + 30).ToString() + "\" y=\"" + (y + INDIVIDUAL_HEIGHT + RELATIONSHIP_YEAR).ToString() + "\" text-anchor=\"middle\" font-family=\"Arial, Helvetica\" font-size=\"8pt\">");
                    svg.Append(family.divorceDate.yearDisplay);
                    svg.Append("</text>");
                }
            }

            // Return the svg.
            return svg.ToString();
        }



        /// <summary>Get the line height for this family position.</summary>
        /// <param name="level">The level of the parents.</param>
        /// <param name="indvidualColumn">The position of the individual.</param>
        /// <param name="familyColumn">The position of the family in the grid.</param>
        /// <returns></returns>
        private int getFamilyBarHeight(int level, int indvidualColumn, int familyColumn)
        {
            foreach (TreeFamilyLine familyLine in _familyLines[level])
            {
                //string[] data = line.Split(';');
                //int dataColumn = int.Parse(data[0]);
                //if (dataColumn == familyColumn)
                if (familyLine.parentJoinPosition == familyColumn)
                {
                    // Add the individual to this family.
                    familyLine.children.Add(indvidualColumn);
                    
                    // Return the existing line height.
                    return familyLine.lineHeight;
                }
            }

            // Save for next time.
            TreeFamilyLine newFamilyLine = new TreeFamilyLine(familyColumn);
            newFamilyLine.lineHeight = (VERTICAL_SPACE - BAR_POSITION) / 2;
            newFamilyLine.children.Add(indvidualColumn);
            _familyLines[level].Add(newFamilyLine);
            // Return the line height.
            return newFamilyLine.lineHeight;
        }

        #endregion

        #region Build Tree for Individual

        /// <summary>Returns a little tree for the specified individual as a svg graphic.</summary>
        /// <param name="individual">Specifies the individual to draw the tree for.</param>
        /// <returns>A little tree for the specified individual as a svg graphic.</returns>
        private void getIndividualTree(Individual individual)
        {
            // Build a grid of individuals to show.
            _grid = new List<string>[5];
            _familyLines = new List<TreeFamilyLine>[5];
            _lowerLines = new int[5];
            _higherLines = new int[5];
            for (int i = 0; i < 5; i++)
            {
                _grid[i] = new List<string>();
                _familyLines[i] = new List<TreeFamilyLine>();
                _lowerLines[i] = 0;
                _higherLines[i] = 0;
            }

            // Add the actual individual.
            addIndividualAndPartners(2, individual, true);

            // Add the siblings.
            string[] siblingIdxes = individual.getSiblingsIdxes();
            int insertPoint = 0;
            foreach (string siblingIdx in siblingIdxes)
            {
                Individual sibling = _gedcom.individuals.find(siblingIdx);
                if (sibling.dob == null || individual.dob == null || sibling.dob.approxDate >= individual.dob.approxDate)
                {
                    // Younger siblings.
                    addIndividualAndPartners(2, sibling, false);
                }
                else
                {
                    // Older siblings.
                    addIndividualAndPartners(2, sibling, false, true, ref insertPoint);
                    // _grid[2].Insert(0, "");
                    // _grid[2].Insert(0, siblingIdx);
                }
            }

            // Add the person's parents.
            addParentsTreeGrid(1, individual, true);
        }



        /// <summary>Add the specified individual and their partners to the tree at the specified level.</summary>
        /// <param name="level">Specifies the level to add the individual on.</param>
        /// <param name="individual">Specifies the individual to add.</param>
        /// <param name="isAddChildren">Specifies true to add the children of the specified individual.</param>
        private void addIndividualAndPartners(int level, Individual individual, bool isAddChildren)
        {
            int ignore = 0;
            addIndividualAndPartners(level, individual, isAddChildren, false, ref ignore);
        }
        /// <summary>Adds the individual and all their partners to the tree.</summary>
        /// <param name="level">Specifies the level to add the individual and partners.</param>
        /// <param name="individual">Specifies the individual to add.</param>
        /// <param name="isAddChildren">Specifies true to add children of this individual.</param>
        /// <param name="isInsert">Specifies true to insert the individuals at the start of the line.</param>
        /// <param name="insertPos">Specifies the position to insert the individual.</param>
        private void addIndividualAndPartners(int level, Individual individual, bool isAddChildren, bool isInsert, ref int insertPos)
        {
            // Add the individual's husband.
            if (individual.isFemale)
            {
                string[] familyIdxes = individual.getFamilyIdxes();
                foreach (string familyIdx in familyIdxes)
                {
                    Family family = _gedcom.families.find(familyIdx);
                    if (family.husbandIdx != "")
                    {
                        if (isInsert)
                        {
                            _grid[level].Insert(insertPos, family.idx);
                            _grid[level].Insert(insertPos, family.husbandIdx);
                            insertPos += 2;
                        }
                        else
                        {
                            // Add the husband.
                            _grid[level].Add(family.husbandIdx);
                            _grid[level].Add(family.idx);
                        }
                    }
                    if (isAddChildren)
                    {
                        addChildrenTreeGrid(level + 1, family);
                    }
                }
            }

            // Add the actual individual.
            if (isInsert)
            {
                _grid[level].Insert(insertPos, individual.idx);
                insertPos += 1;
            }
            else
            {
                _grid[level].Add(individual.idx);
            }

            // Add the individual's wife.
            if (individual.isMale)
            {
                string[] familyIdxes = individual.getFamilyIdxes();
                foreach (string familyIdx in familyIdxes.Reverse())
                {
                    Family family = _gedcom.families.find(familyIdx);
                    if (family.wifeIdx != "")
                    {
                        if (isInsert)
                        {
                            _grid[level].Insert(insertPos, family.wifeIdx);
                            _grid[level].Insert(insertPos, family.idx);
                            insertPos += 2;
                        }
                        else
                        {
                            _grid[level].Add(family.idx);
                            _grid[level].Add(family.wifeIdx);
                        }
                    }
                    if (isAddChildren)
                    {
                        addChildrenTreeGrid(level + 1, family);
                    }
                }
            }

            if (isInsert)
            {
                _grid[level].Insert(insertPos, "");
                insertPos += 1;
            }
            else
            {
                _grid[level].Add("");
            }
        }



        /// <summary>Add the parents of the specified individual to the tree grid at the specified level.</summary>
        /// <param name="level">Specifies the level to add the parents to the grid.</param>
        /// <param name="individual">Specifies the individual to add the parents of.</param>
        /// <param name="isShowSiblings">Specifies true to add the sliblings of the inidividual.</param>
        private void addParentsTreeGrid(int level, Individual individual, bool isShowSiblings)
        {
            if (individual == null)
            {
                return;
            }

            // Add the father to the grid.
            if (individual.fatherIdx != "")
            {
                // Add the father to the grid.
                _grid[level].Add(individual.fatherIdx);

                // Add the parents of the father to the grid.
                if (level == 1)
                {
                    addParentsTreeGrid(0, individual.father, false);
                }

                // Add the siblings of the father to the grid.
                if (isShowSiblings)
                {
                    Individual father = _gedcom.individuals.find(individual.fatherIdx);
                    string[] siblings = father.getSiblingsIdxes();
                    foreach (string sibling in siblings.Reverse())
                    {
                        _grid[level].Insert(0, "");
                        _grid[level].Insert(0, sibling);
                    }
                }
            }

            // Add the family to the grid.
            if ((_grid[level].Count % 2) != 0)
            {
                Family family = individual.parentsFamily;
                if (family == null)
                {
                    _grid[level].Add("");
                }
                else
                {
                    _grid[level].Add(family.idx);
                }
            }

            // Add the mother to the grid.
            if (individual.motherIdx != "")
            {
                _grid[level].Add(individual.motherIdx);
                _grid[level].Add("");
                if (level == 1)
                {
                    addParentsTreeGrid(0, individual.mother, false);
                }
                // Add the siblings of the mother to the grid.
                if (isShowSiblings)
                {
                    Individual mother = _gedcom.individuals.find(individual.motherIdx);
                    string[] siblings = mother.getSiblingsIdxes();
                    foreach (string sibling in siblings)
                    {
                        _grid[level].Add(sibling);
                        _grid[level].Add("");                        
                    }
                }
            }
        }



        /// <summary>Add the children of the specified family to the tree grid at the specified level.</summary>
        /// <param name="level">Specifies the level to add the children to the grid.</param>
        /// <param name="family">Specifies the family to add the children from.</param>
        /// <param name="grid">Specifies the grid to add the children to.</param>
        private void addChildrenTreeGrid(int level, Family family)
        {
            Individual[] children = family.getChildren();

            foreach (Individual child in children)
            {
                // Add child's husbands.
                if (child.isFemale && level < 4)
                {
                    string[] familyIdxes = child.getFamilyIdxes();
                    foreach (string familyIdx in familyIdxes)
                    {
                        Family childFamily = _gedcom.families.find(familyIdx);
                        if (childFamily.husbandIdx != "")
                        {
                            _grid[level].Add(childFamily.husbandIdx);
                            _grid[level].Add(childFamily.idx);
                        }
                        if (level == 3)
                        {
                            addChildrenTreeGrid(4, childFamily);
                        }
                    }
                }

                // Add the actual child.
                _grid[level].Add(child.idx);

                // Add the child's wives.
                if (child.isMale && level < 4)
                {
                    string[] familyIdxes = child.getFamilyIdxes();
                    foreach (string familyIdx in familyIdxes)
                    {
                        Family childFamily = _gedcom.families.find(familyIdx);
                        if (childFamily.wifeIdx != "")
                        {
                            _grid[level].Add(childFamily.idx);
                            _grid[level].Add(childFamily.wifeIdx);
                        }
                        if (level == 3)
                        {
                            addChildrenTreeGrid(4, childFamily);
                        }
                    }
                }

                // Use an even number of positions.
                _grid[level].Add("");
            }
        }

        #endregion

        #region Build Tree for Family

        /// <summary>Returns a little tree for the specified family as a svg graphic.</summary>
        /// <param name="family">Specifies the family to draw the tree for.</param>
        /// <returns>A little tree for the specified family as a svg graphic.</returns>
        private void getFamilyTree(Family family)
        {
            // Create an empty grid.
            _grid = new List<string>[5];
            _familyLines = new List<TreeFamilyLine>[5];
            _lowerLines = new int[5];
            _higherLines = new int[5];
            for (int i = 0; i < 5; i++)
            {
                _grid[i] = new List<string>();
                _familyLines[i] = new List<TreeFamilyLine>();
                _lowerLines[i] = 0;
                _higherLines[i] = 0;
            }

            // Add the husband.
            _grid[2].Add(family.husbandIdx);

            // Add the family.
            _grid[2].Add(family.idx);

            // Add the wife.
            _grid[2].Add(family.wifeIdx);

            // Add the children.
            Individual[] children = family.getChildren();
            foreach(Individual child in children)
            {
                _grid[3].Add(child.idx);
                _grid[3].Add("");
            }
        }


        #endregion
    }
}
