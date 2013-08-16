using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GeoCoordinateWatcherDemo.Resources;

namespace GeoCoordinateWatcherDemo {
  public partial class MainPage : PhoneApplicationPage {
    // Constructor
    public MainPage() {
      InitializeComponent();

      InitLocationService();
    }
   
    GeoCoordinateWatcher _watcher;

    public void InitLocationService() {
      try {
        _watcher = new GeoCoordinateWatcher (GeoPositionAccuracy.Default) {
          MovementThreshold = 50,
        };
        _watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
        bool started = this._watcher.TryStart(false, TimeSpan.FromMilliseconds(2000));
        if (!started) {
          MessageBox.Show("GeoCoordinateWatcher konnte nicht gestartet werden.");
        }
      } catch (UnauthorizedAccessException  ex) {
        MessageBox.Show("Bitte erlauben Sie den Zugriff auf die Location API für diese App.");
      }
    }

    void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e) {
      PrintPosition(e.Position.Location.Latitude, e.Position.Location.Longitude);
    }

    void PrintPosition(double latitude, double longitude) {
      tbPos.Text = string.Format("\r\n\tLat: {0}, \r\n\tLon: {1}", latitude, longitude);
    }
  }
}