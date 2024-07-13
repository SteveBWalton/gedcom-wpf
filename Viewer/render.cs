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

        #endregion

        #region Class Constructors

        /// <summary>Class constructor.</summary>
        /// <param name="gedcom">Specifies the gedcom that this will render.</param>
        public Render(Gedcom gedcom)
        {
            _gedcom = gedcom;
        }

        #endregion

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
                html.Append("<tr><td><a href=\"app://individual?id=" + individual.idx + "\">" + individual.fullName + "</a></td><tr>");
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
                html.Append("<tr><td><a href=\"app://source?id=" + source.idx + "\">" + source.fullName + "</a></td></tr>");
                count++;
                if (count >= 10)
                {
                    break;
                }
            }
            html.Append("</table>");
            html.Append("<p>There are " + _gedcom.sources.count.ToString() + " sources.");
            html.Append("</fieldset>");

            return html.ToString();
        }



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
                html.Append("<h1>" + individual.fullName + " (" + individual.idx + ")</h1>");
                html.Append("<p>Last Changed " + individual.lastChanged.ToString() + "</p>");

                // Show some tags.
                foreach (Tag tag in individual.tag.children)
                {
                    html.Append("<p>'" + tag.line + "' = '" + tag.key + "' = '" + tag.value + "'</p>");
                }

                // Show the original gedcom.
                html.Append("<pre>" + individual.tag.display(0) + "</pre>");
            }

            return html.ToString();
        }



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

            return html.ToString();
        }



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

            // Return the built html.
            return html.ToString();
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

            return html.ToString();
        }
    }
}
