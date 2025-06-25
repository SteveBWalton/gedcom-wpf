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
        /// <param name="setParentHeight">Specifies a function to resize the parent container.</param>
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
            const int TOTAL_SPACE = 500;
            int valueWidth = TOTAL_SPACE - 20 * tag.level;
            // Width for helper buttons.
            const int BUTTON_WIDTH = 30;

            // Add a control for the tag value.
            switch (tag.key)
            {
            case "SOUR":
                // Add a combobox for the source.
                ComboBox sourceComboBox = new ComboBox();
                sourceComboBox.Width = valueWidth;
                Source[] sourcesInDateOrder = tag.gedcom.sources.inDateOrder();
                string sourceIdx = gedcom.Tag.toIdx(tag.value);
                foreach (Source source in sourcesInDateOrder)
                {
                    sourceComboBox.Items.Add(source);
                    if (source.idx == sourceIdx)
                    {
                        sourceComboBox.SelectedIndex = sourceComboBox.Items.Count - 1;
                    }
                }
                _mainGrid.Children.Add(sourceComboBox);
                Grid.SetRow(sourceComboBox, 0);
                Grid.SetColumn(sourceComboBox, 2);
                break;

            case "FAMC": // Family Child.
            case "FAMS": // Family Spouse.
                // Add a combobox for the family.
                ComboBox spouseComboBox = new ComboBox();
                spouseComboBox.Width = valueWidth;
                Family[] familiesInDateOrder = tag.gedcom.families.inDateOrder();
                string familyIdx = gedcom.Tag.toIdx(tag.value);
                foreach (Family family in familiesInDateOrder)
                {
                    spouseComboBox.Items.Add(family);
                    if (family.idx == familyIdx)
                    {
                        spouseComboBox.SelectedIndex = spouseComboBox.Items.Count - 1;
                    }
                }
                _mainGrid.Children.Add(spouseComboBox);
                Grid.SetRow(spouseComboBox, 0);
                Grid.SetColumn(spouseComboBox, 2);
                break;

            case "NAME": // Name
            case "BIRT": // Birth
            case "CHAN": // Last Change.
            case "_PGVU": // Last Change by.
                // Add a text block for the read-only.
                TextBlock textblockReadOnly = new TextBlock();
                textblockReadOnly.Width = valueWidth;
                textblockReadOnly.Height = LINE_HEIGHT - 2;
                textblockReadOnly.VerticalAlignment = VerticalAlignment.Center;
                textblockReadOnly.Padding = new Thickness(4, 0, 0, 0);
                textblockReadOnly.Background = Brushes.LightGray;
                if (tag.key == "BIRT" || tag.key == "CHAN")
                {
                    tag.value = "Y";
                }
                textblockReadOnly.Text = tag.value;
                _mainGrid.Children.Add(textblockReadOnly);
                Grid.SetRow(textblockReadOnly, 0);
                Grid.SetColumn(textblockReadOnly, 2);
                break;

            case "DATE":
                // Add an extra button for additional date options.
                StackPanel dateStackPanel = new StackPanel();
                dateStackPanel.Orientation = Orientation.Horizontal;
                // Add a textbox for the tag value.
                TextBox textDateValue = new TextBox();
                textDateValue.Text = tag.value;
                textDateValue.Width = valueWidth - BUTTON_WIDTH;
                textDateValue.VerticalAlignment = VerticalAlignment.Center;
                textDateValue.LostFocus += textBoxValueLostFocus;
                dateStackPanel.Children.Add(textDateValue);
                // Add a button for additional date options.
                Button dateButton = new Button();
                dateButton.Content = "...";
                dateButton.Width = BUTTON_WIDTH;
                dateButton.VerticalAlignment = VerticalAlignment.Center;
                dateButton.Tag = textDateValue;
                dateButton.Click += dateButtonClick;
                dateStackPanel.Children.Add(dateButton);
                // Add stack panel to grid.
                _mainGrid.Children.Add(dateStackPanel);
                Grid.SetRow(dateStackPanel, 0);
                Grid.SetColumn(dateStackPanel, 2);
                break;

            case "PLAC":
                // Add an extra button for the additional place options.
                StackPanel placeStackPanel = new StackPanel();
                placeStackPanel.Orientation = Orientation.Horizontal;
                // Add a textbox for the tag value.
                TextBox textPlaceValue = new TextBox();
                textPlaceValue.Text = tag.value;
                textPlaceValue.Width = valueWidth - BUTTON_WIDTH;
                textPlaceValue.VerticalAlignment = VerticalAlignment.Center;
                textPlaceValue.LostFocus += textBoxValueLostFocus;
                placeStackPanel.Children.Add(textPlaceValue);
                // Add a button for additional date options.
                Button placeButton = new Button();
                placeButton.Content = "...";
                placeButton.Width = BUTTON_WIDTH;
                placeButton.VerticalAlignment = VerticalAlignment.Center;
                placeButton.Tag = textPlaceValue;
                placeButton.Click += placeButtonClick;
                placeStackPanel.Children.Add(placeButton);
                // Add stack panel to grid.
                _mainGrid.Children.Add(placeStackPanel);
                Grid.SetRow(placeStackPanel, 0);
                Grid.SetColumn(placeStackPanel, 2);
                break;

            default:
                // Add a textbox for the tag value.
                TextBox textBoxValue = new TextBox();
                textBoxValue.Text = tag.value;
                textBoxValue.Width = valueWidth;
                textBoxValue.VerticalAlignment = VerticalAlignment.Center;
                textBoxValue.LostFocus += textBoxValueLostFocus;
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
                    TagControl tagControl = new TagControl(_tag.children[i], _isExpand, setChildSizeControl);
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

        #endregion

        #region Properties

        #endregion


        /// <summary>Add a child control for the specified child tag to the control.</summary>
        /// <param name="childTag">Specifies the child tag to add to the control.</param>
        private void addChildControl(Tag childTag)
        {
            _imagePlusMinus.Visibility = Visibility.Visible;

            TagControl tagControl = new TagControl(childTag, _isExpand, setChildSizeControl);
            tagControl.VerticalAlignment = VerticalAlignment.Top;

            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(tagControl.Height);

            _childGrid.RowDefinitions.Add(rowDefinition);
            _childGrid.Children.Add(tagControl);
            Grid.SetRow(tagControl, _tag.children.Count());

            // Request the parent resize the space for this control.
            _setParentHeight?.Invoke();
        }



        /// <summary>Resize the space for each child row.</summary>
        /// <remarks>This is intended so that the children can inform this control when they change size.</remarks>
        private void setChildSizeControl()
        {
            foreach (RowDefinition rowDefinition in _childGrid.RowDefinitions)
            {
                rowDefinition.Height = new GridLength(0, GridUnitType.Auto);
            }
        }


        #region Signal Handlers

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



        /// <summary>Signal handler for the date helper button click.</summary>
        private void dateButtonClick(object sender, RoutedEventArgs e)
        {
            Button buttonDateHelper = (Button)sender;
            TextBox txtDate = (TextBox)buttonDateHelper.Tag;

            DateDialog dateDialog = new DateDialog();
            dateDialog.tagDate = txtDate.Text;
            if (dateDialog.ShowDialog() == true)
            {
                // Update the related control.
                txtDate.Text = dateDialog.tagDate;
                // Update the actual tag, (lost focus usually does this).
                _tag.value = txtDate.Text;
            }
        }



        /// <summary>Signal handler for the place helper button click.</summary>
        private void placeButtonClick(object sender, RoutedEventArgs e)
        {
            Button buttonPlaceHelper = (Button)sender;
            TextBox txtPlace = (TextBox)buttonPlaceHelper.Tag;

            PlaceDialog placeDialog = new PlaceDialog();
            placeDialog.tagPlace = txtPlace.Text;
            // We will need all the child tag values here.
            if (placeDialog.ShowDialog() == true)
            {
                // Update the related control.
                txtPlace.Text = placeDialog.tagPlace;                
                // Update the actual tag, (lost focus usually does this).
                _tag.value = txtPlace.Text;

                // We will need to update the child tags as well.
            }
        }



        /// <summary>Signal handler for the default text box losing the focus.</summary>
        /// <remarks>If the value has changed then update the tag.</remarks>
        private void textBoxValueLostFocus(object sender, RoutedEventArgs e)
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

        #endregion
    }
}
