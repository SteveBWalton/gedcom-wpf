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
                    // html.Append("<p>'" + tag.line + "' = '" + tag.key + "' = '" + tag.value + "'</p>");
                    html.Append("<p>'" + tag.key + "' = '" + tag.value + "'</p>");
                    count++;
                }
            }
            // Return the number of tags.
            return count;
        }



        /// <summary>Returns the long description of a date tag in html.</summary>
        /// <param name="tag">Specifies the date tag to return.</param>
        /// <param name="htmlSources">Specifies the current source references.  Returns with the additonal sources refereneces used in the description.</param>
        /// <returns></returns>
        private string getTagLongDate(Tag tag, HtmlSources htmlSources)
        {
            // Build a long description of the date tag.
            StringBuilder html = new StringBuilder();

            // Examine the date value.
            string dateValue = tag.value;

            if (dateValue.Contains("BEF"))
            {
                // Before date.
                dateValue = dateValue.Replace("BEF", "");
                html.Append("before ");
            }
            else
            {
                // Default date.
                html.Append("on ");
            }

            // Add the unformated date information.
            html.Append(dateValue);

            // Dealt with about.
            html.Replace("ABT", "about");

            // Dealt with months.
            html.Replace("JAN", "January");
            html.Replace("FEB", "February");
            html.Replace("MAR", "March");
            html.Replace("APR", "April");
            html.Replace("MAY", "May");
            html.Replace("JUN", "June");
            html.Replace("JUL", "July");
            html.Replace("AUG", "August");
            html.Replace("SEP", "September");
            html.Replace("OCT", "October");
            html.Replace("NOV", "November");
            html.Replace("DEC", "December");

            // Show the sources.
            html.Append(addSourceReferences(tag, htmlSources));

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
            html.Append("at ");

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
            html.Append(proNoun);
            html.Append(" ");
            html.Append(verb);

            if (tag.value.Length > 1)
            {
                html.Append(" ");
                html.Append(tag.value);
            }

            // Show any date information.
            Tag tagDate = tag.children.findOne("DATE");
            if (tagDate != null)
            {
                html.Append(" ");
                html.Append(getTagLongDate(tagDate, htmlSources));
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
        public string getContent(string host, string query)
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
        private string getHome()
        {
            StringBuilder html = new StringBuilder();

            html.Append("<h1>" + _gedcom.fileName + "</h1>");

            // Display the individuals.
            html.Append("<fieldset style=\"width: 400px; display: inline-block; vertical-align: top;\">");
            html.Append("<legend>Individuals</legend>");
            html.Append("<table>");
            int count = 0;
            Individual[] individualsInDateOrder = _gedcom.individuals.inDateOrder();
            foreach (Individual individual in individualsInDateOrder)
            {
                html.Append("<tr><td>" + htmlIndividual(individual) + "</td><tr>");
                count++;
                if (count >= 10)
                {
                    break;
                }
            }
            html.Append("</table>");
            html.Append("<p>There are " + _gedcom.individuals.count.ToString() + " individuals.");
            html.Append("</fieldset>");
            
            // Display the families.
            html.Append("<fieldset style=\"width: 400px; display: inline-block; vertical-align: top;\">");
            html.Append("<legend>Families</legend>");
            html.Append("<table>");
            count = 0;
            Family[] familiesInDateOrder = _gedcom.families.inDateOrder();
            foreach (Family family in familiesInDateOrder)
            {
                html.Append("<tr><td><a href=\"app://family?id=" + family.idx + "\">" + family.fullName + "</a></td></tr>");
                count++;
                if (count >= 10)
                {
                    break;
                }
            }
            html.Append("</table>");
            html.Append("<p>There are " + _gedcom.families.count.ToString() + " families.");
            html.Append("</fieldset>");

            // Display the sources.
            html.Append("<fieldset style=\"width: 400px; display: inline-block; vertical-align: top;\">");
            html.Append("<legend>Sources</legend>");
            html.Append("<table>");
            count = 0;
            Source[] sourcesInDateOrder = _gedcom.sources.inDateOrder();
            foreach (Source source in sourcesInDateOrder)
            {
                html.Append("<tr><td>" + htmlSource(source) + "</td></tr>");
                count++;
                if (count >= 10)
                {
                    break;
                }
            }
            html.Append("</table>");
            html.Append("<p>There are " + _gedcom.sources.count.ToString() + " sources.");
            html.Append("</fieldset>");

            // Return the built string as html.
            return _userOptions.renderHtml(html.ToString());
        }



        /// <summary>Render an error in html.</summary>
        /// <param name="host">Specifies request host.</param>
        /// <param name="query">Specifies the request query.</param>
        /// <returns>An error message in html format.</returns>
        private string getError(string host, string query)
        {
            StringBuilder html = new StringBuilder();

            html.Append("<h1>Error</h1>");
            html.Append("<p>host is '" + host + "', query is '" + query + "'</p>");
            html.Append("<p><a href=\"app://home\">Home</a></p>");

            // Return the built string as html.
            return _userOptions.renderHtml(html.ToString());
        }

        #endregion

        #region Individual

        /// <summary>Render the specified individual in html.</summary>
        /// <param name="query">Specifies the request query for this individual.</param>
        /// <returns>A html description of the specified individual.</returns>
        private string getIndividual(string query)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<p><a href=\"app://home\">Home</a></p>");

            NameValueCollection queryParams = HttpUtility.ParseQueryString(query);
            string idx = queryParams.Get("id");

            // Find the specified individual.
            Individual individual = _gedcom.individuals.find(idx);
            if (individual == null)
            {
                html.Append("<h1>Individual</h1>");
                html.Append("<p>query is '" + query + "'.</p>");
                html.Append("<p>Can not find '" + idx + "'.</p>");
            }
            else
            {
                // Remember which keys are dealt with.
                List<String> dealtWith = new List<String>();

                // Title the page with the individual name.
                string fullName = individual.fullName;
                dealtWith.Add("NAME");
                dealtWith.Add("SEX");
                html.Append("<h1>" + fullName + " (" + individual.idx + ")</h1>");

                // Initialise the sources referenced in this individual.
                HtmlSources htmlSources = new HtmlSources();
                dealtWith.Add("BIRT");
                Tag tag = individual.tag.children.findOne("BIRT");
                if (tag != null)
                {
                    html.Append(getTagLongHtml(tag, fullName, "was born", htmlSources));
                }

                // Deal with family.
                dealtWith.Add("FAMC");
                tag = individual.tag.children.findOne("FAMC");
                if (tag != null)
                {
                    html.Append(getTagLongParents(individual, tag, htmlSources));
                }

                // Deal with partners.
                dealtWith.Add("FAMS");
                Tag[] tags = individual.tag.children.findAll("FAMS");
                foreach (Tag marriageTag in tags)
                {
                    html.Append(getTagLongPartner(individual, marriageTag, individual.isMale ? "He" : "She", htmlSources));
                }

                // Deal with death.
                dealtWith.Add("DEAT");
                tag = individual.tag.children.findOne("DEAT");
                if (tag != null)
                {
                    html.Append(getTagLongHtml(tag, individual.isMale ? "He" : "She", "died", htmlSources));
                }

                // Deal with the sources.
                dealtWith.Add("SOUR");
                tags = individual.tag.children.findAll("SOUR");
                foreach (Tag sourceTag in tags)
                {
                    string sourceIdx = Tag.toIdx(sourceTag.value);
                    Source source = _gedcom.sources.find(sourceIdx);
                    int refIdx = htmlSources.add(source);
                }

                // Show some tags.
                dealtWith.Add("CHAN");
                addRemainingTags(html, individual.tag.children, dealtWith);

                // Show the referenced tags.
                html.Append(htmlSources.toHtml());

                // Show the last changed information.
                html.Append("<p>Last Changed " + individual.lastChanged.ToString() + "</p>");

                // Show the original gedcom.
                html.Append("<pre style=\"width: 400px; display: inline-block; vertical-align: top;\">" + individual.tag.display(0) + "</pre>");
            }

            // Return the built string as html.
            return _userOptions.renderHtml(html.ToString());
        }



        /// <summary>Returns the full name of the individual in html with a link.</summary>
        /// <param name="individual">Specifies the individual to display.</param>
        /// <returns>The html for the individual with the full name and a link.</returns>
        private string htmlIndividual(Individual individual)
        {
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
        private string getTagLongPartner(Individual individual, Tag familyTag, string proNoun, HtmlSources htmlSources)
        {
            // Find the family tag.
            string familyIdx = Tag.toIdx(familyTag.value);
            Family family = _gedcom.families.find(familyIdx);
            Tag tag = family.tag;

            // Build a long description of the tag.
            StringBuilder html = new StringBuilder();

            // Marriage
            Tag tagMarriage = tag.children.findOne("MARR");
            if (tagMarriage != null)
            {
                Tag tagDate = tagMarriage.children.findOne("DATE");
                if (tag != null)
                {
                    html.Append(firstCaps(getTagLongDate(tagDate, htmlSources)));
                    html.Append(" "+individual.heShe.ToLower() + " <a href=\"app://family?id=" + familyIdx + "\">married</a> ");
                }
                else
                {
                    html.Append(individual.heShe + " <a href=\"app://family?id=" + familyIdx + "\">married</a> ");
                }
            }
            else
            {
                html.Append(individual.heShe + " had a <a href=\"app://family?id=" + familyIdx + "\">relationship with</a> ");
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
            html.Append(".");

            // Return the long description.
            return html.ToString();
        }

        #endregion

        #region Family

        /// <summary>Render the specified family in html.</summary>
        /// <param name="query">Specifies the request query for this family.</param>
        /// <returns>A html description of the specified family.</returns>
        private string getFamily(string query)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<p><a href=\"app://home\">Home</a></p>");

            NameValueCollection queryParams = HttpUtility.ParseQueryString(query);
            string idx = queryParams.Get("id");

            Family family = _gedcom.families.find(idx);
            if (family == null)
            {
                html.Append("<h1>Family</h1>");
                html.Append("<p>query is '" + query + "'</p>");
                html.Append("<p>Can't find '" + idx + "'.</p>");
                html.Append(idx + " not found!");
            }
            else
            {
                html.Append("<h1>" + family.fullName + " (" + family.idx + ")</h1>");
                html.Append("<p>Last Changed " + family.lastChanged.ToString() + "</p>");

                // Show some tags.
                foreach (Tag tag in family.tag.children)
                {
                    html.Append("<p>'" + tag.line + "' = '" + tag.key + "' = '" + tag.value + "'</p>");
                }

                // Show the original gedcom.
                html.Append("<pre>" + family.tag.display(0) + "</pre>");
            }

            // Return the built string as html.
            return _userOptions.renderHtml(html.ToString());
        }

        #endregion

        #region Source

        /// <summary>Render the requested source as html.</summary>
        /// <param name="query">Specifies the request query for this source.</param>
        /// <returns>The requested source as html.</returns>
        private string getSource(string query)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<p><a href=\"app://home\">Home</a></p>");

            NameValueCollection queryParams = HttpUtility.ParseQueryString(query);
            string idx = queryParams.Get("id");

            Source source = _gedcom.sources.find(idx);
            if (source == null)
            {
                html.Append("<h1>Source</h1>");
                html.Append("<p>query is '" + query + "'</p>");
                html.Append("<p>Can't find '" + idx + "'.</p>");
                html.Append(idx + " not found!");
            }
            else
            {
                html.Append("<h1>" + source.fullName + " (" + source.idx + ")</h1>");
                html.Append("<p>Last Changed " + source.lastChanged.ToString() + "</p>");

                // Show some tags.
                foreach (Tag tag in source.tag.children)
                {
                    html.Append("<p>'" + tag.key + "' = '" + tag.value + "'</p>");
                }

                // Show the original gedcom.
                html.Append("<pre>" + source.tag.display(0) + "</pre>");
            }

            // Return the built string as html.
            return _userOptions.renderHtml(html.ToString());
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
