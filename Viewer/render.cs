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
    /// <summary>
    /// Class to render content.
    /// </summary>
    public class Render
    {
        private Gedcom _gedcom;

        public Render(Gedcom gedcom)
        {
            _gedcom = gedcom;
        }

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
            return getError(host, query);
        }

        /// <summary>Render the home page in html.</summary>
        /// <returns>The home page in html.</returns>
        private string getHome()
        {
            StringBuilder html = new StringBuilder();

            html.Append("<h1>" + _gedcom.fileName + "</h1>");

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
            // html.Append("</div>");

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


            return html.ToString();
        }

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
