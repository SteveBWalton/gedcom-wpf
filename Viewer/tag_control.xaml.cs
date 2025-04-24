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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace gedcom.viewer
{
    /// <summary>Class to represent a user control to edit a gedcom tag.</summary>
    public partial class TagControl : UserControl
    {
        #region Member Variables

        /// <summary>The tag to display.</summary>
        private Tag _tag;

        /// <summary>
        /// True if the child tags are expanded, false otherwise.
        /// </summary>
        private bool _isExpand;

        #endregion

        #region Constructors

        /// <summary>Constructor for the edit tag user control.</summary>
        /// <param name="tag">Specifies the tag to edit.</param>
        /// <param name="isExpand">Specifies true to show the tag initially expanded.</param>
        public TagControl(Tag tag, bool isExpand)
        {
            InitializeComponent();

            // Record the parameters.
            _tag = tag;
            _isExpand = isExpand;

            const int LINE_HEIGHT = 17;

            // Add a master row for the actual tag.
            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(LINE_HEIGHT);
            _mainGrid.RowDefinitions.Add(rowDefinition);

            TextBlock textblockTag = new TextBlock();
            textblockTag.Text = tag.key;
            textblockTag.TextAlignment = TextAlignment.Right;
            textblockTag.Margin = new Thickness(0, 0, 4, 0);
            _mainGrid.Children.Add(textblockTag);
            Grid.SetRow(textblockTag, 0);
            Grid.SetColumn(textblockTag, 1);

            TextBox textBoxValue = new TextBox();
            textBoxValue.Text = tag.value;
            textBoxValue.Width = 200;
            _mainGrid.Children.Add(textBoxValue);
            Grid.SetRow(textBoxValue, 0);
            Grid.SetColumn(textBoxValue, 2);

            this.Height = LINE_HEIGHT;

            if (_isExpand)
            {
                // Create rows for the child tags.
                for (int i = 0; i < _tag.children.count; i++)
                {
                    TagControl tagControl = new TagControl(_tag.children[i], true);

                    rowDefinition = new RowDefinition();
                    rowDefinition.Height = new GridLength(tagControl.Height);

                    _childGrid.RowDefinitions.Add(rowDefinition);
                    _childGrid.Children.Add(tagControl);
                    Grid.SetRow(tagControl, i);

                    this.Height += tagControl.Height;
                }
            }
        }

        #endregion

        #region Properties

        #endregion 

    }
}
