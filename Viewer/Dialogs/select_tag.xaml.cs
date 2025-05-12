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
    /// <summary>Class to represent a dialog to select tag.</summary>
    public partial class DialogSelectTag : Window
    {
        #region Member Variables

        /// <summary>True if the parent tag is a top level tag.</summary>
        private bool _isTopLevel;

        /// <summary>The parent tag that will own this new tag.</summary>
        private readonly Tag _parentTag;

        #endregion

        #region Constructor

        /// <summary>Default Constructor.</summary>
        private DialogSelectTag()
        {
            InitializeComponent();
        }



        /// <summary>Constructor for regualar parent tag.</summary>
        /// <param name="parentTag">Specifies the regular parent tag.</param>
        public DialogSelectTag(Tag parentTag) : this()
        {
            _isTopLevel = false;
            _parentTag = parentTag;
        }



        /// <summary>Constructor for top level parent tag.</summary>
        /// <param name="parentTag">Specifies the parent top level tag.</param>
        public DialogSelectTag(TopLevel parentTag) : this()
        {
            _isTopLevel = true;
            _parentTag = parentTag.tag;
        }

        #endregion

        #region Signal Handlers

        /// <summary>Signal handler for window loaded.</summary>
        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            // Every tag can have a child source tag.
            _listSources.Items.Add("Source");
        }



        /// <summary>Signal handler for the cancel button click.</summary>
        private void buttonCancelClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }



        /// <summary>Signal handler for the Ok button click.</summary>
        private void buttonOkClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        #endregion

    }
}
