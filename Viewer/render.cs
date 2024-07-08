using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string getContent(string url)
        {
            if (url.StartsWith("home"))
            {
                return getHome();
            }
            if (url.StartsWith("individual"))
            {
                return getIndividual(url);
            }
            return getError(url);
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

        private string getIndividual(string url)
        {
            StringBuilder html = new StringBuilder();

            html.Append("<h1>Individual</h1>");
            html.Append("<p>Url is '" + url + "'</p>");
            html.Append("<p><a href=\"app://home\">Home</a></p>");

            return html.ToString();
        }

        private string getError(string url)
        {
            StringBuilder html = new StringBuilder();

            html.Append("<h1>Error</h1>");
            html.Append("<p>Url is '" + url + "'</p>");
            html.Append("<p><a href=\"app://home\">Home</a></p>");

            return html.ToString();
        }
    }
}
