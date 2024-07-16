using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// IEnumerable.
using System.Collections;

namespace gedcom.viewer
{
    /// <summary>The collection of sources that are referenced.</summary>
    public class HtmlSources : IEnumerable<HtmlSource>
    {
        #region Member Variables

        /// <summary>The collection of html sources.</summary>
        private List<HtmlSource> _htmlSources;

        #endregion

        #region Class Constructor

        /// <summary>Empty class constructor.</summary>
        public HtmlSources()
        {
            clear();
        }

        #endregion

        #region List



        /// <summary>Empty the collection.</summary>
        /// <returns>True for success, false otherwise.</returns>
        public bool clear()
        {
            _htmlSources = new List<HtmlSource>();
            return true;
        }



        /// <summary>The numbers of html sources in the collection.</summary>
        public int count
        {
            get { return _htmlSources.Count; }
        }



        /// <summary>Add an html source to the collection if not already represent.</summary>
        /// <param name="htmlSource">Specifies the html source to add to the collection.</param>
        /// <returns>Returns the index of the source in the collection.</returns>
        public int add(Source source)
        {
            HtmlSource htmlSource = _htmlSources.Find(searchSource => searchSource.source == source);
            if (htmlSource != null)
            {
                return htmlSource.idx;
            }
            int idx = _htmlSources.Count;
            htmlSource = new HtmlSource(source, idx);
            _htmlSources.Add(htmlSource);
            return idx;
        }



        /// <summary>An indexer for this class.</summary>
        /// <param name="idx">Specifies the index of the individual [0..count-1].</param>
        /// <returns>The tag at the specified position.</returns>
        public HtmlSource this[int idx]
        {
            get { return (HtmlSource)_htmlSources[idx]; }
        }



        #endregion

        #region IEnumerable<HtmlSource>



        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public IEnumerator<HtmlSource> GetEnumerator()
        {
            for (int idx = 0; idx < _htmlSources.Count; idx++)
            {
                yield return (HtmlSource)_htmlSources[idx];
            }
        }



        #endregion

        /// <summary>Return the html source with the specified index.</summary>
        /// <param name="idx">Specifies the index to search for.</param>
        /// <returns>The html source with the specified index or null.</returns>
        public HtmlSource find(string idx)
        {
            foreach (HtmlSource htmlSource in this)
            {
                if (htmlSource.source.idx == idx)
                {
                    return htmlSource;
                }
            }
            return null;
        }



        /// <summary>Show the referenced sources as a html table.</summary>
        /// <returns>The html to show the referenced sources in a table.</returns>
        public string toHtml()
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<table class=\"sourceref\">");
            int idx = 0;
            foreach (HtmlSource htmlSource in this)
            {
                html.Append("<tr>");
                html.Append("<td>" + Convert.ToChar('A' + idx) + "</td>");
                html.Append("<td>" + htmlSource.source.fullName + "</td>");
                html.AppendLine("<tr>");
                idx++;
            }
            html.AppendLine("</table>");
            // Return the built html.
            return html.ToString();
        }



    }
}
