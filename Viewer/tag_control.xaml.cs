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
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class TagControl : UserControl
    {
        /// <summary>The tag to display.</summary>
        private Tag _tag;

        public TagControl(Tag tag)
        {
            InitializeComponent();

            // Add a master row for the actual tag.
            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(30);
            _mainGrid.RowDefinitions.Add(rowDefinition);

            TextBlock textBox = new TextBlock();
            textBox.Text = "1";
            _mainGrid.Children.Add(textBox);

            TextBlock textBoxTag = new TextBlock();
            textBoxTag.Text = tag.key;
            _mainGrid.Children.Add(textBoxTag);

            TextBlock textBoxValue = new TextBlock();
            textBoxValue.Text = tag.value;
            _mainGrid.Children.Add(textBoxValue);





        }
    }
}
