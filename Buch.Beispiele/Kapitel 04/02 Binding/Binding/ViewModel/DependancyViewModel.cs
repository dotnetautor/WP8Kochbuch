using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Binding.ViewModel {
  public class DependancyViewModel : DependencyObject {
    public static readonly DependencyProperty UserNameProperty =
    DependencyProperty.Register("UserName", typeof(string), typeof(DependancyViewModel), null);

    public string UserName {
      get { return (string)GetValue(UserNameProperty); }
      set { SetValue(UserNameProperty, value); }
    }

  }
}
