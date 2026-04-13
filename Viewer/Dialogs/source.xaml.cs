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
    /// <summary>Class to represent the edit source dialog.</summary>
    public partial class DialogSource : Window
    {
        #region Member Variables

        /// <summary>The gedcom that contains the source to edit.</summary>
        private readonly Gedcom _gedcom;

        /// <summary>The source that the dialog is editting.</summary>
        private Source _source;

        /// <summary>The collection of tag controls that make this source.</summary>
        private List<TagControl> _tagControls;

        #endregion

        #region Constructors

        /// <summary>Constructor for the edit source dialog to create a new source.</summary>
        /// <param name="gedcom">Specifies the gedcom that contains the individual to edit.</param>
        public DialogSource(Gedcom gedcom)
        {
            InitializeComponent();

            // Save the parameters.
            _gedcom = gedcom;
            _source = new Source(gedcom);
            _tagControls = new List<TagControl>();
        }



        /// <summary>Constrcutor for the edit individual dialog to edit an existing source.</summary>
        /// <param name="gedcom">Specifies the gedcom that contains the individual to edit.</param>
        /// <param name="query">Specifies the query string which contains an ID key to identifiy the individual.</param>
        public DialogSource(Gedcom gedcom, string query):this(gedcom)
        {
            // Get the idx of the source.
            NameValueCollection queryParams = HttpUtility.ParseQueryString(query);
            string idx = queryParams.Get("id");

            // Find the specified individual.
            _source = _gedcom.sources.find(idx);
            if (_source == null)
            {
                throw (new Exception("No source found for '" + idx + "'"));
            }
        }

        #endregion

        #region Properties

        /// <summary>The source that the dialog is editting.</summary>
        public Source source
        {
            get => _source;
        }

        #endregion

        /// <summary>Resize the space for each child row.</summary>
        /// <remarks>This is intended so that the children can inform this control when they change size.</remarks>
        private void setChildSizeIndividual()
        {
            foreach (RowDefinition rowDefinition in _gridTags.RowDefinitions)
            {
                rowDefinition.Height = new GridLength(0, GridUnitType.Auto);
            }
        }



        /// <summary>The child tag control has requested to be deleted.</summary>
        /// <param name="tagControl">Specifies the child tag control that has requested to be deleted.</param>
        private void askParentDelete(TagControl tagControl)
        {
            int i = 0;
            while (i < _tagControls.Count)
            {
                if (_tagControls[i] == tagControl)
                {
                    // Delete the row.
                    // This doesn't change the row index of the other tag controls.
                    _gridTags.RowDefinitions.RemoveAt(i);

                    // Delete control.
                    tagControl.delete();

                    // Remove from collection.
                    _tagControls.RemoveAt(i);
                }
                else
                {
                    Grid.SetRow(_tagControls[i], i);
                    i++;
                }
            }
        }

        #region Signal Handlers

        /// <summary>Signal handler for the window loaded event.</summary>
        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            // Find the repository for the source.
            Tag tag = _source.tag.children.findOne("REPO");
            string tagIdx = "";
            if (tag != null)
            {
                tagIdx = gedcom.Tag.toIdx(tag.value);
            }

            // Populate the repository combobox.
            _cboRepository.Items.Add("None");
            _cboRepository.SelectedIndex = 0;
            foreach (gedcom.Repository repository in _gedcom.repositories)
            {
                _cboRepository.Items.Add(repository);
                if (repository.idx == tagIdx)
                {
                    _cboRepository.SelectedIndex = _cboRepository.Items.Count - 1;
                }
            }

            // Populate the dialog with the source.
            Tag tagDate = _source.tag.children.findOne("DATE");
            if (tagDate != null)
            {
                _txtDate.Text = tagDate.value;
            }
            Tag tagTitle = _source.tag.children.findOne("TITL");
            if (tagTitle != null)
            {
                string sourceType = "";
                string title = tagTitle.value;
                if (title.IndexOf(":") != -1)
                {
                    sourceType = title.Substring(0, title.IndexOf(":"));
                    title = title.Substring(title.IndexOf(":") + 1).Trim();
                }
                _txtTitle.Text = title;

                // Try to match the source type.
                bool isMatchFound = false;
                for (int i = 0; i < _cboSourceType.Items.Count; i++)
                {
                    ComboBoxItem comboBoxItem = _cboSourceType.Items[i] as ComboBoxItem;
                    string itemValue = (string)comboBoxItem.Content.ToString();
                    Console.WriteLine(itemValue);
                    if (sourceType == itemValue)
                    {
                        _cboSourceType.SelectedIndex = i;
                        isMatchFound = true;
                    }
                }
                if (!isMatchFound)
                {
                    _cboSourceType.SelectedIndex = 0;
                }
            }

            // Create rows for the tags.
            int row = 0;
            for (int i = 0; i < _source.tag.children.count; i++)
            {
                switch (_source.tag.children[i].key)
                {
                case "TITL":
                case "DATE":
                case "REPO":
                case "CHAN":
                    // These tags are dealt with on the main dialog.
                    // Do nothing here.
                    break;

                default:
                    if (_source.tag.children[i].key == "NOTE")
                    {
                        if (_source.tag.children[i].value.StartsWith("GRID:"))
                        {
                            TagNoteGrid tagNoteGrid = new TagNoteGrid(_source.tag.children[i]);
                            _txtSpecial.Text = tagNoteGrid.ToString();

                            //_txtSpecial.Text = _source.tag.children[i].value.Substring(6);
                            //foreach (Tag childTag in _source.tag.children[i].children)
                            //{
                            //    _txtSpecial.Text = _txtSpecial.Text + "\n" + childTag.value;
                            //}
                            break;
                        }
                    }

                    // Add this tag to the control.
                    TagControl tagControl = new TagControl(_source.tag.children[i], false, setChildSizeIndividual, askParentDelete, null);
                    tagControl.VerticalAlignment = VerticalAlignment.Top;
                    _tagControls.Add(tagControl);

                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = new GridLength(tagControl.Height);

                    _gridTags.RowDefinitions.Add(rowDefinition);
                    _gridTags.Children.Add(tagControl);
                    Grid.SetRow(tagControl, row);
                    row++;
                    break;
                }
            }

        }



        /// <summary>Signal handler for the OK button click event.</summary>
        private void buttonOkClick(object sender, RoutedEventArgs e)
        {
            // Get the tags from the dialog as a string.
            StringBuilder tagsAsText = new StringBuilder();
            // Add a title.
            if (_cboSourceType.SelectedIndex > 0)
            {
                // Add the type and the title.
                ComboBoxItem comboBoxItem = _cboSourceType.SelectedItem as ComboBoxItem;
                tagsAsText.AppendLine("1 TITL " + (string)comboBoxItem.Content.ToString() + ": " + _txtTitle.Text);
            }
            else
            {
                // No type information just the title.
                tagsAsText.AppendLine("1 TITL " + _txtTitle.Text);
            }
            // Add a date.
            tagsAsText.AppendLine("1 DATE " + _txtDate.Text);

            // Add the special note.
            if (_txtSpecial.Text != "")
            {
                string[] lines = _txtSpecial.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length > 1)
                {
                    tagsAsText.AppendLine($"1 NOTE GRID: {lines[0]}");
                }
                for (int i = 1; i < lines.Length; i++)
                {
                    tagsAsText.AppendLine($"2 CONT {lines[i]}");
                }
            }

            // Might sort these into a better order.
            // Get the value of each tag control.
            foreach (TagControl tagControl in _tagControls)
            {
                tagsAsText.Append(tagControl.ToString());
            }

            // Add a repository
            if (_cboRepository.SelectedIndex > 0)
            {
                Repository repository = (Repository)_cboRepository.SelectedItem;
                tagsAsText.AppendLine("1 REPO " + gedcom.Tag.toKey(repository.idx));
            }
            //else
            //{
            //    // Repository None.
            //}

            // Clear the existing tags.
            _source.tag.children.clear();

            // Add the tags from the tag controls text.
            using (System.IO.StringReader stringReader = new System.IO.StringReader(tagsAsText.ToString()))
            {
                string line;
                while ((line = stringReader.ReadLine()) != null)
                {
                    if (line != "")
                    {
                        _source.tag.add(line);
                    }
                }
            }

            // Update the last changed tag.
            _source.setLastChanged();

            // Sort the tags into a better order.
            _source.tag.children.sort();

            // Close the dialog with okay.
            this.DialogResult = true;
        }



        /// <summary>Signal handler for the cancel button click.</summary>
        private void buttonCancelClick(object sender, RoutedEventArgs e)
        {

        }



        /// <summary>Signal handler for the add tag button click.</summary>
        private void addTagButtonClick(object sender, RoutedEventArgs e)
        {

        }



        /// <summary>Signal handler for the source type combobox changing value.</summary>
        private void cboSourceTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_cboSourceType.SelectedIndex>0)
            {
                // Show the template button.
                _buttonTemplate.Visibility = Visibility.Visible;
            }
            else
            {
                // Hide the template button.
                _buttonTemplate.Visibility = Visibility.Hidden;
            }
        }



        /// <summary>Signal handler for the apply template button click.</summary>
        private void buttonTemplateClick(object sender, RoutedEventArgs e)
        {
            string[] template = null;
            switch(_cboSourceType.SelectedIndex)
            {
            case 1: // Birth Certifiicate.
                template = new string[] { "GRO Reference: ", "Registration District: ", "When and Where:: ", "Name:: ", "Mother:: ", "Father:: ", "Informant:: " };
                break;

            case 2: // Marriage Certificate.
                template = new string[] { "GRO Reference: ", "Groom:::: ", "Bride::::: ", "Groom's Father::: ", "Bride's Father::: ", "Witness: " };
                break;

            case 3: // Death Certificate.
                template = new string[] { "GRO Reference:1", "Registration District:2", "When:3", "Where:4", "Name:5:6:", "Date & Place of Birth:8:9", "Occupation:10", "Usual Address:11", "Cause of Death:12|12", "Informant:13:14", "Informant Address:15" };
                break;

            case 4: // Census.
            case 5: // 1939 Register.
                template = new string[] { "Reference: Series: : Piece: : Folio: : Page: ", "Name:PersonIdx:Age:Household:Occupation:BornLocation " };
                break;

            }
            // Check if template is defined.
            if (template == null)
            {
                return;
            }

            // This is not exactly what a tag note grid is but it should work well.
            TagNoteGrid tagNoteGrid = new TagNoteGrid(_txtSpecial.Text);
            for (int i = 0; i < template.Length; i++)
            {
                string[] cells = template[i].Split(':');
                for (int j = 0; j < cells.Length; j++)
                {
                    if (cells[j].Trim() != "")
                    {
                        tagNoteGrid.setCell(i, j, cells[j]);
                    }
                }
            }

            // Set the special text.
            _txtSpecial.Text = tagNoteGrid.ToString();

        }

        #endregion
    }
}
