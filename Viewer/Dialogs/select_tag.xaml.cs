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

        /// <summary>The tag that is created by the dialog.</summary>
        private Tag _result;

        #endregion

        #region Constructor

        /// <summary>Default Constructor.</summary>
        private DialogSelectTag()
        {
            InitializeComponent();
            _result = null;
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
            if (parentTag != null)
            {
                _parentTag = parentTag.tag;
            }
            else
            {
                _parentTag = null;
            }
        }

        #endregion

        #region Properties

        /// <summary>The tag that is created by the dialog.</summary>
        public Tag result
        {
            get => _result;
        }

        #endregion

        #region Signal Handlers

        /// <summary>Signal handler for window loaded.</summary>
        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            TagType parentTagType = _isTopLevel ? new TagType(_parentTag.value)  :  new TagType(_parentTag.key);
            _labParentTagType.Text = parentTagType.tagName;
            __labParentTagValue.Text = _isTopLevel ? _parentTag.key :_parentTag.value;

            List<TagType> tagTypes = parentTagType.getChildren();

            foreach(TagType tagType in tagTypes)
            {
                bool isAdd = true;
                if (!tagType.isMultiple)
                {
                    // If there is already an extry of this type then don't add this type.
                    if (_parentTag.children.findOne(tagType.tagKey)!=null)
                    {
                        // Don't add this tag type to the list.
                        isAdd = false;
                    }
                }
                if (isAdd)
                {
                    _listSources.Items.Add(tagType);
                }
            }
        }



        /// <summary>Signal handler for the cancel button click.</summary>
        private void buttonCancelClick(object sender, RoutedEventArgs e)
        {
            // Result cancel button clicked.
            this.DialogResult = false;
        }



        /// <summary>Signal handler for the Ok button click.</summary>
        private void buttonOkClick(object sender, RoutedEventArgs e)
        {
            // Result OK button clicked.
            this.DialogResult = true;

            TagType tagType = (TagType)_listSources.SelectedItem;

            // Create a tag of the specified type.
            _result = new Tag(_parentTag, tagType.tagKey, "value");
        }

        #endregion

    }
}
