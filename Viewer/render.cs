using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// This requires an additional reference to System.Web in references.
// HttpUtility
using System.Web;
// NameValueCollection.
using System.Collections.Specialized;

namespace gedcom.viewer
{
    /// <summary>Class to render content.</summary>
    public class Render
    {
        #region Member Variables

        /// <summary>The gedcom to render.</summary>
        private Gedcom _gedcom;
        /// <summary>The user options.</summary>
        private UserOptions _userOptions;
        
        #endregion

        #region Class Constructors

        /// <summary>Class constructor.</summary>
        /// <param name="gedcom">Specifies the gedcom that this will render.</param>
        public Render(Gedcom gedcom,UserOptions userOptions)
        {
            _gedcom = gedcom;        
            _userOptions = userOptions;
        }

        #endregion

        #region Properties

        #endregion

        #region Supporting Functions



        /// <summary>Show all the tags in the collection that are not in the dealt with collection.</summary>
        /// <param name="html">Specifies the html string to add to.</param>
        /// <param name="tags">Specifies the full collection of tags.</param>
        /// <param name="dealtWith">Specifies the tag keys that are already dealt with.</param>
        /// <returns>The number of tags that were not dealt with.</returns>
        private int addRemainingTags(StringBuilder html, Tags tags, List<String> dealtWith)
        {
            int count = 0;
            foreach (Tag tag in tags)
            {
                if (!dealtWith.Contains(tag.key))
                {
                    // Show this key because it has not been dealt with.
                    html.AppendLine("<p>'" + tag.key + "' = '" + tag.value + "'</p>");
                    count++;
                }
            }
            // Return the number of tags.
            return count;
        }



        /// <summary>Returns the long description of a date tag in html and adds the sources.</summary>
        /// <param name="tag">Specifies the date tag to return.</param>
        /// <param name="htmlSources">Specifies the current source references.  Returns with the additonal sources refereneces used in the description.</param>
        /// <returns></returns>
        private string getTagLongDate(Tag tag, HtmlSources htmlSources)
        {
            TagDate tagDate = new TagDate(tag);
            return getTagLongDate(tagDate, htmlSources);
        }
        /// <summary>Returns the long description of a date tag in html and adds the sources.</summary>
        /// <param name="tagDate">Specifies the date tag to return.</param>
        /// <param name="htmlSources">Specifies the current source references.  Returns with the additonal sources refereneces used in the description.</param>
        /// <returns>The long description of the date tag in html with the sources referenced.</returns>
        private string getTagLongDate(TagDate tagDate, HtmlSources htmlSources)
        {
            StringBuilder html = new StringBuilder();

            html.Append(tagDate.getLongDate());

            // Show the sources.
            html.Append(addSourceReferences(tagDate.tag, htmlSources));

            // Return the long date description.
            return html.ToString();
        }



        /// <summary>Returns references to the sources on the specified tag.</summary>
        /// <param name="tag">Specifies the tag to add the sources from.</param>
        /// <param name="htmlSources">Specifies and returns the source references.</param>
        /// <returns></returns>
        private string addSourceReferences(Tag tag, HtmlSources htmlSources)
        {
            // Build a source reference.
            StringBuilder html = new StringBuilder();

            // Show the sources.
            Tag[] sources = tag.children.findAll("SOUR");
            foreach (Tag sourceTag in sources)
            {
                string sourceIdx = Tag.toIdx(sourceTag.value);
                Source source = _gedcom.sources.find(sourceIdx);
                int refIdx = htmlSources.add(source);
                html.Append("<sup>");
                html.Append(Convert.ToChar('A' + refIdx));
                html.Append("</sup>");
            }
            
            // Return the source references.
            return html.ToString();
        }



        /// <summary>Returns the long description of a place tag in html.</summary>
        /// <param name="tag">Specifies the place tag to return.</param>
        /// <param name="htmlSources">Specifies and returns the source references.</param>
        /// <returns>The long description of a place tag in html.</returns>
        private string getTagLongPlace(Tag tag, HtmlSources htmlSources)
        {
            // Build a long description of the date tag.
            StringBuilder html = new StringBuilder();

            // Examine the date value.
            string placeValue = tag.value;

            // Default date.
            // html.Append("at ");
            html.Append("in ");

            Tag tagAddress = tag.children.findOne("ADDR");
            if (tagAddress != null)
            {
                placeValue = tagAddress.value + ", " + placeValue;
            }

            // Add the full place information.
            html.Append(placeValue);

            // Show the sources.
            html.Append(addSourceReferences(tag, htmlSources));

            // Return the long place description.
            return html.ToString();
        }



        /// <summary>Return a long html description of the specified tag.</summary>
        /// <param name="tag">Specifies the tag to describe.</param>
        /// <param name="proNoun">Specifies the pronoun to use in the description.</param>
        /// <param name="verb">Specifies the verb to use in the description.  Lookup from tag in future?</param>
        /// <param name="htmlSources">Specifies and returns the source references.</param>
        /// <returns>A long html description of the specified tag.</returns>
        private string getTagLongHtml(Tag tag, string proNoun, string verb, HtmlSources htmlSources)
        {
            // Build a long description of the tag.
            StringBuilder html = new StringBuilder();

            // Show any date information.
            Tag tagDate = tag.children.findOne("DATE");
            if (tagDate != null)
            {
                html.Append(firstCaps(getTagLongDate(tagDate, htmlSources)));
                html.Append(" ");
                html.Append(proNoun);
            }
            else 
            {
                html.Append(firstCaps(proNoun));
            }
            html.Append(" ");
            html.Append(verb);

            if (tag.value.Length > 1)
            {
                html.Append(" ");
                html.Append(tag.value);
            }


            // Show any place information.
            Tag tagPlace = tag.children.findOne("PLAC");
            if (tagPlace != null)
            {
                html.Append(" ");
                html.Append(getTagLongPlace(tagPlace, htmlSources));
            }

            // Show the sources.
            html.Append(addSourceReferences(tag, htmlSources));

            // Finish the long description.
            html.Append(". ");
            
            // Return the long description.
            return html.ToString();
        }



        /// <summary>Return the specified text with the first character as a capital.</summary>
        /// <param name="text">Specifies the text to add a capital to.</param>
        /// <returns>The specified text with the first character as a capital.</returns>
        private string firstCaps(string text)
        {
            return text.Substring(0, 1).ToUpper() + text.Substring(1);
        }

        #endregion

        #region Content

        /// <summary>Redner the requested host and query as html.</summary>
        /// <param name="host">Specifies the request host. This is usually just the name of page.</param>
        /// <param name="query">Specifies the request query. This is usually just the parameters for the page.</param>
        /// <returns>The requested page as html.</returns>
        public PageContent getContent(string host, string query)
        {
            if (host == "home")
            {
                return getHome();
            }
            else if (host == "individual")
            {
                return getIndividual(query);
            }
            else if (host == "family")
            {
                return getFamily(query);
            }
            else if (host == "source")
            {
                return getSource(query);
            }
            return getError(host, query);
        }



        /// <summary>Render the home page in html.</summary>
        /// <returns>The home page in html.</returns>
        private PageContent getHome()
        {
            PageContent pageContent = new PageContent();

            pageContent.html.Append("<h1>" + _gedcom.fileName + "</h1>");

            // Display the individuals.
            pageContent.html.Append("<fieldset style=\"width: 400px; display: inline-block; vertical-align: top;\">");
            pageContent.html.Append("<legend>Individuals</legend>");
            pageContent.html.Append("<table>");
            int count = 0;
            Individual[] individualsInDateOrder = _gedcom.individuals.inDateOrder();
            foreach (Individual individual in individualsInDateOrder)
            {
                pageContent.html.Append("<tr><td>" + htmlIndividual(individual) + "</td><tr>");
                count++;
                if (count >= 10)
                {
                    break;
                }
            }
            pageContent.html.Append("</table>");
            pageContent.html.Append("<p>There are " + _gedcom.individuals.count.ToString() + " individuals.");
            pageContent.html.Append("</fieldset>");

            // Display the families.
            pageContent.html.Append("<fieldset style=\"width: 400px; display: inline-block; vertical-align: top;\">");
            pageContent.html.Append("<legend>Families</legend>");
            pageContent.html.Append("<table>");
            count = 0;
            Family[] familiesInDateOrder = _gedcom.families.inDateOrder();
            foreach (Family family in familiesInDateOrder)
            {
                pageContent.html.Append("<tr><td><a href=\"app://family?id=" + family.idx + "\">" + family.fullName + "</a></td></tr>");
                count++;
                if (count >= 10)
                {
                    break;
                }
            }
            pageContent.html.Append("</table>");
            pageContent.html.Append("<p>There are " + _gedcom.families.count.ToString() + " families.");
            pageContent.html.Append("</fieldset>");

            // Display the sources.
            pageContent.html.Append("<fieldset style=\"width: 400px; display: inline-block; vertical-align: top;\">");
            pageContent.html.Append("<legend>Sources</legend>");
            pageContent.html.Append("<table>");
            count = 0;
            Source[] sourcesInDateOrder = _gedcom.sources.inDateOrder();
            foreach (Source source in sourcesInDateOrder)
            {
                pageContent.html.Append("<tr><td>" + htmlSource(source) + "</td></tr>");
                count++;
                if (count >= 10)
                {
                    break;
                }
            }
            pageContent.html.Append("</table>");
            pageContent.html.Append("<p>There are " + _gedcom.sources.count.ToString() + " sources.");
            pageContent.html.Append("</fieldset>");

            // Return the built string as html.
            // return _userOptions.renderHtml(html.ToString());
            return pageContent;
        }



        /// <summary>Render an error in html.</summary>
        /// <param name="host">Specifies request host.</param>
        /// <param name="query">Specifies the request query.</param>
        /// <returns>An error message in html format.</returns>
        private PageContent getError(string host, string query)
        {
            PageContent pageContent = new PageContent();
            // StringBuilder html = new StringBuilder();

            pageContent.html.Append("<h1>Error</h1>");
            pageContent.html.Append("<p>host is '" + host + "', query is '" + query + "'</p>");
            pageContent.html.Append("<p><a href=\"app://home\">Home</a></p>");

            // Return the built string as html.
            // return _userOptions.renderHtml(html.ToString());
            return pageContent;
        }

        #endregion

        #region Individual

        /// <summary>Render the specified individual in html.</summary>
        /// <param name="query">Specifies the request query for this individual.</param>
        /// <returns>A html description of the specified individual.</returns>
        private PageContent getIndividual(string query)
        {
            // Get the index of the individual.
            NameValueCollection queryParams = HttpUtility.ParseQueryString(query);
            string idx = queryParams.Get("id");

            // Start a html page for the individual.
            // StringBuilder html = new StringBuilder();
            PageContent pageContent = new PageContent();

            // Setup the edit options.
            pageContent.editForm = "individual?id=" + idx;
            pageContent.editGedomDirectly = "individual?id=" + idx;

            // Find the specified individual.
            Individual individual = _gedcom.individuals.find(idx);
            if (individual == null)
            {
                pageContent.html.AppendLine("<h1>Individual</h1>");
                pageContent.html.AppendLine("<p>query is '" + query + "'.</p>");
                pageContent.html.AppendLine("<p>Can not find '" + idx + "'.</p>");
            }
            else
            {
                // Remember which keys are dealt with.
                List<String> dealtWith = new List<String>();

                // Title the page with the individual name.
                string fullName = individual.fullName;
                dealtWith.Add("NAME");
                dealtWith.Add("SEX");
                pageContent.html.AppendLine("<h1>" + fullName + " (" + individual.idx + ")</h1>");

                // Little family tree control.
                RenderTree renderTree = new RenderTree(individual, _gedcom);
                pageContent.html.Append(renderTree.getTree());

                // Initialise the sources referenced in this individual.
                HtmlSources htmlSources = new HtmlSources();

                pageContent.html.AppendLine("<p>");
                dealtWith.Add("BIRT");
                Tag tag = individual.tag.children.findOne("BIRT");
                if (tag != null)
                {
                    pageContent.html.Append(getTagLongHtml(tag, fullName, "was born", htmlSources));
                }

                // Deal with family.
                dealtWith.Add("FAMC");
                tag = individual.tag.children.findOne("FAMC");
                if (tag != null)
                {
                    pageContent.html.Append(getTagLongParents(individual, tag, htmlSources));
                }

                // Deal with partners.
                dealtWith.Add("FAMS");
                string[] relationships = individual.getFamilyIdxes();
                // Tag[] tags = individual.tag.children.findAll("FAMS");
                // foreach (Tag marriageTag in tags)
                foreach(string relationship in relationships)
                {
                    // pageContent.html.Append(getTagLongPartner(individual, marriageTag, htmlSources));
                    pageContent.html.Append(getTagLongPartner(individual, relationship, htmlSources));
                }

                // Deal with education.
                dealtWith.Add("EDUC");
                Tag[] tags = individual.tag.children.findAll("EDUC");
                foreach (Tag educationTag in tags)
                {
                    pageContent.html.Append(getTagLongHtml(educationTag, individual.isMale ? "he" : "she", "was educated at", htmlSources));
                }

                // Deal with occupation.
                dealtWith.Add("OCCU");
                tags = individual.tag.children.findAll("OCCU");
                foreach (Tag occupationTag in tags)
                {
                    pageContent.html.Append(getTagLongHtml(occupationTag, individual.isMale ? "he" : "she", "worked as", htmlSources));
                }

                // Deal with death.
                dealtWith.Add("DEAT");
                tag = individual.tag.children.findOne("DEAT");
                if (tag != null)
                {
                    pageContent.html.Append(getTagLongHtml(tag, individual.isMale ? "he" : "she", "died", htmlSources));
                }

                pageContent.html.AppendLine("</p>");

                // Deal with the sources.
                dealtWith.Add("SOUR");
                tags = individual.tag.children.findAll("SOUR");
                foreach (Tag sourceTag in tags)
                {
                    string sourceIdx = Tag.toIdx(sourceTag.value);
                    Source source = _gedcom.sources.find(sourceIdx);
                    int refIdx = htmlSources.add(source);
                }

                // Show the remaining tags.
                dealtWith.Add("CHAN");
                addRemainingTags(pageContent.html, individual.tag.children, dealtWith);

                // Show the source references.
                pageContent.html.Append(htmlSources.toHtml());

                // Show the last changed information.
                pageContent.html.Append("<p>Last Changed " + individual.lastChanged.ToString() + "</p>");

                // Show the original gedcom.
                pageContent.html.Append("<pre style=\"width: 400px; display: inline-block; vertical-align: top;\">" + individual.tag.display(0) + "</pre>");
            }

            // Return the built string as html.
            // return _userOptions.renderHtml(html.ToString());
            return pageContent;
        }



        /// <summary>Returns the full name of the individual in html with a link.</summary>
        /// <param name="individual">Specifies the individual to display.</param>
        /// <returns>The html for the individual with the full name and a link.</returns>
        private string htmlIndividual(Individual individual)
        {
            if (individual==null)
            {
                return "Error";
            }
            return "<a href=\"app://individual?id=" + individual.idx + "\">" + individual.fullName + "</a>";
        }



        /// <summary>Describe the parents of the specified individual.</summary>
        /// <param name="individual">Specifies the individual.</param>
        /// <param name="parentsTag">Specifies the tag that gives the family of the parents.</param>
        /// <param name="htmlSources">Specifies and returns the source references.</param>
        /// <returns>A long description of the parents of the specified individual.</returns>
        private string getTagLongParents(Individual individual, Tag parentsTag, HtmlSources htmlSources)
        {
            // Find the family tag.
            string familyIdx = Tag.toIdx(parentsTag.value);
            Family family = _gedcom.families.find(familyIdx);
            Tag tag = family.tag;

            // Build a long description of the tag.
            StringBuilder html = new StringBuilder();

            if (family.husband != null)
            {
                html.Append(individual.isMale ? "His" : "Her");
                html.Append(" <a href=\"app://family?id=" + familyIdx + "\">father</a> was ");
                html.Append(htmlIndividual(family.husband));
                // Show the sources.
                html.Append(addSourceReferences(tag, htmlSources));
                html.Append(". ");
            }
            if (family.wife != null)
            {
                html.Append(individual.isMale ? "His" : "Her");
                html.Append(" <a href=\"app://family?id=" + familyIdx + "\">mother</a> was ");
                html.Append(htmlIndividual(family.wife));
                // Show the sources.
                html.Append(addSourceReferences(tag, htmlSources));
                // Finish the long description.
                html.Append(". ");
            }

            // Return the long description.
            return html.ToString();
        }



        /// <summary>Describe the relationship for the specified individual.</summary>
        /// <param name="individual">Specifies the individual.</param>
        /// <param name="familyTag">Specifies the family to describe.</param>
        /// <param name="proNoun"></param>
        /// <param name="htmlSources">Specifies and returns the source references.</param>
        /// <returns>A long description of the family for the specified individual.</returns>
        // private string getTagLongPartner(Individual individual, Tag familyTag, HtmlSources htmlSources)
        private string getTagLongPartner(Individual individual, string familyIdx, HtmlSources htmlSources)
        {
            // Find the family tag.
            Family family = _gedcom.families.find(familyIdx);
            Tag tag = family.tag;

            bool isBothPartnersKnown = true;
            if (family.wifeIdx == "" || family.husbandIdx == "")
            {
                isBothPartnersKnown = false;
            }

            // Build a long description of the tag.
            StringBuilder html = new StringBuilder();

            // Marriage or relationship.
            Tag tagMarriage = tag.children.findOne("MARR");
            if (isBothPartnersKnown)
            {
                if (tagMarriage != null)
                {
                    Tag tagDate = tagMarriage.children.findOne("DATE");
                    if (tagDate != null)
                    {
                        html.Append(firstCaps(getTagLongDate(tagDate, htmlSources)));
                        html.Append(" " + (individual.isMale ? "he" : "she") + " <a href=\"app://family?id=" + familyIdx + "\">married</a> ");
                    }
                    else
                    {
                        html.Append((individual.isMale ? "He" : "She") + " <a href=\"app://family?id=" + familyIdx + "\">married</a> ");
                    }
                }
                else
                {
                    TagDate relationshipDate = family.relationshipDate;
                    if (relationshipDate == null)
                    {
                        html.Append((individual.isMale ? "He" : "She") + " had a <a href=\"app://family?id=" + familyIdx + "\">relationship with</a> ");
                    }
                    else
                    {
                        html.Append(firstCaps(getTagLongDate(relationshipDate, htmlSources)));
                        html.Append(" " + (individual.isMale ? "he" : "she") + " had a <a href=\"app://family?id=" + familyIdx + "\">relationship with</a> ");
                    }
                }

                if (individual.idx == family.husbandIdx)
                {
                    html.Append(htmlIndividual(family.wife));
                }
                if (individual.idx == family.wifeIdx)
                {
                    html.Append(htmlIndividual(family.husband));
                }

                // Show the sources.
                html.Append(addSourceReferences(tag, htmlSources));

                // Show any place information.
                if (tagMarriage != null)
                {
                    Tag tagPlace = tagMarriage.children.findOne("PLAC");
                    if (tagPlace != null)
                    {
                        html.Append(" ");
                        html.Append(getTagLongPlace(tagPlace, htmlSources));
                    }
                }

                // Finish the long description.
                html.Append(". ");
            }

            // Describe any children.
            Individual[] children = family.getChildren();
            if (isBothPartnersKnown)
            {
                html.Append("They");
            }
            else
            {
                if (family.husbandIdx == "")
                {
                    html.Append("She");
                }
                else
                {
                    html.Append("He");
                }
            }
            if (children.Length == 0)
            {
                html.Append(" had no children");
            }
            else if (children.Length == 1)
            {
                html.Append(" had 1 child");
            }
            else
            {
                html.Append(" had " + children.Length.ToString() + " children");
            }
            int count = 1;
            foreach(Individual child in children)
            {
                if (count == children.Length && children.Length != 1)
                {
                    html.Append(" and ");
                }
                else
                {
                    html.Append(", ");
                }
                html.Append(htmlIndividual(child));
                count++;
            }

            // Finish the children description.
            html.Append(". ");

            // Divorce or relationship end.
            if (isBothPartnersKnown)
            {
                if (family.isDivorce)
                {
                    if (family.isMarriage)
                    {
                        html.Append("They divorced");
                    }
                    else
                    {
                        html.Append("They separated");
                    }
                    TagDate endDate = family.divorceDate;
                    if (endDate != null)
                    {
                        html.Append(" ");
                        html.Append(endDate.getLongDate());
                    }
                    html.Append(". ");
                }
            }

            // Return the long description.
            return html.ToString();
        }

        #endregion

        #region Family

        /// <summary>Render the specified family in html.</summary>
        /// <param name="query">Specifies the request query for this family.</param>
        /// <returns>A html description of the specified family.</returns>
        private PageContent getFamily(string query)
        {
            PageContent pageContent = new PageContent();
            pageContent.html.Append("<p><a href=\"app://home\">Home</a></p>");

            NameValueCollection queryParams = HttpUtility.ParseQueryString(query);
            string idx = queryParams.Get("id");

            Family family = _gedcom.families.find(idx);
            if (family == null)
            {
                pageContent.html.Append("<h1>Family</h1>");
                pageContent.html.Append("<p>query is '" + query + "'</p>");
                pageContent.html.Append("<p>Can't find '" + idx + "'.</p>");
                pageContent.html.Append(idx + " not found!");
            }
            else
            {
                // Remember which keys are dealt with.
                List<String> dealtWith = new List<String>();

                // Add a title for this family page.
                pageContent.html.Append("<h1>" + family.fullName + " (" + family.idx + ")</h1>");
                dealtWith.Add("HUSB");
                dealtWith.Add("WIFE");

                // Initialise the sources referenced in this family.
                HtmlSources htmlSources = new HtmlSources();

                Tag tagMarriage = family.tag.children.findOne("MARR");
                dealtWith.Add("MARR");
                if (tagMarriage != null)
                {
                    Tag tagDate = tagMarriage.children.findOne("DATE");
                    if (tagDate != null)
                    {
                        pageContent.html.Append(firstCaps(getTagLongDate(tagDate, htmlSources)));
                        pageContent.html.Append(" " + htmlIndividual(family.husband) + " married ");
                    }
                    else
                    {
                        pageContent.html.Append(htmlIndividual(family.husband) + " married ");
                    }
                }
                else
                {
                    pageContent.html.Append(htmlIndividual(family.husband) + " had a relationship with ");
                }
                pageContent.html.Append(htmlIndividual(family.wife) + " ");
                if (tagMarriage != null)
                {
                    Tag tagPlace = tagMarriage.children.findOne("PLAC");
                    if (tagPlace != null)
                    {
                        pageContent.html.Append(getTagLongPlace(tagPlace, htmlSources));
                    }
                }
                pageContent.html.Append(". ");

                // Show the children.
                dealtWith.Add("CHIL");
                Individual[] children = family.getChildren();
                if (children.Count() == 0)
                {
                    pageContent.html.Append("They had no children. ");
                }
                else
                {
                    if (children.Count() == 1)
                    {
                        pageContent.html.Append("They had one child, ");
                    }
                    else
                    {
                        pageContent.html.Append("They had " + children.Count().ToString() + " children, ");
                    }
                    for(int childIdx = 0; childIdx< children.Count(); childIdx++)
                    {
                        Individual child = children[childIdx];
                        if (childIdx == children.Count() - 1)
                        {
                            pageContent.html.Append(htmlIndividual(child) + ". ");
                        }
                        else if (childIdx == children.Count() - 2)
                        {
                            pageContent.html.Append(htmlIndividual(child) + " and ");
                        }
                        else
                        {
                            pageContent.html.Append(htmlIndividual(child) + ", ");
                        }
                    }
                }

                dealtWith.Add("DIV");
                Tag tagDivorce = family.tag.children.findOne("DIV");
                if (tagDivorce != null)
                {
                    pageContent.html.Append("They ");
                    if (tagMarriage != null)
                    {
                        pageContent.html.Append("divorced");
                    }
                    else
                    {
                        pageContent.html.Append("separated");
                    }

                    // Show the divorce sources.
                    pageContent.html.Append(addSourceReferences(tagDivorce, htmlSources));

                    Tag tagDate = tagDivorce.children.findOne("DATE");
                    if (tagDate != null)
                    {
                        pageContent.html.Append(" " + getTagLongDate(tagDate, htmlSources));
                    }

                    pageContent.html.Append(". ");
                }

                // Show the remaining tags.
                dealtWith.Add("CHAN");
                addRemainingTags(pageContent.html, family.tag.children, dealtWith);

                // Deal with the sources.
                dealtWith.Add("SOUR");
                Tag[] tags = family.tag.children.findAll("SOUR");
                foreach (Tag sourceTag in tags)
                {
                    string sourceIdx = Tag.toIdx(sourceTag.value);
                    Source source = _gedcom.sources.find(sourceIdx);
                    int refIdx = htmlSources.add(source);
                }

                // Show the source references.
                pageContent.html.Append(htmlSources.toHtml());

                // Show the last changed information.
                pageContent.html.Append("<p>Last Changed " + family.lastChanged.ToString() + "</p>");

                // Show the original gedcom.
                pageContent.html.Append("<pre>" + family.tag.display(0) + "</pre>");
            }

            // Return the built string as html.
            // return _userOptions.renderHtml(html.ToString());
            // Return the page content.
            return pageContent;
        }

        #endregion

        #region Source

        /// <summary>Render the requested source as html.</summary>
        /// <param name="query">Specifies the request query for this source.</param>
        /// <returns>The requested source as html.</returns>
        private PageContent getSource(string query)
        {
            PageContent pageContent = new PageContent();

            NameValueCollection queryParams = HttpUtility.ParseQueryString(query);
            string idx = queryParams.Get("id");

            // Setup the edit options.
            pageContent.editForm = "source?id=" + idx;
            pageContent.editGedomDirectly = "source?id=" + idx;

            Source source = _gedcom.sources.find(idx);
            if (source == null)
            {
                pageContent.html.Append("<h1>Source</h1>");
                pageContent.html.Append("<p>query is '" + query + "'</p>");
                pageContent.html.Append("<p>Can't find '" + idx + "'.</p>");
                pageContent.html.Append(idx + " not found!");
            }
            else
            {
                // Remember which keys are dealt with.
                List<String> dealtWith = new List<String>();

                // Title for the source.
                pageContent.html.Append("<h1>" + source.fullName + " (" + source.idx + ")</h1>");
                dealtWith.Add("TITL");

                // Initialise the sources referenced in this source.  Really expect this to be empty.
                HtmlSources htmlSources = new HtmlSources();

                // Deal with the note tags.
                dealtWith.Add("NOTE");
                Tag[] tagNotes = source.tag.children.findAll("NOTE");
                foreach(Tag tagNote in tagNotes)
                {
                    if (tagNote.value.StartsWith("GRID:"))
                    {
                        // Grid value.
                        pageContent.html.Append("<table style=\"border: 2px solid black;\">");
                        string[][] grid = tagNote.getGridValue();
                        foreach (string[] row in grid)
                        {
                            pageContent.html.Append("<tr>");
                            foreach (string cell in row)
                            {
                                pageContent.html.Append("<td>");
                                pageContent.html.Append(cell);
                                pageContent.html.Append("</td>");
                            }
                            pageContent.html.Append("</tr>");
                        }
                        pageContent.html.Append("</table>");
                    }
                    else
                    {
                        // Standard value.
                        pageContent.html.Append("<p>");
                        string[] lines = tagNote.getMultiLineValue();
                        bool isFirst = true;
                        foreach (string line in lines)
                        {
                            if (!isFirst)
                            {
                                pageContent.html.Append("<br/>");
                            }
                            else
                            {
                                isFirst = false;
                            }
                            pageContent.html.Append(line);
                        }
                        pageContent.html.Append("</p>");
                    }
                }
                
                // Show the remaining tags.
                dealtWith.Add("CHAN");
                addRemainingTags(pageContent.html, source.tag.children, dealtWith);

                // Show the source references.
                pageContent.html.Append(htmlSources.toHtml());

                // Show the last changed information.
                pageContent.html.Append("<p>Last Changed " + source.lastChanged.ToString() + "</p>");

                // Show the original gedcom.
                pageContent.html.Append("<pre>" + source.tag.display(0) + "</pre>");
            }

            // Return the built string as html.
            // return _userOptions.renderHtml(html.ToString());
            // Return the page content.
            return pageContent;
        }



        /// <summary>Returns the full name of the source with a link in html.</summary>
        /// <param name="source">Specifies the source to display.</param>
        /// <returns>The full name of the source with a link in html.</returns>
        private string htmlSource(Source source)
        {
            return "<a href=\"app://source?id=" + source.idx + "\">" + source.fullName + "</a>";
        }

        #endregion

    }
}
