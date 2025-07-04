using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
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
    /// <summary>
    /// Dialog to allow the user to enter / edit a gedcom place.
    /// </summary>
    public partial class PlaceDialog : Window
    {
        #region Member Variables

        /// <summary>The gedcom that this place is in.</summary>
        private Gedcom _gedcom;

        /// <summary>The place string to use in the gedcom file.</summary>
        private string _tagPlace;

        /// <summary>True when the controls should not update the object or each other.</summary>
        private bool _isNoUpdate;
       
        #endregion

        #region Constructors

        public PlaceDialog(Gedcom gedcom)
        {
            _gedcom = gedcom;
            _isNoUpdate = true;
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>The place string to use in the gedcom file.</summary>
        /// <remarks>This is not thread safe.</remarks>
        public string tagPlace
        {
            get => _tagPlace;
            set 
            {
                _tagPlace = value;

                // Decode the date value onto the dialog.
                if (!_isNoUpdate)
                {
                    toDialog();
                }
            }
        }


        #endregion

        #region Encode Decode

        /// <summary>Transfer the values from the member variables to the dialog.</summary>
        /// <returns>True for success, false otherwise.</returns>
        private bool toDialog()
        {
            // No update from the dialog controls.
            _isNoUpdate = true;

            // Select the existing place (might not be available).
            _cboExistingPlaces.SelectedItem = _tagPlace;

            // Allow updates from the dialog controls.
            _isNoUpdate = false;

            // Return success.
            return true;
        }



        /// <summary>Transfer the values from the member variables to the dialog text box.</summary>
        /// <returns>True for success, false otherwise.</returns>
        private bool fromDialog()
        {
            // Build a new date string.
            StringBuilder newDate = new StringBuilder();

            // Return success.
            return true;
        }

        #endregion

        /// <summary>
        /// Add the specified places to the existing places combo box.
        /// </summary>
        /// <param name="places">Specifies the places to add to the combobox.</param>
        private void addExistingPlaces(Places places)
        {
            // Add the specified places to the combo box.
            foreach(Place place in places)
            {
                // Add the place to the combobox.
                _cboExistingPlaces.Items.Add(place.fullName);

                // Add the children of the place.
                addExistingPlaces(place.children);
            }
        }        
        
        #region Signal Handlers

        /// <summary>Signal handler for the window loaded event.</summary>
        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            // Update the date text box.
            _txtTagPlace.Text = _tagPlace;

            // Populate the existing places comobobox.            
            addExistingPlaces(_gedcom.places);

            // Update the dialog.
            toDialog();
        }



        /// <summary>Signal handler for the OK button click. </summary>
        private void buttonOkClick(object sender, RoutedEventArgs e)
        {
            // Update the tagDate property.
            _tagPlace = _txtTagPlace.Text;
            // Close the dialog with success.
            this.DialogResult = true;
        }



        /// <summary>Signal handler for the cancel button click.</summary>
        private void buttonCancelClick(object sender, RoutedEventArgs e)
        {

        }



        /// <summary>Signal handler for the date textbox losing the focus.</summary>
        private void txtTagDateLostFocus(object sender, RoutedEventArgs e)
        {
            // Decode the new string value.
            _tagPlace = _txtTagPlace.Text;

            // Update the other controls on the dialog.
            toDialog();
        }



        #endregion

        /// <summary>Signal handler for the google maps button click.</summary>
        private void buttonGoogleMapsClick(object sender, RoutedEventArgs e)
        {
            string url = "https://www.google.com/maps/search/?api=1&query="+ _txtLatitude.Text + ","+ _txtLongitude.Text;

            // Launch the url in the default browser.
            System.Diagnostics.Process.Start(url);
        }



        /// <summary>Signal handler for the existing place combo selection changing.</summary>
        private void cboExistingPlacesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check that updates are allowed.
            if(_isNoUpdate)
            {
                return;
            }

            // Check for a valid selection.
            if (_cboExistingPlaces.SelectedIndex < 0)
            {
                // No selection abort.
                return;
            }

            string placeString = (string)_cboExistingPlaces.SelectedItem;
            Place selectedPlace = _gedcom.places.getPlace(placeString);
            if (selectedPlace != null)
            {
                _txtTagPlace.Text = selectedPlace.fullName;

                if (selectedPlace.latitude == 0 && selectedPlace.longitude == 0)
                {
                    // Map information is not available.
                    _txtLatitude.Text = "";
                    _txtLongitude.Text = "";
                }
                else
                {
                    // Map information is available.
                    _txtLatitude.Text = selectedPlace.latitude.ToString();
                    _txtLongitude.Text = selectedPlace.longitude.ToString();
                }
            }
        }
    }
}
