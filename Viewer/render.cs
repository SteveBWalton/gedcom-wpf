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

            // Default place.
            html.Append("in ");

            Tag tagAddress = tag.children.findOne("ADDR");
            if (tagAddress != null)
            {
                html.Append(tagAddress.value + ", ");
            }

            // Add the full place information.
            html.Append(htmlPlace(placeValue));

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
            switch (host)
            {
            case "home":
                return getHome();
            case "individual":
                return getIndividual(query);
            case "family":
                return getFamily(query);
            case "source":
                return getSource(query);
            case "media":
                return getMediaObject(query);
            case "repository":
                return getRepository(query);
            case "place":
                return getPlace(query);
            default:
                return getError(host, query);
            }
        }



        /// <summary>Render the home page in html.</summary>
        /// <returns>The home page in html.</returns>
        private PageContent getHome()
        {
            // The number of items to show in each category.
            const int NUM_ITEMS = 15;

            PageContent pageContent = new PageContent();

            pageContent.html.AppendLine("<h1>" + _gedcom.fileName + (_gedcom.isDirty ? " (*)" : "") + "</h1>");

            // Display the individuals.
            pageContent.html.Append("<fieldset style=\"display: inline-block; vertical-align: top;\">");
            pageContent.html.AppendLine("<legend>Individuals</legend>");
            pageContent.html.AppendLine("<table>");
            int count = 0;
            Individual[] individualsInDateOrder = _gedcom.individuals.inDateOrder();
            foreach (Individual individual in individualsInDateOrder)
            {
                pageContent.html.AppendLine("<tr><td>" + htmlIndividual(individual) + "</td><tr>");
                count++;
                if (count >= NUM_ITEMS)
                {
                    break;
                }
            }
            pageContent.html.AppendLine("</table>");
            pageContent.html.AppendLine("<p>There are " + _gedcom.individuals.count.ToString() + " individuals.");
            pageContent.html.AppendLine("</fieldset>");

            // Display the families.
            pageContent.html.Append("<fieldset style=\"display: inline-block; vertical-align: top;\">");
            pageContent.html.AppendLine("<legend>Families</legend>");
            pageContent.html.AppendLine("<table>");
            count = 0;
            Family[] familiesInDateOrder = _gedcom.families.inDateOrder();
            foreach (Family family in familiesInDateOrder)
            {
                pageContent.html.AppendLine("<tr><td><a href=\"app://family?id=" + family.idx + "\">" + family.fullName + "</a></td></tr>");
                count++;
                if (count >= NUM_ITEMS)
                {
                    break;
                }
            }
            pageContent.html.AppendLine("</table>");
            pageContent.html.AppendLine("<p>There are " + _gedcom.families.count.ToString() + " families.");
            pageContent.html.AppendLine("</fieldset>");

            // Display the sources.
            pageContent.html.Append("<fieldset style=\"display: inline-block; vertical-align: top;\">");
            pageContent.html.AppendLine("<legend>Sources</legend>");
            pageContent.html.AppendLine("<table>");
            count = 0;
            Source[] sourcesInDateOrder = _gedcom.sources.inDateOrder();
            foreach (Source source in sourcesInDateOrder)
            {
                pageContent.html.AppendLine("<tr><td>" + htmlSource(source) + "</td></tr>");
                count++;
                if (count >= NUM_ITEMS)
                {
                    break;
                }
            }
            pageContent.html.AppendLine("</table>");
            pageContent.html.AppendLine("<p>There are " + _gedcom.sources.count.ToString() + " sources.");
            pageContent.html.AppendLine("</fieldset>");

            // Display the media objects.
            pageContent.html.Append("<fieldset style=\"display: inline-block; vertical-align: top;\">");
            pageContent.html.AppendLine("<legend>Media Objects</legend>");
            pageContent.html.AppendLine("<table>");
            count = 0;
            MediaObject[] mediaObjectsInDateOrder = _gedcom.mediaObjects.inDateOrder();
            foreach (MediaObject mediaObject in mediaObjectsInDateOrder)
            {
                pageContent.html.AppendLine("<tr><td>" + htmlMediaObject(mediaObject) + "</td></tr>");
                count++;
                if (count >= NUM_ITEMS)
                {
                    break;
                }
            }
            pageContent.html.AppendLine("</table>");
            pageContent.html.AppendLine("<p>There are " + _gedcom.mediaObjects.count.ToString() + " media objects.");
            pageContent.html.AppendLine("</fieldset>");

            // Display the repositories.
            pageContent.html.Append("<fieldset style=\"display: inline-block; vertical-align: top;\">");
            pageContent.html.AppendLine("<legend>Repositories</legend>");
            pageContent.html.AppendLine("<table>");
            count = 0;
            Repository[] repositoriesInDateOrder = _gedcom.repositories.inDateOrder();
            foreach (Repository repository in repositoriesInDateOrder)
            {
                pageContent.html.AppendLine("<tr><td>" + htmlRepository(repository) + "</td></tr>");
                count++;
                if (count >= NUM_ITEMS)
                {
                    break;
                }
            }
            pageContent.html.AppendLine("</table>");
            pageContent.html.AppendLine("<p>There are " + _gedcom.repositories.count.ToString() + " repositories.");
            pageContent.html.AppendLine("</fieldset>");
            // Display the places.
            pageContent.html.Append("<fieldset style=\"display: inline-block; vertical-align: top;\">");
            pageContent.html.AppendLine("<legend>Places</legend>");
            pageContent.html.AppendLine("<table>");
            count = 0;
            foreach(Place place in _gedcom.places)
            {
                int totalCount = place.getTotalCount();
                pageContent.html.AppendLine("<tr><td><a href=\"app://place?id=" + place.name + "\">" + place.name + "</a> (" + totalCount.ToString() + ")</td></tr>");
                count += totalCount;
            }
            pageContent.html.AppendLine("</table>");
            pageContent.html.AppendLine("<p>There are " + count.ToString() + " places.");
            pageContent.html.AppendLine("</fieldset>");

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
                RenderTree renderTree = new RenderTree(individual);
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
                foreach (string relationship in relationships)
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

                // Show the file gedcom.
                pageContent.html.Append("<pre style=\"width: 400px; display: inline-block; vertical-align: top;\">" + individual.tag.toText() + "</pre>");

                // Mark the individual as viewed.
                individual.lastViewed = DateTime.Now;
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
            if (individual == null)
            {
                return "Error";
            }
            // return "<a href=\"app://individual?id=" + individual.idx + "\">" + individual.fullName + "</a>";
            StringBuilder result = new StringBuilder("<a href=\"app://individual?id=" + individual.idx + "\">" + individual.fullName);
            if (individual.dob != null)
            {
                result.Append(" (");
                if (individual.dob.approxDate != null)
                {
                    result.Append(individual.dob.approxDate.Year.ToString());
                }
                if (individual.dod != null)
                {
                    result.Append("-");
                    if (individual.dod.approxDate != null)
                    {
                        result.Append(individual.dod.approxDate.Year.ToString());
                    }
                }
                result.Append(")");
            }
            result.Append("</a>");
            return result.ToString();
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
            if (family== null)
            {
                return "error unknown family";
            }
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
            if (family == null)
            {
                return "error unknown family";
            }
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
            // Start html page for family.
            PageContent pageContent = new PageContent();
            pageContent.html.Append("<p><a href=\"app://home\">Home</a></p>");

            // Identify the family.
            NameValueCollection queryParams = HttpUtility.ParseQueryString(query);
            string idx = queryParams.Get("id");

            // Setup the edit options.
            pageContent.editForm = "family?id=" + idx;
            pageContent.editGedomDirectly = "family?id=" + idx;

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

                // Little family tree control for this family.
                RenderTree renderTree = new RenderTree(family);
                // pageContent.html.Append(renderTree.getTree());
                pageContent.html.Append(renderTree.getSmallTree());

                // Initialise the sources referenced in this family.
                HtmlSources htmlSources = new HtmlSources();

                pageContent.html.AppendLine("<p>");

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

                pageContent.html.AppendLine("</p>");

                // Deal with any level 1 sources.
                dealtWith.Add("SOUR");
                Tag[] tagSources = family.tag.children.findAll("SOUR");
                if (tagSources!=null)
                {
                    foreach(Tag tagSource in tagSources)
                    {
                        string sourceIdx = Tag.toIdx(tagSource.value);
                        Source source = _gedcom.sources.find(sourceIdx);
                        int refIdx = htmlSources.add(source);
                    }
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

            // Set the last viewed time.
            family.lastViewed = DateTime.Now;

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
            // Start html page for source.
            PageContent pageContent = new PageContent();

            // Identify the source.
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

                // Deal with the note grid tag.
                dealtWith.Add("NOTE");
                Tag[] tagNotes = source.tag.children.findAll("NOTE");
                foreach (Tag tagNote in tagNotes)
                {
                    if (tagNote.value.StartsWith("GRID:"))
                    {
                        switch (source.sourceType)
                        {
                        case Source.SourceType.MARRIAGE_CERTIFICATE:
                            pageContent.html.Append(getMarriageCertificate(source, tagNote, dealtWith));
                            break;

                        case Source.SourceType.BIRTH_CERTIFICATE:
                            pageContent.html.Append(getBirthCertificate(source, tagNote, dealtWith));
                            break;

                        case Source.SourceType.DEATH_CERTIFICATE:
                            pageContent.html.Append(getDeathCerificate(source, tagNote, dealtWith));
                            break;

                        case Source.SourceType.CENSUS:
                            pageContent.html.Append(getCensus(source, tagNote, dealtWith));
                            break;

                        default:
                            // Grid value.
                            pageContent.html.Append("<table style=\"border: 2px solid black;\">");
                            TagNoteGrid grid = tagNote.getTagNoteGrid();
                            grid.toHtml();
                            pageContent.html.Append("</table>");
                            break;
                        }
                    }
                }

                // Deal with the other note tags.
                dealtWith.Add("NOTE");
                foreach (Tag tagNote in tagNotes)
                {
                    if (tagNote.value.StartsWith("GRID:"))
                    {
                        // Already dealt with.
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

                // Deal with the REPO tag.
                Tag tagRepo = source.tag.children.findOne("REPO");
                dealtWith.Add("REPO");
                if (tagRepo == null)
                {
                    pageContent.html.Append("<p>This source is not in a repository.</p>");
                }
                else
                {
                    string repositoryIdx = Tag.toIdx(tagRepo.value);
                    Repository repository = _gedcom.repositories.find(repositoryIdx);
                    if (repository == null)
                    {
                        pageContent.html.Append("<p>This source is in the {ERROR: " + repositoryIdx + "} repository.</p>");
                    }
                    else
                    {
                        pageContent.html.Append("<p>This source is in the " + repository.name + " repository.</p>");
                    }
                }

                // Show the remaining tags.
                dealtWith.Add("CHAN");
                addRemainingTags(pageContent.html, source.tag.children, dealtWith);

                // Show the source references.
                pageContent.html.Append(htmlSources.toHtml());

                // Show the last changed information.
                pageContent.html.Append("<p>Last Changed " + source.lastChanged.ToString() + "</p>");

                // Show the individuals with a connection to this media object.
                int count = 0;
                pageContent.html.Append("<fieldset style=\"display: inline-block; vertical-align: top;\">");
                pageContent.html.AppendLine("<legend>Connected Individuals</legend>");
                pageContent.html.AppendLine("<table>");
                Individual[] individualsInDateOrder = _gedcom.individuals.inDateOrder();
                foreach (Individual individual in individualsInDateOrder)
                {
                    if (individual.hasConnection(source))
                    {
                        pageContent.html.AppendLine("<tr><td>" + htmlIndividual(individual) + "</td><tr>");
                        count++;
                    }
                }
                pageContent.html.AppendLine("</table>");
                pageContent.html.AppendLine("<p>There are " + count.ToString() + " individuals with a connection to this source.");
                pageContent.html.AppendLine("</fieldset>");


                // Show the original gedcom.
                pageContent.html.Append("<pre>" + source.tag.display(0) + "</pre>");
            }

            // Set the last viewed time.
            source.lastViewed = DateTime.Now;

            // Return the built string as html.
            // return _userOptions.renderHtml(html.ToString());
            // Return the page content.
            return pageContent;
        }



        /// <summary>Get the note grid in marriage certificate html format.</summary>
        /// <param name="source">Specifies the source that contains the note grid.</param>
        /// <param name="tagNote">Specifies the note grid tag in the source.</param>
        /// <param name="dealtWith">Specifies and returns the dealt with tags in the source.</param>
        /// <returns>A html render of the marriage certificate data.</returns>
        private string getMarriageCertificate(Source source, Tag tagNote, List<string> dealtWith)
        {
            // Start to build the html for the marriage certificate.
            StringBuilder pageContent = new StringBuilder();

            // Get the date of the source.
            Tag tag = source.tag.children.findOne("DATE");
            dealtWith.Add("DATE");
            TagDate tagDate = new TagDate(tag);

            // Get the place of the source.
            Tag tagPlac = source.tag.children.findOne("PLAC");
            dealtWith.Add("PLAC");
            TagPlace tagPlace = new TagPlace(tagPlac);

            // Marriage certificate.
            pageContent.Append("<table style=\"background-color: #ccff99; border: 1px solid black; margin: auto;\" cellpadding=\"5\" cellspacing=\"0\" >");

            // The grid of values in the note grid tag.
            TagNoteGrid grid = tagNote.getTagNoteGrid();
            pageContent.Append("<tr><td colspan=\"7\">" + tagDate.yearDisplay + " <span class=\"marriage\">Marriage solemnized at</span> " + tagPlace.ToString() + "</td></tr>");
            pageContent.Append("<tr>");
            pageContent.Append("<td><span class=\"marriage\">When Married</span></td>");
            pageContent.Append("<td><span class=\"marriage\">Name</span></td>");
            pageContent.Append("<td><span class=\"marriage\">Age</span></td>");
            pageContent.Append("<td><span class=\"marriage\">Rank or Profession</span></td>");
            pageContent.Append("<td><span class=\"marriage\">Residence at the time of marriage</span></td>");
            pageContent.Append("<td><span class=\"marriage\">Father's Name</span></td>");
            pageContent.Append("<td><span class=\"marriage\">Rank of Profession of Father</span></td>");
            pageContent.Append("</tr>");
            pageContent.Append("<tr>");
            pageContent.Append($"<td rowspan=\"2\">{tagDate.getShortDate()}</td>");
            pageContent.Append($"<td>{grid.getCell(1, 1)}</td>");
            pageContent.Append($"<td>{grid.getCell(1, 3)}</td>");
            pageContent.Append("<td>" + grid.getCell(1,4) + "</td>");
            pageContent.Append("<td>" + grid.getCell(1,5) + "</td>");
            pageContent.Append("<td>" + grid.getCell(3,1) + "</td>");
            pageContent.Append("<td>" + grid.getCell(3,3) + "</td>");
            pageContent.Append("</tr>");
            pageContent.Append("<tr>");
            pageContent.Append("<td>" + grid.getCell(2,1) + "</td>");
            pageContent.Append("<td>" + grid.getCell(2,3) + "</td>");
            pageContent.Append("<td>" + grid.getCell(2,4) + "</td>");
            pageContent.Append("<td>" + grid.getCell(2,5) + "</td>");
            pageContent.Append("<td>" + grid.getCell(4,1) + "</td>");
            pageContent.Append("<td>" + grid.getCell(4,3) + "</td>");
            pageContent.Append("</tr>");
            pageContent.Append("<tr><td colspan=\"7\"><span class=\"marriage\">in the Presence of us,</span> " + grid.getCell(5,1) + "</td></tr>");
            pageContent.Append("<tr><td colspan=\"7\" style=\"text-align: center;\"><span class=\"marriage\">GRO Reference</span> " + grid.getCell(0,1) + "</td></tr>");

            //foreach (string[] row in grid)
            //{
            //    pageContent.Append("<tr>");
            //    foreach (string cell in row)
            //    {
            //        pageContent.Append("<td>");
            //        pageContent.Append(cell);
            //        pageContent.Append("</td>");
            //    }
            //    pageContent.Append("</tr>");
            //}
            pageContent.Append("</table>");

            // Return the html for a marriage cerificate.
            return pageContent.ToString();
        }



        /// <summary>Get the note grid tag in birth certificate html format.</summary>
        /// <param name="source">Specifies the source that contains the note grid.</param>
        /// <param name="tagNote">Specifies the note grid tag in the source.</param>
        /// <param name="dealtWith">Specifies and returns the dealt with tags in the source.</param>
        /// <returns>A html render of the birth certificate data.</returns>
        private string getBirthCertificate(Source source, Tag tagNote, List<string> dealtWith)
        {
            // Start to build the html for the marriage certificate.
            StringBuilder pageContent = new StringBuilder();

            // Get the date of the source.
            Tag tag = source.tag.children.findOne("DATE");
            dealtWith.Add("DATE");
            TagDate tagDate = new TagDate(tag);

            // Birth certificate.
            pageContent.Append("<table style=\"background-color: mistyrose; border: 1px solid black; margin: auto;\" cellpadding=\"5\" cellspacing=\"0\" >");

            // The grid of values in the note grid tag.
            TagNoteGrid grid = tagNote.getTagNoteGrid();
            pageContent.Append($"<tr><td colspan=\"8\">{tagDate.yearDisplay} <span class=\"birth\"> Birth in the registration district of</span> {grid.getCell(1, 1)}</td></tr>");
            pageContent.Append("<tr>");
            pageContent.Append("<td><span class=\"birth\">When and<br/>Where Born</span></td>");
            pageContent.Append("<td><span class=\"birth\">Name</span></td>");
            pageContent.Append("<td><span class=\"birth\">Sex</span></td>");
            pageContent.Append("<td><span class=\"birth\">Father</span></td>");
            pageContent.Append("<td><span class=\"birth\">Mother</span></td>");
            pageContent.Append("<td><span class=\"birth\">Occupation of Father</span></td>");
            pageContent.Append("<td><span class=\"birth\">Informant</span></td>");
            pageContent.Append("<td><span class=\"birth\">When Registered</span></td>");
            pageContent.Append("</tr>");

            pageContent.Append("<tr>");
            pageContent.Append($"<td>{grid.getCell(2, 1)}<br/>{grid.getCell(2, 2)}</td>");
            pageContent.Append($"<td>{grid.getCell(3, 1)}</td>");
            pageContent.Append($"<td>{grid.getCell(3, 2)}</td>");
            pageContent.Append($"<td>{grid.getCell(5, 1)}</td>");
            pageContent.Append($"<td>{grid.getCell(4, 1)}<br/>{grid.getCell(4, 2)}</td>");
            pageContent.Append($"<td>{grid.getCell(5, 2)}</td>");
            pageContent.Append($"<td>{grid.getCell(6, 1)}<br/>{grid.getCell(6, 2)}</td>");
            // pageContent.Append($"<td>{grid.getCell(7, 1)}</td>");
            pageContent.Append($"<td>{tagDate.getShortDate()}</td>");
            pageContent.Append("</tr>");

            pageContent.Append($"<tr><td colspan=\"7\" style=\"text-align: center;\"><span class=\"birth\">GRO Reference</span> {grid.getCell(0, 1)}</td></tr>");

            // pageContent.Append(grid.toHtml());

            pageContent.Append("</table>");

            // Return the html for a marriage cerificate.
            return pageContent.ToString();
        }



        /// <summary>Get the note grid tag in death certificate html format.</summary>
        /// <param name="source">Specifies the source that contains the note grid.</param>
        /// <param name="tagNote">Specifies the note grid tag in the source.</param>
        /// <param name="dealtWith">Specifies and returns the dealt with tags in the source.</param>
        /// <returns>A html render of the death cerificate data.</returns>
        private string getDeathCerificate(Source source, Tag tagNote, List<string> dealtWith)
        {
            // Start to build the html for the death certificate.
            StringBuilder pageContent = new StringBuilder();

            // Get the date of the source.
            Tag tag = source.tag.children.findOne("DATE");
            dealtWith.Add("DATE");
            TagDate tagDate = new TagDate(tag);

            // The grid of values in the note grid tag.
            TagNoteGrid grid = tagNote.getTagNoteGrid();

            // Death certificate.
            pageContent.Append("<table style=\"background-color: thistle; border: 1px solid black; margin: auto;\" cellpadding=\"5\" cellspacing=\"0\" >");
            pageContent.Append("<tr>");
            pageContent.Append($"<tr><td class=\"death\" style=\"text-align: right\">Registration District</td><td colspan=\"3\">{grid.getCell(1, 1)}</td></tr>");
            pageContent.Append($"<tr><td class=\"death\" style=\"text-align: right\">When and Where</td><td colspan=\"3\">{grid.getCell(2, 1)}</td></tr>");
            pageContent.Append($"<tr><td class=\"death\" style=\"text-align: right\">Name</td><td>{grid.getCell(4, 1)}</td><td class=\"death\" style=\"text-align: right\">Sex</td><td>{grid.getCell(4, 2)}</td></tr>");
            pageContent.Append($"<tr><td class=\"death\" style=\"text-align: right\">Date Place of Birth</td><td colspan=\"3\">{grid.getCell(5, 1)}</td></tr>");
            pageContent.Append($"<tr><td class=\"death\" style=\"text-align: right\">Occupation</td><td colspan=\"3\">{grid.getCell(6, 1)}</td></tr>");
            pageContent.Append($"<tr><td class=\"death\" style=\"text-align: right\">Usual Address</td><td colspan=\"3\">{grid.getCell(7, 1)}</td></tr>");
            pageContent.Append($"<tr><td class=\"death\" style=\"text-align: right\">Cause of Death</td><td colspan=\"3\">{grid.getCell(8, 1)}</td></tr>");
            pageContent.Append($"<tr><td class=\"death\" style=\"text-align: right\">Informant</td><td>{grid.getCell(9, 1)}</td><td class=\"death\" style=\"text-align: right\">Informant Description</td><td>{grid.getCell(9, 2)}</td></tr>");
            pageContent.Append($"<tr><td class=\"death\" style=\"text-align: right\">Informant Address</td><td colspan=\"3\">{grid.getCell(10, 1)}</td></tr>");
            pageContent.Append($"<tr><td class=\"death\" style=\"text-align: right\">When Registered</td><td>{tagDate.getShortDate()}</td><td class=\"death\" style=\"text-align: right\">Reference</td><td>{grid.getCell(0, 1)}</td></tr>");

            // pageContent.Append(grid.toHtml());

            pageContent.Append("</table>");

            // Return the html for a death cerificate.
            return pageContent.ToString();
        }



        /// <summary>Get the note grid tag in census source html format.</summary>
        /// <param name="source">Specifies the source that contains the note grid.</param>
        /// <param name="tagNote">Specifies the note grid tag in the source.</param>
        /// <param name="dealtWith">Specifies and returns the dealt with tags in the source.</param>
        /// <returns>A html render of the census data.</returns>
        private string getCensus(Source source, Tag tagNote, List<string> dealtWith)
        {
            // Start to build the html for the census record.
            StringBuilder pageContent = new StringBuilder();

            // Get the date of the source.
            Tag tag = source.tag.children.findOne("DATE");
            dealtWith.Add("DATE");
            TagDate tagDate = new TagDate(tag);

            // The grid of values in the note grid tag.
            TagNoteGrid grid = tagNote.getTagNoteGrid();

            // Census record.
            pageContent.Append("<table style=\"background-color: lightcyan; border: 1px solid black; margin: auto;\" cellpadding=\"5\" cellspacing=\"0\" >");
            pageContent.Append($"<tr><td class=\"census\" style=\"text-align: center;\" colspan=\"5\"><span style=\"font-size:200%;\">{tagDate.yearDisplay} Census</span> ({tagDate.getShortDate()})</td><td>");
            pageContent.Append("<tr><td colspan=\"5\">");
            pageContent.Append("<table width=\"100%\">");
            pageContent.Append("<tr><td class=\"census\" style=\"text-align: center;\">Series</td><td class=\"census\" style=\"text-align: center;\">Piece</td><td class=\"census\" style=\"text-align: center;\">Folio</td><td class=\"census\" style=\"text-align: center;\">Page</td></tr>");
            pageContent.Append($"<tr><td style=\"text-align: center;\">{grid.getCell(0, 2)}</td><td style=\"text-align: center;\">{grid.getCell(0, 4)}</td><td style=\"text-align: center;\">{grid.getCell(0, 6)}</td><td style=\"text-align: center;\">{grid.getCell(0, 8)}</td></tr>");
            pageContent.Append("</table></td></tr>");

            pageContent.Append("<tr><td class=\"census\">Name</td><td class=\"census\">Relation<br/>To Head</td><td class=\"census\">Age</td><td class=\"census\">Occupation</td><td class=\"census\">Born Location</td></tr>");

            for (int i = 1; i < grid.numRows; i++)
            {
                pageContent.Append($"<tr><td><a href=\"app://individual?id={grid.getCell(i, 1).Trim()}\">{grid.getCell(i, 0)}</a></td><td>{grid.getCell(i, 3)}</td><td>{grid.getCell(i, 2)}</td><td>{grid.getCell(i, 4)}</td><td>{grid.getCell(i, 5)}</td></tr>");
            }

            // pageContent.Append(grid.toHtml());

            pageContent.Append("</table>");

            // Return the html for a census record.
            return pageContent.ToString();
        }



        /// <summary>Returns the full name of the source with a link in html.</summary>
        /// <param name="source">Specifies the source to display.</param>
        /// <returns>The full name of the source with a link in html.</returns>
        private string htmlSource(Source source)
        {
            return "<a href=\"app://source?id=" + source.idx + "\">" + source.fullName + "</a>";
        }

        #endregion

        #region Media Object

        /// <summary>Render the requested media object as html.</summary>
        /// <param name="query">Specifies the request query for this media object.</param>
        /// <returns>The requested media object as html.</returns>
        private PageContent getMediaObject(string query)
        {
            // Start html page for source.
            PageContent pageContent = new PageContent();

            // Identify the source.
            NameValueCollection queryParams = HttpUtility.ParseQueryString(query);
            string idx = queryParams.Get("id");

            // Setup the edit options.
            // pageContent.editForm = "media?id=" + idx;
            pageContent.editGedomDirectly = "media?id=" + idx;

            MediaObject mediaObject = _gedcom.mediaObjects.find(idx);
            if (mediaObject == null)
            {
                pageContent.html.Append("<h1>Media Object</h1>");
                pageContent.html.Append("<p>query is '" + query + "'</p>");
                pageContent.html.Append("<p>Can't find '" + idx + "'.</p>");
                pageContent.html.Append(idx + " not found!");
            }
            else
            {
                // Remember which keys are dealt with.
                List<String> dealtWith = new List<String>();

                // Title for the source.
                pageContent.html.Append("<h1>" + mediaObject.title + " (" + mediaObject.idx + ")</h1>");
                dealtWith.Add("TITL");

                // Show the remaining tags.
                dealtWith.Add("CHAN");
                addRemainingTags(pageContent.html, mediaObject.tag.children, dealtWith);

                // Show the last changed information.
                pageContent.html.Append("<p>Last Changed " + mediaObject.lastChanged.ToString() + "</p>");

                // Show the original gedcom.
                pageContent.html.Append("<pre>" + mediaObject.tag.display(0) + "</pre>");

                // Show the individuals with a connection to this media object.
                int count = 0;
                pageContent.html.AppendLine("<fieldset><legend>Connected Individuals</legend>");
                pageContent.html.AppendLine("<table>");
                Individual[] individualsInDateOrder = _gedcom.individuals.inDateOrder();
                foreach (Individual individual in individualsInDateOrder)
                {
                    if (individual.hasConnection(mediaObject))
                    {
                        pageContent.html.AppendLine("<tr><td>" + htmlIndividual(individual) + "</td><tr>");
                        count++;
                    }
                }
                pageContent.html.AppendLine("</table>");
                pageContent.html.AppendLine("<p>There are " + count.ToString() + " individuals with a connection to this media object.");
                pageContent.html.AppendLine("</fieldset>");
            }

            // Return the built page.
            return pageContent;
        }


        /// <summary>Returns the title for the media object with a link in html.</summary>
        /// <param name="mediaObject">Specifies the media object to display.</param>
        /// <returns>The title for the media object with a link in html.</returns>
        private string htmlMediaObject(MediaObject mediaObject)
        {
            return "<a href=\"app://media?id=" + mediaObject.idx + "\">" + mediaObject.title + "</a>";
        }

        #endregion

        #region Repository

        /// <summary>Render the requested repository as html.</summary>
        /// <param name="query">Specifies the request query for this repository.</param>
        /// <returns>The requested repostiory as html.</returns>
        private PageContent getRepository(string query)
        {
            // Start html page for source.
            PageContent pageContent = new PageContent();

            // Identify the source.
            NameValueCollection queryParams = HttpUtility.ParseQueryString(query);
            string idx = queryParams.Get("id");

            // Setup the edit options.
            // pageContent.editForm = "repository?id=" + idx;
            pageContent.editGedomDirectly = "repository?id=" + idx;

            Repository repository = _gedcom.repositories.find(idx);
            if (repository == null)
            {
                pageContent.html.Append("<h1>Repository</h1>");
                pageContent.html.Append("<p>query is '" + query + "'</p>");
                pageContent.html.Append("<p>Can't find '" + idx + "'.</p>");
                pageContent.html.Append(idx + " not found!");
            }
            else
            {
                // Remember which keys are dealt with.
                List<String> dealtWith = new List<String>();

                // Title for the source.
                pageContent.html.Append("<h1>" + repository.name + " (" + repository.idx + ")</h1>");
                dealtWith.Add("NAME");

                // Show the remaining tags.
                dealtWith.Add("CHAN");
                addRemainingTags(pageContent.html, repository.tag.children, dealtWith);

                // Show the last changed information.
                pageContent.html.Append("<p>Last Changed " + repository.lastChanged.ToString() + "</p>");

                // Show the original gedcom.
                pageContent.html.Append("<pre>" + repository.tag.display(0) + "</pre>");

                // Show the sources with a connection to this repository.
                int count = 0;
                pageContent.html.AppendLine("<fieldset><legend>Sources</legend>");
                pageContent.html.AppendLine("<table>");
                foreach (Source source in _gedcom.sources)
                {
                    if (source.hasConnection(repository))
                    {
                        pageContent.html.AppendLine("<tr><td>" + htmlSource(source) + "</td><tr>");
                        count++;
                    }
                }
                pageContent.html.AppendLine("</table>");
                pageContent.html.AppendLine("<p>There are " + count.ToString() + " sources with a connection to " + repository.name + ".");
                pageContent.html.AppendLine("</fieldset>");
            }

            // Return the built page.
            return pageContent;
        }



        /// <summary>Returns the name for the repository with a link in html.</summary>
        /// <param name="repository">Specifies the repository to display.</param>
        /// <returns>The name for the repository with a link in html.</returns>
        private string htmlRepository(Repository repository)
        {
            return "<a href=\"app://repository?id=" + repository.idx + "\">" + repository.name + "</a>";
        }

        #endregion

        #region Place

        /// <summary>Render the requested source as html.</summary>
        /// <param name="query">Specifies the request query for this source.</param>
        /// <returns>The requested source as html.</returns>
        private PageContent getPlace(string query)
        {
            PageContent pageContent = new PageContent();

            NameValueCollection queryParams = HttpUtility.ParseQueryString(query);
            string idx = queryParams.Get("id");

            // Setup the edit options.
            pageContent.editForm = ""; // "source?id=" + idx;
            pageContent.editGedomDirectly = ""; // "source?id=" + idx;

            Place place = _gedcom.places.getPlace(idx);
            if (place == null)
            {
                pageContent.html.Append("<h1>Place</h1>");
                pageContent.html.Append("<p>query is '" + query + "'</p>");
                pageContent.html.Append("<p>Can't find '" + idx + "'.</p>");
                pageContent.html.Append(idx + " not found!");
            }
            else
            {
                // Title for the place.
                pageContent.html.AppendLine("<h1>" + htmlPlace(place) + "</h1>");

                // Show the child places.
                pageContent.html.Append("<fieldset style=\"display: inline-block; vertical-align: top;\">");
                pageContent.html.AppendLine("<legend>Properties</legend>");
                pageContent.html.AppendLine("<table>");
                pageContent.html.Append("<tr><td>Longitude</td><td>");
                pageContent.html.Append(place.longitude.ToString("##0.000000"));
                pageContent.html.AppendLine("</td></tr>");
                pageContent.html.Append("<tr><td>Latitude</td><td>");
                pageContent.html.Append(place.latitude.ToString("##0.000000"));
                pageContent.html.AppendLine("</td></tr>");
                pageContent.html.AppendLine("</table>");
                pageContent.html.AppendLine("</fieldset>");

                // Show the child places.
                pageContent.html.Append("<fieldset style=\"display: inline-block; vertical-align: top;\">");
                pageContent.html.AppendLine("<legend>Child Locations</legend>");

                pageContent.html.AppendLine("<table>");
                pageContent.html.AppendLine("<tr style=\"font-weight: bold;\"><td>Child Locations</td></tr>");
                foreach (Place child in place.children)
                {
                    pageContent.html.Append("<tr>");
                    pageContent.html.Append("<td><a href=\"app://place?id=" + child.fullName + "\">" + child.name + "</a> (" + child.getTotalCount() + ")</td>");
                    pageContent.html.AppendLine("</tr>");
                }

                pageContent.html.AppendLine("</table>");
                pageContent.html.AppendLine("</fieldset>");

                // Show individuals with connection to this place.
                pageContent.html.Append("<fieldset style=\"display: inline-block; vertical-align: top;\">");
                pageContent.html.AppendLine("<legend>Connected Individuals</legend>");
                pageContent.html.AppendLine("<table>");

                int count = 0;
                Individual[] individualsInDateOrder = _gedcom.individuals.inDateOrder();
                foreach (Individual individual in individualsInDateOrder)
                {
                    if (individual.hasConnection(place))
                    {
                        pageContent.html.AppendLine("<tr><td>" + htmlIndividual(individual) + "</td><tr>");
                        count++;
                    }
                }
                pageContent.html.AppendLine("</table>");
                pageContent.html.AppendLine("<p>There are " + count.ToString() + " individuals with a connection to " + place.fullName + ".");
                pageContent.html.AppendLine("</fieldset>");
            }

            // Return the built string as html.
            // return _userOptions.renderHtml(html.ToString());
            // Return the page content.
            return pageContent;
        }



        /// <summary>Returns the full name of the place in html with links.</summary>
        /// <param name="place">Specifies the place to display.</param>
        /// <returns>The html for the place with the full name and links.</returns>
        private string htmlPlace(Place place)
        {
            if (place == null)
            {
                return "Error";
            }
            if (place.parent == null)
            {
                return "<a href=\"app://place?id=" + place.fullName + "\">" + place.name + "</a>";
            }
            return "<a href=\"app://place?id=" + place.fullName + "\">" + place.name + "</a>, " + htmlPlace(place.parent);
        }



        /// <summary>Returns the full name of the place in html with links.</summary>
        /// <param name="placeDescription">Specifies the place value.</param>
        /// <returns>The html for the place with the full name and links.</returns>
        private string htmlPlace(string placeDescription)
        {
            if (placeDescription.Contains(","))
            {
                // Split the place desription.
                int lastPos = placeDescription.IndexOf(",");
                string right = placeDescription.Substring(lastPos + 1).Trim();
                string left = placeDescription.Substring(0, lastPos).Trim();
                return "<a href=\"app://place?id=" + placeDescription + "\">" + left + "</a>, " + htmlPlace(right);
            }
            else
            {
                // Simple place description.
                return "<a href=\"app://place?id=" + placeDescription + "\">" + placeDescription + "</a>";
            }
        }

        #endregion

    }
}
