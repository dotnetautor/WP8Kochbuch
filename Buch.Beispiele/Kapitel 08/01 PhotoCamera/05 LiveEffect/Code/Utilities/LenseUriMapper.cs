using System;
using System.Windows.Navigation;

namespace LiveEffect.Code.Utilities {
  public class LenseUriMapper : UriMapperBase {
    private string _tempUri;

    public override Uri MapUri(Uri uri) {
      _tempUri = uri.ToString();

      // Look for a URI from the lens picker.
      if (_tempUri.Contains("ViewfinderLaunch")) {
        // Launch as a lens, launch viewfinder screen.
        return new Uri("/Pages/LensePage.xaml", UriKind.Relative);
      }

      // Otherwise perform normal launch.
      return uri;
    }
  }
}
