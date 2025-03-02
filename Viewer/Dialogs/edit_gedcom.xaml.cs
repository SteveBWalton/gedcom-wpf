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
        /// <summary>The tag to edit.</summary>
        private Tag _tag;


        /// <summary>Class constructor for the edit gedcom dialog.</summary>
        public DialogEditGedcom(Tag tag)
        {
            InitializeComponent();
            _tag = tag;
            _statusTextBox.Text = tag.toText();
        }

        #region Signal Handlers

        /// <summary>Signal handler for the cancel button.</summary>
        private void buttonCancelClick(object sender, RoutedEventArgs e)
        {
            // Close the dialog with cancel.
            this.DialogResult = false;
        }



        /// <summary>Signal handler for the okay button.</summary>
        private void buttonOkClick(object sender, RoutedEventArgs e)
        {
            // Remove the existing tags.
            _tag.children.clear();

            // Add the tags from the gedcom text.
            bool isFirst = true;
            using (System.IO.StringReader stringReader = new System.IO.StringReader(_statusTextBox.Text))
            {
                string line;
                while((line=stringReader.ReadLine())!=null)
                {
                    if (isFirst)
                    {
                        // Ignore the first line.
                        isFirst = false;
                    }
                    else if (line != "")
                    {
                        _tag.add(line);
                    }
                }
            }

            // Close the dialog with okay.
            this.DialogResult = true;
        }

        #endregion
    }
}
