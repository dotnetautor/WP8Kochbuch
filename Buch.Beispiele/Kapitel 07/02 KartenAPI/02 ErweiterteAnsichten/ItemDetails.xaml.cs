using System;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using Microsoft.Phone.Controls;
using MapViews.Model;
using Microsoft.Phone.Tasks;
using Windows.System;

namespace MapViews {
  public partial class ItemDetails : PhoneApplicationPage {
    private Item _currentItem;

    public ItemDetails() {
      InitializeComponent();
    }

    protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e) {

      string id;
      if (NavigationContext.QueryString.TryGetValue("id", out id)) {
        _currentItem = DataManager.SampleItems.FirstOrDefault(i => i.Id.ToString() == id);
        DataContext = _currentItem ;
      }

    }

    private async void Route_Click(object sender, System.EventArgs e) {

      var uri = new Uri("ms-drive-to:?destination.latitude=" + _currentItem.Coordinate.Latitude.ToString(CultureInfo.InvariantCulture) + "&destination.longitude=" + _currentItem.Coordinate.Longitude.ToString(CultureInfo.InvariantCulture) + "&destination.name=" + _currentItem.Name);
      // Launch the Uri.
      var success = await Launcher.LaunchUriAsync(uri);

    }

    private void Navigation_Click(object sender, System.EventArgs e) {

      // Get Directions
      var mapsDirectionsTask = new MapsDirectionsTask();
      // You can specify a label and a geocoordinate for the end point.
      var loc = new GeoCoordinate(_currentItem.Coordinate.Latitude, _currentItem.Coordinate.Longitude);
      var itemLoc = new LabeledMapLocation(_currentItem.Name, loc);
      // If mapsDirectionsTask.Start is not set, the user's current location
      mapsDirectionsTask.End = itemLoc;
      mapsDirectionsTask.Show();


    }

    private void Download_Click(object sender, System.EventArgs e) {
      var mapDownloaderTask = new MapDownloaderTask(); 
      mapDownloaderTask.Show();

    }

    private void Setup_Click(object sender, System.EventArgs e) {
      MapUpdaterTask mapUpdaterTask = new MapUpdaterTask(); 
      mapUpdaterTask.Show();
    }
  }
}