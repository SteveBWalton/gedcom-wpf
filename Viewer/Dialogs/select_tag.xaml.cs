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

        // <summary>The parent tag that will own this new tag.</summary>
        // private readonly Tag _parentTag;

        private string _parentTagKey;
        private string _parentTagValue;
        private string[] _childKeys;

        /// <summary>The tag that is created by the dialog.</summary>
        // private Tag _result;
        private string _result;

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
            _parentTagKey = parentTag.key;
            _parentTagValue = parentTag.value;
            List<string> keys = new List<string>();
            foreach (Tag child in parentTag.children)
            {
                keys.Add(child.key);
            }
            _childKeys = keys.ToArray();
        }



        /// <summary>Constructor for a parent tag control with no actual tag.</summary>
        /// <param name="tagKey"></param>
        /// <param name="tagValue"></param>
        /// <param name="childKeys"></param>
        public DialogSelectTag(string tagKey, string tagValue, string[] childKeys) : this()
        {
            _isTopLevel = false;
            _parentTagKey = tagKey;
            _parentTagValue = tagValue;
            _childKeys = childKeys;
        }



        /// <summary>Constructor for top level parent tag.</summary>
        /// <param name="parentTag">Specifies the parent top level tag.</param>
        public DialogSelectTag(TopLevel parentTag) : this()
        {
            _isTopLevel = true;
            if (parentTag != null)
            {
                _parentTagValue = parentTag.tag.value;
                _parentTagKey = parentTag.tag.key;
                List<string> keys = new List<string>();
                foreach (Tag child in parentTag.tag.children)
                {
                    keys.Add(child.key);
                }
                _childKeys = keys.ToArray();

            }
            else
            {
                // _parentTag = null;
                _parentTagValue = "";
                _parentTagKey = "";
                _childKeys = new string[0];
            }
        }

        #endregion

        #region Properties

        /// <summary>The tag key that is created by the dialog.</summary>
        public string result
        {
            get => _result;
        }

        #endregion

        #region Signal Handlers

        /// <summary>Signal handler for window loaded.</summary>
        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            TagType parentTagType = _isTopLevel ? new TagType(_parentTagValue)  :  new TagType(_parentTagKey);
            _labParentTagType.Text = parentTagType.tagName;
            _labParentTagValue.Text = _isTopLevel ? _parentTagKey :_parentTagValue;

            List<TagType> tagTypes = parentTagType.getChildren();

            foreach(TagType tagType in tagTypes)
            {
                bool isAdd = true;
                if (!tagType.isMultiple)
                {
                    // If there is already an extry of this type then don't add this type.
                    if (_childKeys.Contains(tagType.tagKey))
                    //if (_parentTag.children.findOne(tagType.tagKey)!=null)
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
            // _result = new Tag(_parentTag, tagType.tagKey, "value");
            _result = tagType.tagKey;
        }

        #endregion

    }
}
