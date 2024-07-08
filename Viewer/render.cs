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
            if (host == "individual")
            {
                return getIndividual(query);
            }
            return getError(host, query);
        }

        private string getHome()
        {
            StringBuilder html = new StringBuilder();

            html.Append("<h1>Hello World</h1>");
            html.Append("<p><a href=\"https://news.bbc.co.uk\">bbc</a></p>");

            int count = 0;
            foreach (Individual individual in _gedcom.individuals)
            {
                html.Append("<p><a href=\"app://individual?id=" + count.ToString() + "\">" + count.ToString() + "</a> - " + individual.tags.count.ToString());

                html.Append("</p>");
                count++;
            }

            return html.ToString();
        }

        private string getIndividual(string query)
        {
            StringBuilder html = new StringBuilder();

            html.Append("<h1>Individual</h1>");
            html.Append("<p>query is '" + query + "'</p>");
            NameValueCollection queryParams = HttpUtility.ParseQueryString(query);
            string idx = queryParams.Get("id");
            html.Append("<p>id is '" + idx + "'</p>");
            string missing = queryParams.Get("missing");
            html.Append("<p>missing is '" + missing + "'</p>");
            html.Append("<p><a href=\"app://home\">Home</a></p>");

            // This will be some like of lookup in future.
            int individualIdx = int.Parse(idx);

            Individual individual = _gedcom.individuals[individualIdx];
            html.Append("<pre>" + individual.tag.display(0) + "</pre>");

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
