using System.Windows;
using Binding.ViewModel;
using Microsoft.Phone.Controls;

namespace Binding {
  public partial class MainPage : PhoneApplicationPage {
    // Konstruktor
    public MainPage() {
      InitializeComponent();

      // Beispielcode zur Lokalisierung der ApplicationBar
      //BuildLocalizedApplicationBar();
    }


    private void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
      MainContainer.DataContext = new MyViewModel {
        NurAusgabeText = "Hello Word",
        EinUndAusgabeText = "Verändere mich"
      };

    }
  }
}