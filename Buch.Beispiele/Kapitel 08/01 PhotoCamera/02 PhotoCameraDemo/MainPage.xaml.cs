using System.Windows;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Devices;
using Microsoft.Xna.Framework.Media;

namespace PhotoCameraDemo {
  public partial class MainPage : PhoneApplicationPage {
    private PhotoCamera _cam;
    private int _imgCounter = 0;
    private readonly MediaLibrary _library = new MediaLibrary();

    // Constructor
    public MainPage() {
      InitializeComponent();
    }

    protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e) {

      _cam = new PhotoCamera();
      previewCanvas.SetSource(_cam);
      previewCanvas.RelativeTransform = new CompositeTransform() {CenterX = 0.5, CenterY = 0.5, Rotation = 90};

      _cam.CaptureImageAvailable += (sender, args) => {
          //var library = new MediaLibrary();
          string fileName = "wp8_kochbuch_" + _imgCounter + ".jpg";
          _library.SavePictureToCameraRoll(fileName, args.ImageStream);
        };

      _cam.CaptureCompleted += (sender, args) => { _imgCounter++; };
      
      CameraButtons.ShutterKeyPressed += (sender, args) => _cam.CaptureImage();
      CameraButtons.ShutterKeyHalfPressed += (sender, args) => _cam.Focus();
      //CameraButtons.ShutterKeyReleased += (sender, args) => { };

      base.OnNavigatedTo(e);
    }

   
    protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e) {

      if (_cam != null) {
        _cam.Dispose();
      }

      base.OnNavigatedFrom(e);
    }

    private void btnFlashOn_Click(object sender, RoutedEventArgs e) {
      _cam.FlashMode = FlashMode.Auto;
    }

    private void btnFlashOff_Click(object sender, RoutedEventArgs e) {
      _cam.FlashMode = FlashMode.Off;
    }

    private void btnAF_Click(object sender, RoutedEventArgs e) {
      _cam.Focus();
    }


    private void btnSnap_Click(object sender, RoutedEventArgs e) {
      _cam.CaptureImage();
    }
  }
}