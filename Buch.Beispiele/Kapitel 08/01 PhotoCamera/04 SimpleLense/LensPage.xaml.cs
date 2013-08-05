using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Windows.Phone.Media.Capture;
using System.Threading.Tasks;
using Microsoft.Devices;
using System.IO;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SimpleLense {




  public partial class LensPage : PhoneApplicationPage {
    private PhotoCaptureDevice cam = null;
    private WriteableBitmap bmp = null;
    private DispatcherTimer timer = null;

    // Constructor
    public LensPage() {
      InitializeComponent();

    }

    private async Task initCameraAsync(CameraSensorLocation sensorLocation) {

      if (cam != null) {
        cam.Dispose();
        cam = null;
      }


      Windows.Foundation.Size res = new Windows.Foundation.Size(640, 480);

      cam = await PhotoCaptureDevice.OpenAsync(sensorLocation, res);
      await cam.SetPreviewResolutionAsync(res);

      viewfinder.SetSource(cam);

      viewfinderTransform.Rotation = sensorLocation == CameraSensorLocation.Back ?
                                       cam.SensorRotationInDegrees : -cam.SensorRotationInDegrees;

      imgFilterTransform.Rotation = sensorLocation == CameraSensorLocation.Back ?
                                       cam.SensorRotationInDegrees : -cam.SensorRotationInDegrees;

      // Vorbereitung für die Live s/w Vorschau
      bmp = new WriteableBitmap((int)cam.PreviewResolution.Width,  (int)cam.PreviewResolution.Height);
      timer = new DispatcherTimer {
        Interval = TimeSpan.FromMilliseconds(10)
      };
      timer.Tick += timer_Tick;
      timer.Start();

    }


    protected override async void OnNavigatedTo(NavigationEventArgs e) {
      base.OnNavigatedTo(e);
      await initCameraAsync(CameraSensorLocation.Back);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e) {
      if (timer != null) {
        timer.Stop();
      }
      
      base.OnNavigatedFrom(e);
    }

    private async void Button_Click_1(object sender, RoutedEventArgs e) {
      
      if (timer != null) {
        timer.Stop();
      }
      
      if (cam.SensorLocation == CameraSensorLocation.Back) {
        await initCameraAsync(CameraSensorLocation.Front);
      } else {
        await initCameraAsync(CameraSensorLocation.Back);
      }
      
    }

    private async void Button_Click_2(object sender, RoutedEventArgs e) {
      CameraCaptureSequence cameraCaptureSequence = cam.CreateCaptureSequence(1);

      MemoryStream stream = new MemoryStream();
      cameraCaptureSequence.Frames[0].CaptureStream = stream.AsOutputStream();

      await cam.PrepareCaptureSequenceAsync(cameraCaptureSequence);
      await cameraCaptureSequence.StartCaptureAsync();

      stream.Seek(0, SeekOrigin.Begin);

      MediaLibrary library = new MediaLibrary();
      library.SavePictureToCameraRoll("pic1.jpg", stream);


    }

    void timer_Tick(object sender, EventArgs e) {
      int[] pixelData = new int[(int)(cam.PreviewResolution.Width * cam.PreviewResolution.Height)];
      cam.GetPreviewBufferArgb(pixelData);
      for (int i = 0; i < pixelData.Length; i++) {
        pixelData[i] = ColorToGray(pixelData[i]);
      }
      pixelData.CopyTo(bmp.Pixels, 0);
      imgFilter.ImageSource = bmp;
    }


    protected void GetARGB(int color, out int a, out int r, out int g, out int b) {
      a = color >> 24;
      r = (color & 0x00ff0000) >> 16;
      g = (color & 0x0000ff00) >> 8;
      b = (color & 0x000000ff);
    }

    private int ColorToGray(int color) {
      int gray = 0;
      int a, r, g, b;
      GetARGB(color, out a, out r, out g, out b);
      if ((r == g) && (g == b)) {
        gray = color;
      } else {
        int i = (7 * r + 38 * g + 19 * b + 32) >> 6;
        gray = ((0xFF) << 24) | ((i & 0xFF) << 16) | ((i & 0xFF) << 8) | (i & 0xFF);
      }
      return gray;
    }


  }
}