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
        public DialogIndividual(Gedcom gedcom)
        {
            InitializeComponent();

            // Save the parameters.
            _gedcom = gedcom;
        }

        #endregion
    }
}
