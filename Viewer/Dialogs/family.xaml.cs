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
    /// <summary>Class to represent a dialog to edit a family object.</summary>
    public partial class DialogFamily : Window
    {
        #region Member Variables

        /// <summary>The gedcom that contains the individual to edit.</summary>
        private readonly Gedcom _gedcom;

        /// <summary>The family that the dialog is editting.</summary>
        private Family _family;

        /// <summary>The collection of tag controls that make this family.</summary>
        private List<TagControl> _tagControls;

        #endregion

        #region Constructors

        /// <summary>Constructor for the edit family dialog to create a new family.</summary>
        /// <param name="gedcom">Specifies the gedcom to add the new family to.</param>
        public DialogFamily(Gedcom gedcom)
        {
            InitializeComponent();

            // Save the parameters.
            _gedcom = gedcom;
            _family = new Family(gedcom);
            _tagControls = new List<TagControl>();
        }

        /// <summary>Constructor for the edit family dialog to edit an existing family.</summary>
        /// <param name="gedcom">Specifies the gedcom that contains the family.</param>
        /// <param name="query">Specifies a query that contains an ID to identify the family.</param>
        public DialogFamily(Gedcom gedcom, string query) : this(gedcom)
        {
            // Get the index of the individual.
            NameValueCollection queryParams = HttpUtility.ParseQueryString(query);
            string idx = queryParams.Get("id");

            // Find the specified individual.
            _family = _gedcom.families.find(idx);
            if (_family == null)
            {
                throw (new Exception("No family found for '" + idx + "'"));
            }

            // Create rows for the tags.
            for (int i = 0; i < _family.tag.children.count; i++)
            {
                TagControl tagControl = new TagControl(_family.tag.children[i], false, setChildSizeIndividual, askParentDelete);
                tagControl.VerticalAlignment = VerticalAlignment.Top;
                _tagControls.Add(tagControl);

                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(tagControl.Height);

                _gridTags.RowDefinitions.Add(rowDefinition);
                _gridTags.Children.Add(tagControl);
                Grid.SetRow(tagControl, i);
            }
        }

        #endregion

        /// <summary>Resize the space for each child row.</summary>
        /// <remarks>This is intended so that the children can inform this control when they change size.</remarks>
        private void setChildSizeIndividual()
        {
            foreach (RowDefinition rowDefinition in _gridTags.RowDefinitions)
            {
                rowDefinition.Height = new GridLength(0, GridUnitType.Auto);
            }
        }



        /// <summary>The child tag control has requested to be deleted.</summary>
        /// <param name="tagControl">Specifies the child tag control that has requested to be deleted.</param>
        private void askParentDelete(TagControl tagControl)
        {
            int i = 0;
            while (i < _tagControls.Count)
            {
                if (_tagControls[i] == tagControl)
                {
                    // Delete the row.
                    // This doesn't change the row index of the other tag controls.
                    _gridTags.RowDefinitions.RemoveAt(i);

                    // Delete control.
                    tagControl.delete();

                    // Remove from collection.
                    _tagControls.RemoveAt(i);
                }
                else
                {
                    Grid.SetRow(_tagControls[i], i);
                    i++;
                }
            }
        }

        #region Signal Handlers

        /// <summary>Signal handler for the OK button click.</summary>
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
            DialogSelectTag dialogSelectTag = new DialogSelectTag(_family);
            if (dialogSelectTag.ShowDialog() == true)
            {

            }

            #endregion
        }
    }
}
