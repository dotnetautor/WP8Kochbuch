using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using MapViews.Model;

namespace MapViews.Utilities {
  public class PositionConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (value is Position && targetType == typeof(GeoCoordinate)) {
        var temp = value as Position;
        return new GeoCoordinate(temp.Latitude, temp.Longitude);
      }
      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }
  }
}
