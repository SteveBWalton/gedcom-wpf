using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
// This requires an additional reference to System.Web in references.
// HttpUtility
using System.Web;
// NameValueCollection.
using System.Collections.Specialized;

namespace gedcom.viewer
{
    /// <summary>Class to represent the edit source dialog.</summary>
    public partial class DialogSource : Window
    {
        #region Member Variables

        /// <summary>The gedcom that contains the source to edit.</summary>
        private readonly Gedcom _gedcom;

        /// <summary>The source that the dialog is editting.</summary>
        private Source _source;

        /// <summary>The collection of tag controls that make this source.</summary>
        private List<TagControl> _tagControls;

        #endregion

        #region Constructors

        /// <summary>Constructor for the edit source dialog to create a new source.</summary>
        /// <param name="gedcom">Specifies the gedcom that contains the individual to edit.</param>
        public DialogSource(Gedcom gedcom)
        {
            InitializeComponent();

            // Save the parameters.
            _gedcom = gedcom;
            _source = new Source(gedcom);
            _tagControls = new List<TagControl>();
        }



        /// <summary>Constrcutor for the edit individual dialog to edit an existing source.</summary>
        /// <param name="gedcom">Specifies the gedcom that contains the individual to edit.</param>
        /// <param name="query">Specifies the query string which contains an ID key to identifiy the individual.</param>
        public DialogSource(Gedcom gedcom, string query):this(gedcom)
        {
            // Get the idx of the source.
            NameValueCollection queryParams = HttpUtility.ParseQueryString(query);
            string idx = queryParams.Get("id");

            // Find the specified individual.
            _source = _gedcom.sources.find(idx);
            if (_source == null)
            {
                throw (new Exception("No source found for '" + idx + "'"));
            }
        }

        #endregion

        #region Signal Handlers

        /// <summary>Signal handler for the window loaded event.</summary>
        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            // Populate the dialog with the source.
            Tag tagDate = _source.tag.children.findOne("DATE");
            if (tagDate != null)
            {
                _txtDate.Text = tagDate.value;
            }
        }



        /// <summary>Signal handler for the OK button click event.</summary>
        private void buttonOkClick(object sender, RoutedEventArgs e)
        {
            // Close the dialog with okay.
            this.DialogResult = true;
        }



        /// <summary>Signal handler for the cancel button click.</summary>
        private void buttonCancelClick(object sender, RoutedEventArgs e)
        {

        }



        /// <summary>Signal handler for the add tag button click.</summary>
        private void addTagButtonClick(object sender, RoutedEventArgs e)
        {

        }

        #endregion

    }
}
