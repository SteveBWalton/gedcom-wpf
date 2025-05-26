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

        /// <summary>The height for a single line of tag control.</summary>
        private const int LINE_HEIGHT = 19;

        /// <summary>Delegate to a function to request the parent control resizes the space for this control.</summary>
        public delegate void SetParentHeight();

        /// <summary>The tag to display.</summary>
        private Tag _tag;

        /// <summary>
        /// True if the child tags are expanded, false otherwise.
        /// </summary>
        private bool _isExpand;

        /// <summary>A function to request the parent control to resize the space for this control.</summary>
        private SetParentHeight _setParentHeight;

        #endregion

        #region Constructors

        /// <summary>Constructor for the edit tag user control.</summary>
        /// <param name="tag">Specifies the tag to edit.</param>
        /// <param name="isExpand">Specifies true to show the tag initially expanded.</param>
        public TagControl(Tag tag, bool isExpand, SetParentHeight setParentHeight)
        {
            InitializeComponent();

            // Record the parameters.
            _tag = tag;
            _isExpand = isExpand;
            _setParentHeight = setParentHeight;

            // Add a label for the tag.
            TextBlock textblockTag = new TextBlock();
            TagType tagType = new TagType(tag.key);
            // textblockTag.Text = tag.key;
            textblockTag.Text = tagType.tagName;
            textblockTag.TextAlignment = TextAlignment.Right;
            textblockTag.Margin = new Thickness(0, 0, 4, 0);
            textblockTag.VerticalAlignment = VerticalAlignment.Center;
            _mainGrid.Children.Add(textblockTag);
            Grid.SetRow(textblockTag, 0);
            Grid.SetColumn(textblockTag, 1);

            // Width for the control.
            const int TOTAL_SPACE = 400;
            int valueWidth = TOTAL_SPACE - 20 * tag.level;

            // Add a control for the tag value.
            switch (tag.key)
            {
            case "SOUR":
                // Add a combobox for the source.
                ComboBox sourceComboBox = new ComboBox();
                sourceComboBox.Width = valueWidth;
                Source[] sourcesInDateOrder = tag.gedcom.sources.inDateOrder();
                foreach (Source source in sourcesInDateOrder)
                {
                    sourceComboBox.Items.Add(source);
                }                
                _mainGrid.Children.Add(sourceComboBox);
                Grid.SetRow(sourceComboBox, 0);
                Grid.SetColumn(sourceComboBox, 2);
                break;

            default:
                // Add a textbox for the tag value.
                TextBox textBoxValue = new TextBox();
                textBoxValue.Text = tag.value;
                textBoxValue.Width = valueWidth;
                textBoxValue.VerticalAlignment = VerticalAlignment.Center;
                textBoxValue.LostFocus += TextBoxValue_LostFocus;
                _mainGrid.Children.Add(textBoxValue);
                Grid.SetRow(textBoxValue, 0);
                Grid.SetColumn(textBoxValue, 2);
                break;
            }

            // Set the height of this line.
            this.Height = LINE_HEIGHT;

            // Show or hide the plus minus image.
            if (_tag.children.count == 0)
            {
                _imagePlusMinus.Visibility = Visibility.Hidden;
            }
            else
            {
                _imagePlusMinus.Visibility = Visibility.Visible;

                // Show the children.
                for (int i = 0; i < _tag.children.count; i++)
                {
                    TagControl tagControl = new TagControl(_tag.children[i], _isExpand, setChildSize);
                    tagControl.VerticalAlignment = VerticalAlignment.Top;

                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = new GridLength(tagControl.Height);

                    _childGrid.RowDefinitions.Add(rowDefinition);
                    _childGrid.Children.Add(tagControl);
                    Grid.SetRow(tagControl, i);
                    if (_isExpand)
                    {
                        this.Height += tagControl.Height;
                    }
                }
                // Hide or show the children.
                if (_isExpand)
                {
                    _childGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    _childGrid.Visibility = Visibility.Collapsed;
                }
            }

            // Request the parent resize the space for this control.
            setParentHeight?.Invoke();
        }



        /// <summary>Signal handler for the default text box losing the focus.</summary>
        /// <remarks>If the value has changed then update the tag.</remarks>
        private void TextBoxValue_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (textBox.Text != _tag.value)
                {
                    _tag.value = textBox.Text;
                }
            }
        }



        /// <summary>Add a child control for the specified child tag to the control.</summary>
        /// <param name="childTag">Specifies the child tag to add to the control.</param>
        private void addChildControl(Tag childTag)
        {
            _imagePlusMinus.Visibility = Visibility.Visible;

            TagControl tagControl = new TagControl(childTag, _isExpand, setChildSize);
            tagControl.VerticalAlignment = VerticalAlignment.Top;

            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(tagControl.Height);

            _childGrid.RowDefinitions.Add(rowDefinition);
            _childGrid.Children.Add(tagControl);
            Grid.SetRow(tagControl, _tag.children.Count());
        }



        /// <summary>Resize the space for each child row.</summary>
        /// <remarks>This is intended so that the children can inform this control when they change size.</remarks>
        private void setChildSize()
        {
            foreach (RowDefinition rowDefinition in _childGrid.RowDefinitions)
            {
                rowDefinition.Height = new GridLength(0, GridUnitType.Auto);
            }
        }


        #endregion

        #region Properties

        #endregion

        /// <summary>Signal handler for the show hide child tags button click.  /// </summary>
        private void buttonPlusMinusClick(object sender, RoutedEventArgs e)
        {
            _isExpand = !_isExpand;
            if (_isExpand)
            {
                // Show the children.
                _imagePlusMinus.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/16/minus.png"));
                _childGrid.Visibility = Visibility.Visible;
                this.Height = LINE_HEIGHT + _childGrid.Height;
            }
            else
            {
                // Hide the children.
                _imagePlusMinus.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/16/add.png"));
                _childGrid.Visibility = Visibility.Collapsed;
                _setParentHeight?.Invoke();
                this.Height = LINE_HEIGHT;
            }
        }



        /// <summary>Signal handler for the add child tag button click.</summary>
        private void addChildTagButtonClick(object sender, RoutedEventArgs e)
        {
            DialogSelectTag dialogSelectTag = new DialogSelectTag(_tag);
            if (dialogSelectTag.ShowDialog() == true)
            {
                // Add the tag to this tag as a child.
                _tag.children.add(dialogSelectTag.result);

                // Add a child control for this tag.
                addChildControl(dialogSelectTag.result);

                // Display the child controls.
                if (_isExpand)
                {
                    // Increase the height of this control.
                    this.Height += LINE_HEIGHT;
                }
                else
                {
                    // Put the control into expanded state.
                    buttonPlusMinusClick(sender, null);
                }

                // Refresh the dialog.
                _setParentHeight?.Invoke();
            }
        }
    }
}
