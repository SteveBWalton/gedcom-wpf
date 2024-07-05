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

namespace gedcom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AppExit_Click(object sender, RoutedEventArgs e)
        {
            // Close the main window and exit the program.
            Close();
        }

        private void AppTest_Click(object sender, RoutedEventArgs e)
        {
            webBrowser_.NavigateToString("<h1>Hello World</h1>");
        }
    }
}
