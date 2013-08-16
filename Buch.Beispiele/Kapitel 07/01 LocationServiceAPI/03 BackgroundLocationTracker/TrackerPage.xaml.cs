using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Devices.Geolocation;

namespace LocationTracker {
  public partial class TrackerPage : PhoneApplicationPage {
    public TrackerPage() {
      InitializeComponent();
    }

    protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e) {
      if (App.Geolocator == null) {
        App.Geolocator = new Geolocator {DesiredAccuracy = PositionAccuracy.High, MovementThreshold = 100};
        App.Geolocator.PositionChanged += geolocator_PositionChanged;
      }
    }

    // Ensure that if the page is removed from the app's journal, references to the Geolocator are destroyed
    // If the app ever shows this page again, a new instance of Geolocator will be craeted in OnNavigatedTo
    protected override void OnRemovedFromJournal(System.Windows.Navigation.JournalEntryRemovedEventArgs e) {
      App.Geolocator.PositionChanged -= geolocator_PositionChanged;
      App.Geolocator = null;
    }

    private void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args) {
      if (!App.RunningInBackground) {
        Dispatcher.BeginInvoke(() =>
          {
            LatitudeTextBlock.Text = args.Position.Coordinate.Latitude.ToString("0.00");
            LongitudeTextBlock.Text = args.Position.Coordinate.Longitude.ToString("0.00");
          });
      } else {
        // Show toast if running in background
        var toast = new ShellToast {
          Content = string.Format("{0} - {1}",args.Position.Coordinate.Latitude.ToString("0.00"),args.Position.Coordinate.Longitude.ToString("0.00")),
          Title = "Location: ",
          NavigationUri = new Uri("/TrackerPage.xaml", UriKind.Relative)
        };
        toast.Show();

      }
    }
  }
}