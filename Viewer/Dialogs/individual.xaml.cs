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
    /// <summary>Interaction logic for individual.xaml.</summary>
    public partial class DialogIndividual : Window
    {
        #region Member Variables

        /// <summary>The gedcom that contains the individual to edit.</summary>
        Gedcom _gedcom;

        #endregion

        #region Constructors

        /// <summary>Class constrcutor for the edit individual dialog.</summary>
        /// <param name="gedcom">Specifies the gedcom that contains the individual to edit.</param>
        public DialogIndividual(Gedcom gedcom, string query)
        {
            InitializeComponent();

            // Save the parameters.
            _gedcom = gedcom;

            // Get the index of the individual.
            NameValueCollection queryParams = HttpUtility.ParseQueryString(query);
            string idx = queryParams.Get("id");

            // Find the specified individual.
            Individual individual = _gedcom.individuals.find(idx);
            if (individual == null)
            {
                throw (new Exception("No individual found for '" + idx + "'"));
            }

            // Create rows for the tags.
            for (int i = 0; i < individual.tag.children.count; i++)
            {
                TagControl tagControl = new TagControl(individual.tag.children[i], false, setChildSize);
                tagControl.VerticalAlignment = VerticalAlignment.Top;
                
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(tagControl.Height);

                _mainGrid.RowDefinitions.Add(rowDefinition);
                _mainGrid.Children.Add(tagControl);
                Grid.SetRow(tagControl, i);
            }
        }



        /// <summary>Resize the space for each child row.</summary>
        /// <remarks>This is intended so that the children can inform this control when they change size.</remarks>
        private void setChildSize()
        {
            foreach (RowDefinition rowDefinition in _mainGrid.RowDefinitions)
            {
                rowDefinition.Height = new GridLength(0, GridUnitType.Auto);
            }
        }

        #endregion

        #region Signal Handlers

        private void buttonCancelClick(object sender, RoutedEventArgs e)
        {
            
            
        }

        private void buttonOkClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        #endregion
    }
}
