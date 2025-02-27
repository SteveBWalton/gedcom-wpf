using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom.viewer
{
    /// <summary>
    /// Class to represent the content on a page.
    /// </summary>
    public class PageContent
    {
        #region Member Variables

        /// <summary>The contents of the page in html.</summary>
        private readonly StringBuilder _html;

        #endregion

        #region Class Constructors

        public PageContent()
        {
            _html = new StringBuilder();
        }

        #endregion

        #region Properties

        public StringBuilder html
        {
            get => _html;
        }

        #endregion


    }
}
