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
// DispatcherTimer.
using System.Windows.Threading;

namespace gedcom.viewer
{
    /// <summary>Class to represent the main window for the gedcom-wpf application.</summary>
    public partial class MainWindow : Window
    {
        #region Member Variables

        /// <summary>The gedom to display.</summary>
        private readonly gedcom.Gedcom _gedcom;
        /// <summary>The class to render the gedom to html.</summary>
        private readonly Render _render;
        //private DispatcherTimer _dispatcherTimer;
        //private string _newUrl;
        /// <summary>The user preferences.</summary>
        private readonly UserOptions _userOptions;
        #endregion

        #region Class Constructors

        /// <summary>Class constructor for the main window.</summary>
        public MainWindow()
        {
            _gedcom = new gedcom.Gedcom();
            _userOptions = new UserOptions();
            _render = new Render(_gedcom, _userOptions);
            //_newUrl = "";
            //_dispatcherTimer = new DispatcherTimer();
            //_dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            //_dispatcherTimer.Tick += dispatcherTimerTick;
            
            InitializeComponent();
        }

        #endregion

        /*
        private void dispatcherTimerTick(object sender, EventArgs e)
        {
            if (_newUrl != "")
            {
                _webBrowser.NavigateToString(_render.getContent(_newUrl));
                _newUrl = "";
            }
        }
        */

        #region Signal Handlers

        private void appExitClick(object sender, RoutedEventArgs e)
        {
            // Close the main window and exit the program.
            Close();
        }



        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            // _gedcom.open("walton.ged");

            _webBrowser.NavigateToString(_render.getContent("home", ""));
        }



        private void webBrowserNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.Uri == null)
            {
                // Probably navigating to a string, so okay.
                return;
            }

            // Console.WriteLine("uri.Query = " + e.Uri.Query);
            // Console.WriteLine("uri.LocalPath = " + e.Uri.LocalPath);
            // Console.WriteLine("uri.Host = " + e.Uri.Host);
            // Console.WriteLine("uri.Scheme = " + e.Uri.Scheme);

            // An in application link.
            if (e.Uri.Scheme == "app")
            {
                // A within app link, so build ourselves not web browser follow.
                e.Cancel = true;

                // Build the content within the application.
                _webBrowser.NavigateToString(_render.getContent(e.Uri.Host, e.Uri.Query));

                // Close this navigation.
                return;
            }

            // An in-application dialog.
            if (e.Uri.Scheme == "dialog")
            {
                // A within app link, so build ourselves not web browser follow.
                e.Cancel = true;

                // Show the dialog and allow the user to edit it.
                switch (e.Uri.Host)
                {
                case "individual":
                    DialogIndividual dialogIndividual = new DialogIndividual(_gedcom, e.Uri.Query);
                    dialogIndividual.ShowDialog();
                    break;
                }
                
                // Build the content within the application.
                _webBrowser.NavigateToString(_render.getContent(e.Uri.Host, e.Uri.Query));

                // Close this navigation.
                return;
            }


            // Allow the web browser control to deal with the uri.
            return;
        }



        /// <summary>Signal handler for the File -> Open menu point click.</summary>
        private void menuFileOpenClick(object sender, RoutedEventArgs e)
        {
            // Check that the current gedcom does not need saving.


            // Get a gedcom file from the user.
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Title = "Select Gedcom File",
                Filter = "Gedcom Files (*.ged)|*.ged|All Files (*.*)|*.*"
            };
            bool? result = openFileDialog.ShowDialog();

            // Open the selected gedcom file.
            if (result == true)
            {
                // Open the specified file.
                _gedcom.open(openFileDialog.FileName);

                // Display the home page.
                _webBrowser.NavigateToString(_render.getContent("home", ""));
            }
        }



        /// <summary>Signal handler for the File -> New menu point click.</summary>        
        private void menuFileNewClick(object sender, RoutedEventArgs e)
        {
            // Check that the current gedcom does not need saving.

            // Creata a new empty document.
            _gedcom.clear();

            // Display the home page.
            _webBrowser.NavigateToString(_render.getContent("home", ""));
        }



        /// <summary>Signal handler for the 'View' => 'Home' menu point click.</summary>
        private void menuViewHomeClick(object sender, RoutedEventArgs e)
        {
            // Show the home page.
            _webBrowser.NavigateToString(_render.getContent("home", ""));
        }

        #endregion

    }
}
