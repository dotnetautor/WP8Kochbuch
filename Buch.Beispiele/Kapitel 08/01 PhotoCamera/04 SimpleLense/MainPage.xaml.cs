using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Windows.Phone.Media.Capture;
using System.Threading.Tasks;
using Microsoft.Devices;
using System.IO;
using Microsoft.Xna.Framework.Media;

namespace SimpleLense {
  public partial class MainPage : PhoneApplicationPage {
    private PhotoCaptureDevice _cam = null;

    // Constructor
    public MainPage() {
      InitializeComponent();
    }


    private async Task initCameraAsync(CameraSensorLocation sensorLocation) {

      if (_cam != null) {
        _cam.Dispose();
        _cam = null;
      }


      var res = new Windows.Foundation.Size(640, 480);

      _cam = await PhotoCaptureDevice.OpenAsync(sensorLocation, res);
      await _cam.SetPreviewResolutionAsync(res);

      viewfinder.SetSource(_cam);

      viewfinderTransform.Rotation = sensorLocation == CameraSensorLocation.Back ?
                                       _cam.SensorRotationInDegrees : -_cam.SensorRotationInDegrees;



    }


    protected override async void OnNavigatedTo(NavigationEventArgs e) {
      base.OnNavigatedTo(e);
      await initCameraAsync(CameraSensorLocation.Back);
    }

    private async void Button_Click_1(object sender, RoutedEventArgs e) {
      if (_cam.SensorLocation == CameraSensorLocation.Back) {
        await initCameraAsync(CameraSensorLocation.Front);
      } else {
        await initCameraAsync(CameraSensorLocation.Back);
      }
    }

    private async void Button_Click_2(object sender, RoutedEventArgs e) {
      CameraCaptureSequence cameraCaptureSequence = _cam.CreateCaptureSequence(1);

      MemoryStream stream = new MemoryStream();
      cameraCaptureSequence.Frames[0].CaptureStream = stream.AsOutputStream();

      await _cam.PrepareCaptureSequenceAsync(cameraCaptureSequence);
      await cameraCaptureSequence.StartCaptureAsync();

      stream.Seek(0, SeekOrigin.Begin);

      var library = new MediaLibrary();
      library.SavePictureToCameraRoll("pic1.jpg", stream);


    }
  }
}