using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom.viewer
{
    /// <summary>Extension to the Render class to draw trees via svg.</summary>
    public partial class Render
    {
        #region Member Variables

        /// <summary>The width of individuals.</summary>
        const int INDIVIDUAL_WIDTH = 150;
        
        /// <summary>The height of individuals.</summary>
        const int INDIVIDUAL_HEIGHT = 80;
        
        /// <summary>The horizontal space between individuals.</summary>
        /// <remarks>This was previously the horizontal space for family markers.  Please rename.</remarks>
        const int FAMILY_WIDTH = 10;

        /// <summary>The vertical space between individuals.</summary>
        const int VERTICAL_SPACE = 30;

        /// <summary>The height of of a row of individuals.</summary>
        const int ROW_HEIGHT = INDIVIDUAL_HEIGHT + VERTICAL_SPACE;

        #endregion

        #region Render to svg



        /// <summary>Converts the specified grid into a tree in svg format.</summary>
        /// <param name="grid">Specifies the individuals and families in the tree.</param>
        /// <returns>A tree in svg format.</returns>
        private string getTree(List<string>[] grid)
        {
            StringBuilder html = new StringBuilder();

            int maxPeople = 1;
            for (int row = 0; row < 5; row++)
            {
                int people = (1 + grid[row].Count) / 2;
                if (people > maxPeople)
                {
                    maxPeople = people;
                }
            }

            // Calculate the height and width.
            int height = ROW_HEIGHT * 5;
            int width = (INDIVIDUAL_WIDTH + FAMILY_WIDTH) * maxPeople;

            html.AppendLine("<svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\"  width=\"" + width.ToString() + "\" height=\"" + height.ToString() + "\" style=\"text-alignment: center; border: 1px solid black;\">");

            int y = 0;

            for (int row = 0; row < 5; row++)
            {
                int x = 0;
                for (int col = 0; col < grid[row].Count; col++)
                {
                    if (col % 2 == 0)
                    {
                        // Individual.
                        html.Append(drawIndividual(grid[row][col], x, y, INDIVIDUAL_WIDTH, INDIVIDUAL_HEIGHT, grid, row - 1));
                        x += INDIVIDUAL_WIDTH;
                    }
                    else
                    {
                        // Family.
                        html.Append(drawFamily(grid[row][col], x, y, FAMILY_WIDTH, INDIVIDUAL_HEIGHT, INDIVIDUAL_WIDTH));
                        x += FAMILY_WIDTH;
                    }
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
        /// <param name="width">Specifies the width of the individual.</param>
        /// <param name="height">Specifies the height of the individual.</param>
        /// <returns>The svg code to draw the specified individual including a final line feed.</returns>
        private string drawIndividual(string idx, int x, int y, int width, int height, List<string>[] grid, int parentsLevel)
        {
            const int LINE1 = 14;
            const int LINE2 = 32;
            const int LINE3 = 46;
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
            svg.Append("<rect width=\"" + width.ToString() + "\" height=\"" + height.ToString() + "\" x=\"" + x.ToString() + "\" y=\"" + y.ToString() + "\"");
            if (individual.isMale)
            {
                svg.Append(" fill=\"" + BOY_COLOUR + "\" />");
            }
            else
            {
                svg.Append(" rx=\"6\" ry=\"6\" fill=\"" + GIRL_COLOUR + "\" />");
            }

            // Show the individual name.
            svg.Append("<text x=\"" + (x + width / 2).ToString() + "\" y=\"" + (y + LINE1).ToString() + "\" text-anchor=\"middle\" font-family=\"Arial, Helvetica\" font-size=\"9pt\">");
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
            if (individual.dod != null)
            {
                svg.Append("<text x=\"" + (x + 2).ToString() + "\" y=\"" + (y + LINE4).ToString() + "\" text-anchor=\"left\" font-family=\"Arial, Helvetica\" font-size=\"9pt\">");
                svg.Append("d. ");
                svg.Append(individual.dod.getShortDate());
                svg.Append("</text>");

            }

            // Close the link.
            svg.AppendLine();
            svg.AppendLine("</a>");

            // Try and connect the individual with their parents.
            if (parentsLevel >= 0)
            {
                if (individual.parentsFamily != null)
                {
                    int familyColumn = -1;
                    for (int i = 1; i < grid[parentsLevel].Count; i += 2)
                    {
                        if (grid[parentsLevel][i] == individual.parentsFamily.idx)
                        {
                            familyColumn = i;
                            break;
                        }
                    }
                    if (familyColumn > 0)
                    {
                        svg.Append("<line x1=\"" + (x + width / 2).ToString() + "\" y1=\"" + y.ToString() + "\" x2=\"" + (x + width / 2).ToString() + "\" y2=\"" + (y - 5).ToString() + "\" stroke=\"black\" />");

                        // The 10 here is FAMILY_WIDTH.
                        int connectionX = ((familyColumn + 1) / 2) * width + ((familyColumn - 1) / 2) * 10 - 5;

                        svg.Append("<line x1=\"" + (x + width / 2).ToString() + "\" y1=\"" + (y - 5).ToString() + "\" x2=\"" + connectionX.ToString() + "\" y2=\"" + (y - 5).ToString() + "\" stroke=\"black\" />");
                        svg.AppendLine("<line x1=\"" + connectionX.ToString() + "\" y1=\"" + (y - 5).ToString() + "\" x2=\"" + connectionX.ToString() + "\" y2=\"" + (y - 15).ToString() + "\" stroke=\"black\" />");
                    }
                }
            }

            // Return the svg.
            return svg.ToString();
        }



        /// <summary>Return the svg code to draw the speicifed family without the individuals in the family.</summary>
        /// <param name="idx">Specifies the index of the family.</param>
        /// <param name="x">Specifies the x position of the family.</param>
        /// <param name="y">Specifies the y position of the family.</param>
        /// <param name="width">Specifies the width of the family.</param>
        /// <param name="height">Specifies the height of the family.</param>
        /// <returns>The svg code to draw the family without the individuals in the family.</returns>
        private string drawFamily(string idx, int x, int y, int width, int height, int individualWidth)
        {
            const int BAR_POSITION = 15;

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
            svg.Append("<line x1=\"" + (x - individualWidth / 2).ToString() + "\" y1=\"" + (y + height).ToString() + "\" x2=\"" + (x - individualWidth / 2).ToString() + "\" y2=\"" + (y + height + BAR_POSITION).ToString() + "\" stroke=\"black\" " + strokeDashArray + "/>");
            svg.Append("<line x1=\"" + (x + width + individualWidth / 2).ToString() + "\" y1=\"" + (y + height).ToString() + "\" x2=\"" + (x + width + individualWidth / 2).ToString() + "\" y2=\"" + (y + height + BAR_POSITION).ToString() + "\" stroke=\"black\" " + strokeDashArray + "/>");
            svg.Append("<line x1=\"" + (x - individualWidth / 2).ToString() + "\" y1=\"" + (y + height + BAR_POSITION).ToString() + "\" x2=\"" + (x + width + individualWidth / 2).ToString() + "\" y2=\"" + (y + height + BAR_POSITION).ToString() + "\" stroke=\"black\" " + strokeDashArray + "/>");

            // Show marriage year.
            if (family.marriageDate != null)
            {
                int horizontalOffset = 0;
                if (family.isDivorce)
                {
                    horizontalOffset = -25;
                }
                svg.Append("<text x=\"" + (x + width / 2 + horizontalOffset).ToString() + "\" y=\"" + (y + height + 12).ToString() + "\" text-anchor=\"middle\" font-family=\"Arial, Helvetica\" font-size=\"8pt\">");
                svg.Append(family.marriageDate.yearDisplay);
                svg.Append("</text>");
            }

            if (family.isDivorce)
            {
                svg.Append("<line x1=\"" + (x + width - 5 - 2).ToString() + "\" y1=\"" + (y + height + BAR_POSITION + 5).ToString() + "\" x2=\"" + (x + width + 5 - 2).ToString() + "\" y2=\"" + (y + height + BAR_POSITION - 5).ToString() + "\" stroke=\"black\" />");
                svg.Append("<line x1=\"" + (x + width - 5 + 2).ToString() + "\" y1=\"" + (y + height + BAR_POSITION + 5).ToString() + "\" x2=\"" + (x + width + 5 + 2).ToString() + "\" y2=\"" + (y + height + BAR_POSITION - 5).ToString() + "\" stroke=\"black\" />");

                // Show divorce year.
                if (family.divorceDate != null)
                {
                    svg.Append("<text x=\"" + (x + width / 2 + 25).ToString() + "\" y=\"" + (y + height + 12).ToString() + "\" text-anchor=\"middle\" font-family=\"Arial, Helvetica\" font-size=\"8pt\">");
                    svg.Append(family.divorceDate.yearDisplay);
                    svg.Append("</text>");
                }
            }

            // Return the svg.
            return svg.ToString();
        }

        #endregion

        #region Build Tree for Individual

        /// <summary>Returns a little tree for the specified individual as a svg graphic.</summary>
        /// <param name="individual">Specifies the individual to draw the tree for.</param>
        /// <returns>A little tree for the specified individual as a svg graphic.</returns>
        private string getIndividualTree(Individual individual)
        {
            // Build a grid of individuals to show.
            // Even column positions 0,2,4 are individuals.
            // Odd column positions 1,3,5 are the families.
            // Rows 0 Grandparents, 1 parents, 2 individual, 3 children, 4 grand-children
            List<string>[] grid = new List<string>[5];
            for (int i = 0; i < 5; i++)
            {
                grid[i] = new List<string>();
            }

            // Add the actual individual.
            addIndividualAndPartners(2, individual, true, grid);

            // Add the siblings.
            string[] siblingIdxes = individual.getSiblingsIdxes();
            foreach (string siblingIdx in siblingIdxes)
            {
                Individual sibling = _gedcom.individuals.find(siblingIdx);
                if (sibling.dob.approxDate >= individual.dob.approxDate)
                {
                    // Younger siblings.
                    addIndividualAndPartners(2, sibling, false, grid);
                }
                else
                {
                    // Older siblings.
                    grid[2].Insert(0, "");
                    grid[2].Insert(0, siblingIdx);
                }
            }

            // Add the person's parents.
            addParentsTreeGrid(1, individual, grid);

            // Return the tree in svg format.
            return getTree(grid);
        }



        /// <summary>Adds the individual and all their partners to the tree.</summary>
        /// <param name="level">Specifies the level to add the individual and partners.</param>
        /// <param name="individual">Specifies the individual to add.</param>
        /// <param name="grid">Specifies the grid to add the individuals to.</param>
        private void addIndividualAndPartners(int level, Individual individual, bool isAddChildren, List<string>[] grid)
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
                        grid[level].Add(family.husbandIdx);
                        grid[level].Add(family.idx);
                    }
                    if (isAddChildren)
                    {
                        addChildrenTreeGrid(level+1, family, grid);
                    }
                }
            }

            // Add the actual individual.
            grid[level].Add(individual.idx);

            // Add the individual's wife.
            if (individual.isMale)
            {
                string[] familyIdxes = individual.getFamilyIdxes();
                foreach (string familyIdx in familyIdxes)
                {
                    Family family = _gedcom.families.find(familyIdx);
                    if (family.wifeIdx != "")
                    {
                        grid[level].Add(family.idx);
                        grid[level].Add(family.wifeIdx);
                    }
                    if (isAddChildren)
                    {
                        addChildrenTreeGrid(level + 1, family, grid);
                    }
                }
            }
            grid[level].Add("");
        }



        /// <summary>Add the parents of the specified individual to the tree grid at the specified level.</summary>
        /// <param name="level">Specifies the level to add the parents to the grid.</param>
        /// <param name="individual">Specifies the individual to add the parents of.</param>
        /// <param name="grid">Specifies the grid to add the parents to.</param>
        private void addParentsTreeGrid(int level, Individual individual, List<string>[] grid)
        {
            if (individual == null)
            {
                return;
            }

            // Add the father to the grid.
            if (individual.fatherIdx != "")
            {
                grid[level].Add(individual.fatherIdx);
                if (level == 1)
                {
                    addParentsTreeGrid(0, individual.father, grid);
                }
            }

            // Add the family to the grid.
            if ((grid[level].Count % 2) != 0)
            {
                Family family = individual.parentsFamily;
                if (family == null)
                {
                    grid[level].Add("");
                }
                else
                {
                    grid[level].Add(family.idx);
                }
            }

            // Add the mother to the grid.
            if (individual.motherIdx != "")
            {
                grid[level].Add(individual.motherIdx);
                if (level == 1)
                {
                    addParentsTreeGrid(0, individual.mother, grid);
                }
            }
            grid[level].Add("");
        }



        /// <summary>Add the children of the specified family to the tree grid at the specified level.</summary>
        /// <param name="level">Specifies the level to add the children to the grid.</param>
        /// <param name="family">Specifies the family to add the children from.</param>
        /// <param name="grid">Specifies the grid to add the children to.</param>
        private void addChildrenTreeGrid(int level, Family family, List<string>[] grid)
        {
            Individual[] children = family.getChildren();

            foreach (Individual child in children)
            {
                if (child.isFemale)
                {
                    string[] familyIdxes = child.getFamilyIdxes();
                    foreach (string familyIdx in familyIdxes)
                    {
                        Family childFamily = _gedcom.families.find(familyIdx);
                        if (childFamily.husbandIdx != "")
                        {
                            grid[level].Add(childFamily.husbandIdx);
                            grid[level].Add(childFamily.idx);
                        }
                        if (level == 3)
                        {
                            addChildrenTreeGrid(4, childFamily, grid);
                        }
                    }
                }

                grid[level].Add(child.idx);

                if (child.isMale)
                {
                    string[] familyIdxes = child.getFamilyIdxes();
                    foreach (string familyIdx in familyIdxes)
                    {
                        Family childFamily = _gedcom.families.find(familyIdx);
                        if (childFamily.wifeIdx != "")
                        {
                            grid[level].Add(childFamily.idx);
                            grid[level].Add(childFamily.wifeIdx);
                        }
                        if (level == 3)
                        {
                            addChildrenTreeGrid(4, childFamily, grid);
                        }
                    }
                }

                grid[level].Add("");
            }
        }

        #endregion


    }
}
