using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MapViews.Model;
using Microsoft.Phone.Controls;
using System.Device.Location;
using Microsoft.Phone.Maps.Controls;
using Windows.Devices.Geolocation;

namespace MapViews {
  public partial class MainPage : PhoneApplicationPage {
    private static readonly CultureInfo ci = new CultureInfo("en-US");

    // Constructor
    public MainPage() {
      InitializeComponent();
    }

    private void OnCenterZoom_Click(object sender, EventArgs args) {
      MyMap.Center = new GeoCoordinate(47.6097, -122.3331);
      MyMap.ZoomLevel = 15;
    }

    private void OnAnimate_Click(object sender, EventArgs args) {
      // Variante 1
      //MyMap.SetView(new GeoCoordinate(52.53083, 13.38457), 15, MapAnimationKind.Parabolic);
      
      // Variante 2
      //MyMap.SetView(
      //  new LocationRectangle(
      //    new GeoCoordinate(52.546853, 13.190607),
      //    new GeoCoordinate(52.538475, 13.206914)), 
      //    new Thickness(40),
      //    MapAnimationKind.Parabolic);


      // Variante 3
      var rect = LocationRectangle.CreateBoundingRectangle(
        new GeoCoordinate[] {
          new GeoCoordinate(52.546853, 13.190607),
          new GeoCoordinate(52.538475, 13.206914), 
          new GeoCoordinate(52.543278, 13.234524),
          new GeoCoordinate(52.539843, 13.198724), 
        }
        
        );

      MyMap.SetView(
          rect,
          new Thickness(40),
          MapAnimationKind.Parabolic);
    }

    private async void BtnReset_Click(object sender, EventArgs e) {
      var geoPos = await GetCurrentPosiotion();
      MyMap.Center = (geoPos != null) ? geoPos.Coordinate.ToGeoCoordinate() : new GeoCoordinate(0, 0);
      MyMap.ZoomLevel = 1;
    }

    private void ShowSampleItems_Click(object sender, EventArgs e) {
      // create some sample items;

      MyMap.MapElements.Clear();
      MyMap.Layers.Clear();

      var bounds = new List<LocationRectangle>();
      foreach (var area in DataManager.SampleAreas) {
        var shape = new MapPolygon();
        foreach (var parts in area.Split(' ').Select(cord => cord.Split(','))) {
          shape.Path.Add(new GeoCoordinate(double.Parse(parts[1], ci), double.Parse(parts[0], ci)));
        }

        shape.StrokeThickness = 3;
        shape.StrokeColor = Colors.Blue;
        shape.FillColor = Color.FromArgb((byte)0x20, shape.StrokeColor.R, shape.StrokeColor.G, shape.StrokeColor.B);
        bounds.Add(LocationRectangle.CreateBoundingRectangle(shape.Path));
        MyMap.MapElements.Add(shape);
      }

      var rect = new LocationRectangle(bounds.Max(b => b.North), bounds.Min(b => b.West), bounds.Min(b => b.South), bounds.Max(b => b.East));
      MyMap.SetView(rect);

      // show all Items
      var itemsLayer = new MapLayer();
      foreach (var item in DataManager.SampleItems) {
        DrawItem(item, "Ulm", itemsLayer);
      }

      MyMap.Layers.Add(itemsLayer);
    }

    private async Task<Geoposition> GetCurrentPosiotion() {
       Geoposition geoposition;
       var geolocator = new Geolocator {DesiredAccuracyInMeters = 50};
       try{
         geoposition = await geolocator.GetGeopositionAsync( maximumAge: TimeSpan.FromMinutes(5),timeout: TimeSpan.FromSeconds(10) );   
       }     catch (UnauthorizedAccessException)     {
         // the app does not have the right capability or the location master switch is off         
         geoposition = null;
       }
      return geoposition;
    }

    private void DrawItem(Item item, string loc, MapLayer mapLayer) {
      var contentPres = new ContentPresenter { Content = item.Name, FlowDirection = FlowDirection.LeftToRight, Margin = new Thickness(4) };
      var innerGrid = new Grid { HorizontalAlignment = HorizontalAlignment.Left, MinHeight = 30, MinWidth = 30, Background = new SolidColorBrush(Colors.Blue) };
      innerGrid.Children.Add(contentPres);
      var panel = new StackPanel { Orientation = System.Windows.Controls.Orientation.Vertical };
      panel.Children.Add(innerGrid);
      panel.Children.Add(new Polygon { Points = new PointCollection { new Point(0, 0), new Point(15, 0), new Point(0, 30) }, Width = 15, Height = 30, Fill = new SolidColorBrush(Colors.Blue), HorizontalAlignment = HorizontalAlignment.Left });
      panel.Tag = item;
      panel.MouseLeftButtonUp += (sender, args) => {
        var p = sender as StackPanel;
        if (p == null) return;
        var c = p.Tag as Item;
        if (c == null) return;
        NavigationService.Navigate(new Uri("/ItemDetails.xaml?id=" + item.Id, UriKind.Relative));
      };

      // Create a MapOverlay and add marker
      var overlay = new MapOverlay {
        Content = panel,
        GeoCoordinate = new GeoCoordinate(item.Coordinate.Latitude, item.Coordinate.Longitude),
        PositionOrigin = new Point(0.0, 1.0)
      };

      mapLayer.Add(overlay);

    }

    private void MyMap_OnLoaded(object sender, RoutedEventArgs e) {
#if !DEBUG
       Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "YourApplicationID";
       Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "YourAuthenticationToken";
#endif
    }
  }
}