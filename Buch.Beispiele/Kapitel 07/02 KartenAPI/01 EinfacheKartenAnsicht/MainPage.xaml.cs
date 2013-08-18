using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Windows.Devices.Geolocation;
using System.Device.Location;
using Microsoft.Phone.Maps.Controls;

namespace BasicLocationandMap
{
  public partial class MainPage : PhoneApplicationPage {
    // Constructor
    public MainPage() {
      InitializeComponent();

    }

    private async void LocateMeButton_Click_1(object sender, EventArgs e) {
      //Aktuelle Position des Telefons bestimmen
      var myGeolocator = new Geolocator { DesiredAccuracyInMeters = 5 };
      Geoposition myGeoPosition = null;
      try {
        myGeoPosition = await myGeolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));
      } catch (UnauthorizedAccessException) {
        MessageBox.Show("Bitte erlauben Sie den Zugriff auf die Positionsdaten");
      }

      // Kartenansicht auf die aktuelle Position zentrieren
      if (myGeoPosition != null) this.MyMap.Center = new GeoCoordinate(myGeoPosition.Coordinate.Latitude, myGeoPosition.Coordinate.Longitude);
      this.MyMap.ZoomLevel = 15;
    }

    private void mnuRoad_Click(object sender, EventArgs e) {
      MyMap.CartographicMode = MapCartographicMode.Road;
    }

    private void mnuAerial_Click(object sender, EventArgs e) {
      MyMap.CartographicMode = MapCartographicMode.Aerial;
    }

    private void mnuHybrid_Click(object sender, EventArgs e) {
      MyMap.CartographicMode = MapCartographicMode.Hybrid;
    }

    private void mnuTerrain_Click(object sender, EventArgs e) {
      MyMap.CartographicMode = MapCartographicMode.Terrain;
    }

    private void mnuLight_Click(object sender, EventArgs e) {
      MyMap.ColorMode = MapColorMode.Light;
    }

    private void mnuDark_Click(object sender, EventArgs e) {
      MyMap.ColorMode = MapColorMode.Dark;
    }
  }
}