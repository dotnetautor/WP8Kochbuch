using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MiniMVVM.Code {
  public class DoubleConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (targetType == typeof (string) && value is double) {
        return value.ToString();
      } else {
        return value;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      if (targetType == typeof (double) && value is string) {
        double temp;
        return (double.TryParse(value.ToString(), out temp)) ? temp : 0;
      }
      return 0;
    }
  }
}
