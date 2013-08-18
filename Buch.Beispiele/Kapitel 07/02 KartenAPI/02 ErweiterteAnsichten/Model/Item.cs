using System.Collections.Generic;
using System.Globalization;

namespace MapViews.Model {
  public class Position {

    public Position() {
      Longitude = 0;
      Latitude = 0;
      Address = "";
    }

    public Position(string loc) {
      var tmp = loc.Split(new[] { ',' });
      Longitude = double.Parse(tmp[0], CultureInfo.InvariantCulture);
      Latitude = double.Parse(tmp[1], CultureInfo.InvariantCulture);
    }

    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public string Address { get; set; }
  }

  public class Item {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Position Coordinate { get; set; }
    public Dictionary<string, string> ExtendedData { get; set; }
  }
}
