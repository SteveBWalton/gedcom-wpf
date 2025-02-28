using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom.viewer
{
    /// <summary>Class to represent the contents on a page.</summary>
    public class PageContent
    {
        #region Member Variables

        /// <summary>The contents of the page in html.</summary>
        private readonly StringBuilder _html;

        /// <summary>The command to edit the gedcom directly.</summary>
        private string _editGedomDirectly;

        /// <summary>The command to edit the object via a custom dialog.</summary>
        private string _editForm;

        #endregion

        #region Class Constructors

        /// <summary>Class constructor for the PageContents class.</summary>
        public PageContent()
        {
            _html = new StringBuilder();
            _editGedomDirectly = "";
            _editForm = "";
        }

        #endregion

        #region Properties

        /// <summary>The contents of the page in html./ </summary>
        public StringBuilder html
        {
            get => _html;
        }



        /// <summary>The command to edit the gedcom directly.</summary>
        public string editGedomDirectly
        {
            get => _editGedomDirectly;
            set { _editGedomDirectly = value; }
        }



        /// <summary>The command to edit the object via a custom dialog.</summary>
        public string editForm
        {
            get => _editForm;
            set { _editForm = value; }
        }



        #endregion
    }
}
