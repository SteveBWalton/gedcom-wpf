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
    /// <summary>Class to present a dialog to edit gedcom as text.</summary>
    /// <remarks>This can be a fall back editor when a specific editor is not available.</remarks>
    public partial class DialogEditGedcom : Window
    {
        private string _text;


        /// <summary>Class constructor for the edit gedcom dialog.</summary>
        public DialogEditGedcom(string text)
        {
            InitializeComponent();
            _text = text;
            _statusTextBox.Text = text;
        }

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
