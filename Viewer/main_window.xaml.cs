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
        /// <summary>The object to hold the page contents.</summary>
        private PageContent _pageContent;
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

        /// <summary>Returns true if the current document is saved or okay to delete.</summary>
        /// <returns>True if the current document is saved or okay to delete.</returns>
        private bool isDocumentSaved()
        {
            while (_gedcom.isDirty)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("The current gedcom has changed.\n\nDo you want to save the current changes?", "Gedcom Viewer", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    menuFileSaveClick(null, null);
                }
                else if (messageBoxResult == MessageBoxResult.No)
                {
                    // Don't save the file, but okay to continue.
                    return true;
                }
                else if (messageBoxResult == MessageBoxResult.Cancel)
                {
                    // Don't save the file but abort the current operation.
                    return false;
                }
            }

            // Document is not dirty, OK to continue.
            return true;
        }

        /// <summary>Populate the main window with the content from the specified host and query.</summary>
        /// <param name="host">Specifies the page host.</param>
        /// <param name="query">Specifies the page query.</param>
        /// <returns>True for success, false otherwise.</returns>
        private bool populateWindow(string host, string query)
        {
            //string html = _render.getContent(host, query);
            //_webBrowser.NavigateToString(html);
            _pageContent = _render.getContent(host, query);
            _webBrowser.NavigateToString(_userOptions.renderHtml(_pageContent.html.ToString()));

            if (_pageContent.editForm == "")
            {
                _menuEditEdit.IsEnabled = false;
                _toolbarEdit.IsEnabled = false;
            }
            else
            {
                _menuEditEdit.IsEnabled = true;
                _toolbarEdit.IsEnabled = true;
            }
            if (_pageContent.editGedomDirectly == "")
            {
                _menuEditEditGedcom.IsEnabled = false;
                _toolbarEditGedom.IsEnabled = false;
            }
            else
            {
                _menuEditEditGedcom.IsEnabled = true;
                _toolbarEditGedom.IsEnabled = true;
            }

            // Set window title.
            string fileName = System.IO.Path.GetFileName(_gedcom.fileName);
            if (fileName == "")
            {
                fileName = "New Document";
            }
            Title = fileName + (_gedcom.isDirty ? "*" : "") + " - Gedom Viewer WPF";

            // Return success.
            return true;
        }



        /// <summary>Launch the dialog specified in the host.</summary>
        /// <param name="host">Specifies a host which specifies an edit dialog.</param>
        /// <param name="query">Specifies the extra parameters for the edit dialog.</param>
        private void processDialogScheme(string host, string query)
        {
            switch (host)
            {
            case "individual":
                DialogIndividual dialogIndividual = new DialogIndividual(_gedcom, query);
                dialogIndividual.ShowDialog();
                break;
            }

            // Build the content within the application.
            populateWindow(host, query);
        }

        #region Signal Handlers

        /// <summary>Signal handler for the main window loaded signal.</summary>
        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            populateWindow("home", "");
        }



        /// <summary>Signal handler for the web browser control requesting a new page.</summary>
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
                populateWindow(e.Uri.Host, e.Uri.Query);

                // Close this navigation.
                return;
            }

            // An in-application dialog.
            if (e.Uri.Scheme == "dialog")
            {
                // A within app link, so build ourselves not web browser follow.
                e.Cancel = true;

                // Show the dialog and allow the user to edit it.
                processDialogScheme(e.Uri.Host, e.Uri.Query);

                // Close this navigation.
                return;
            }

            // Allow the web browser control to deal with the uri.
            return;
        }



        /// <summary>Signal handler for the 'File' -> 'Exit' menu point click.</summary>
        private void menuFileExitClick(object sender, RoutedEventArgs e)
        {
            if (isDocumentSaved())
            {
                // Close the main window and exit the program.
                Close();
            }
        }



        /// <summary>Signal handler for the 'File' -> 'Open' menu point click.</summary>
        private void menuFileOpenClick(object sender, RoutedEventArgs e)
        {
            // Check that the current gedcom does not need saving.
            if (isDocumentSaved())
            {
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
                    populateWindow("home", "");
                }
            }
        }



        /// <summary>Signal handler for the 'File' -> 'New' menu point click.</summary>        
        private void menuFileNewClick(object sender, RoutedEventArgs e)
        {
            // Check that the current gedcom does not need saving.
            if (isDocumentSaved())
            {
                // Creata a new empty document.
                _gedcom.clear();

                // Display the home page.
                populateWindow("home", "");
            }
        }



        /// <summary>Signal handler for the 'View' => 'Home' menu point click.</summary>
        private void menuViewHomeClick(object sender, RoutedEventArgs e)
        {
            // Show the home page.
            populateWindow("home", "");
        }



        /// <summary>Signal handler for the 'Edit' -> 'Edit' menu point click.</summary>
        private void menuEditEditClick(object sender, RoutedEventArgs e)
        {
            string[] splitUri = _pageContent.editForm.Split('?');
            string host = splitUri[0];
            string query = splitUri[1];

            // Show the dialog.
            processDialogScheme(host, query);
        }



        /// <summary>Signal handler for the 'Edit' -> 'Edit Gedcom' menu point click.</summary>
        private void menuEditEditGedcomClick(object sender, RoutedEventArgs e)
        {
            // Split the edit gedcom string into host and query.
            string[] splitUri = _pageContent.editGedomDirectly.Split('?');
            string host = splitUri[0];
            string query = splitUri[1];

            // Get the index from the query.
            System.Collections.Specialized.NameValueCollection queryParams = System.Web.HttpUtility.ParseQueryString(query);
            string idx = queryParams.Get("id");

            // Get the top level tag.
            TopLevel topLevel = null;
            switch (host)
            {
            case "individual":
                topLevel = _gedcom.individuals.find(idx);
                break;

            case "source":
                topLevel = _gedcom.sources.find(idx);
                break;
            }

            // Let the user edit the top level tag.
            if (topLevel != null)
            {
                DialogEditGedcom dialogEditGedcom = new DialogEditGedcom(topLevel.tag);
                if (dialogEditGedcom.ShowDialog() == true)
                {
                    // Mark the gedcom as dirty.
                    topLevel.gedcom.isDirty = true;

                    // Show the actual page.
                    populateWindow(host, query);
                }
            }
        }


        private void menuFileSaveClick(object sender, RoutedEventArgs e)
        {

        }

        private void menuFileSaveAsClick(object sender, RoutedEventArgs e)
        {

        }

        #endregion

    }
}
