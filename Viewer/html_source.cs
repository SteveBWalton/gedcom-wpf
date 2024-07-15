using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom.viewer
{
    /// <summary>Class to represent a reference to a source.  The sources are reported at the end and referenced during the html build.</summary>
    public class HtmlSource
    {
        #region Member Variables

        /// <summary>The actual source that this HtmlSource references.</summary>
        private Source _source;
        /// <summary>The index of this source reference in the collection.</summary>
        private int _idx;

        #endregion

        #region Constructors



        /// <summary>Class constructor.</summary>
        /// <param name="source">Specifies the actual source that is attached to this html source.</param>
        public HtmlSource(Source source, int idx)
        {
            _source = source;
            _idx = idx;
        }



        #endregion

        #region Properties



        /// <summary>The gedcom that contains this html source.</summary>
        public Gedcom gedcom
        {
            get { return _source.gedcom; }
        }
        


        /// <summary>The actual source that this HtmlSource references.</summary>
        public Source source
        {
            get { return _source; }
        }



        /// <summary>The index of this reference in the collection.</summary>
        public int idx
        {
            get { return _idx; }
        }



        #endregion
    }
}
