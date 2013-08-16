using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GeolocatorDemo.Resources;
using Windows.Devices.Geolocation;

namespace GeolocatorDemo {
  public partial class MainPage : PhoneApplicationPage {

    private Geolocator geolocator;
    
    // Constructor
    public MainPage() {
      InitializeComponent();
      tbStatus.Text = "Stopped";
      tbPos.Text = string.Format("\r\n\t{0:#0.0000}\r\n\t{1:#0.0000}", 0, 0);

    }

    private async void btnOneShot_Click(object sender, RoutedEventArgs e) {
      var geolocator = new Geolocator { DesiredAccuracyInMeters = 50 };
      try {
        Geoposition geoposition = await geolocator.GetGeopositionAsync(
          maximumAge: TimeSpan.FromMinutes(5),
          timeout: TimeSpan.FromSeconds(10));

        tbPos.Text = string.Format("\r\n\t{0:#0.0000}\r\n\t{1:#0.0000}",
          geoposition.Coordinate.Latitude,
          geoposition.Coordinate.Longitude);

       
      } catch (UnauthorizedAccessException) {
        MessageBox.Show("Bitte erlauben Sie den Zugriff auf die Location API für diese App.");
      }
    }

    private void btnStart_Click(object sender, RoutedEventArgs e) {

      try {
      geolocator = new Geolocator {
        DesiredAccuracy = PositionAccuracy.High, 
        MovementThreshold = 50
      };

        geolocator.StatusChanged += (sender1, args) =>

                                    Dispatcher.BeginInvoke(() =>
                                      { tbStatus.Text = args.Status.ToString(); });


        geolocator.PositionChanged += (sender1, args) => Dispatcher.BeginInvoke(() =>
          {
            tbPos.Text = string.Format("\r\n\t{0:#0.0000}\r\n\t{1:#0.0000}",
                                       args.Position.Coordinate.Latitude,
                                       args.Position.Coordinate.Longitude);
          });


        btnOneShot.IsEnabled = false;
        btnStart.IsEnabled = false;
        btnStop.IsEnabled = true;
      } catch (UnauthorizedAccessException) {
        MessageBox.Show("Bitte erlauben Sie den Zugriff auf die Location API für diese App.");
      }

     

    }

    private void btnStop_Click(object sender, RoutedEventArgs e) {
    
      geolocator = null;

      tbStatus.Text = "Stopped";
      tbPos.Text = string.Format("\r\n\t{0:#0.0000}\r\n\t{1:#0.0000}", 0, 0);

      btnOneShot.IsEnabled = true;
      btnStart.IsEnabled = true;
      btnStop.IsEnabled = false;
      
    }

  }
}