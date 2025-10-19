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
    /// <summary>Class to represent the custom dialog to edit an individual.</summary>
    public partial class DialogIndividual : Window
    {
        #region Member Variables

        /// <summary>The gedcom that contains the individual to edit.</summary>
        private readonly Gedcom _gedcom;

        /// <summary>The individual that the dialog is editting.</summary>
        private Individual _individual;

        /// <summary>The collection of tag controls that make this individual.</summary>
        private List<TagControl> _tagControls;

        #endregion

        #region Constructors

        /// <summary>Constructor for the edit individual dialog to create a new individual.</summary>
        /// <param name="gedcom">Specifies the gedom to add the individual to.</param>
        public DialogIndividual(Gedcom gedcom)
        {
            InitializeComponent();

            // Save the parameters.
            _gedcom = gedcom;
            _individual = new Individual(gedcom);
            _tagControls = new List<TagControl>();
        }



        /// <summary>Constrcutor for the edit individual dialog to edit an existing individual.</summary>
        /// <param name="gedcom">Specifies the gedcom that contains the individual to edit.</param>
        /// <param name="query">Specifies the query string which contains an ID key to identifiy the individual.</param>
        public DialogIndividual(Gedcom gedcom, string query) : this(gedcom)
        {
            // Get the index of the individual.
            NameValueCollection queryParams = HttpUtility.ParseQueryString(query);
            string idx = queryParams.Get("id");

            // Find the specified individual.
            _individual = _gedcom.individuals.find(idx);
            if (_individual == null)
            {
                throw (new Exception("No individual found for '" + idx + "'"));
            }

            // Create rows for the tags.
            for (int i = 0; i < _individual.tag.children.count; i++)
            {
                TagControl tagControl = new TagControl(_individual.tag.children[i], false, setChildSizeIndividual, askParentDelete, null);
                tagControl.VerticalAlignment = VerticalAlignment.Top;
                _tagControls.Add(tagControl);

                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(tagControl.Height);

                _mainGrid.RowDefinitions.Add(rowDefinition);
                _mainGrid.Children.Add(tagControl);
                Grid.SetRow(tagControl, i);
            }
        }



        /// <summary>Resize the space for each child row.</summary>
        /// <remarks>This is intended so that the children can inform this control when they change size.</remarks>
        private void setChildSizeIndividual()
        {
            foreach (RowDefinition rowDefinition in _mainGrid.RowDefinitions)
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
                    _mainGrid.RowDefinitions.RemoveAt(i);

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

        #endregion

        #region Properties

        /// <summary>The individual that the dialog is editting.</summary>
        public Individual individual
        {
            get => _individual;
        }

        #endregion

        #region Signal Handlers

        /// <summary>Signal handler for the cancel button click.</summary>
        private void buttonCancelClick(object sender, RoutedEventArgs e)
        {
        }



        /// <summary>Signal handler for the OK button click.</summary>
        private void buttonOkClick(object sender, RoutedEventArgs e)
        {
            // Might sort these into a better order.
            // Get the value of each tag control.
            StringBuilder tagsAsText = new StringBuilder();
            foreach(TagControl tagControl in _tagControls)
            {
                tagsAsText.Append(tagControl.ToString());
            }

            // Remove the existing tags.
            _individual.tag.children.clear();

            // Add the tags from the tag controls text.
            using (System.IO.StringReader stringReader = new System.IO.StringReader(tagsAsText.ToString()))
            {
                string line;
                while ((line = stringReader.ReadLine()) != null)
                {
                    if (line != "")
                    {
                        _individual.tag.add(line);
                    }
                }
            }

            // Update the last changed tag.
            _individual.setLastChanged();

            // Close the dialog with okay.
            this.DialogResult = true;
        }



        /// <summary>Signal handler for the add tag button click.</summary>
        private void addTagButtonClick(object sender, RoutedEventArgs e)
        {
            // Allow the user to select the tag to add to the dialog.
            DialogSelectTag dialogSelectTag = new DialogSelectTag(_individual);
            if (dialogSelectTag.ShowDialog() == true)
            {
                // Add a new tag control to the dialog.                
                TagControl tagControl = new TagControl(dialogSelectTag.result, "", 1, null, _gedcom, false, setChildSizeIndividual, askParentDelete, null);
                tagControl.VerticalAlignment = VerticalAlignment.Top;
                _tagControls.Add(tagControl);

                // Add a new row to the dialog.
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(tagControl.Height);

                // Add the new tag control to the new row.
                _mainGrid.RowDefinitions.Add(rowDefinition);
                _mainGrid.Children.Add(tagControl);
                Grid.SetRow(tagControl, _mainGrid.RowDefinitions.Count - 1);
            }
        }

        #endregion
    }
}
