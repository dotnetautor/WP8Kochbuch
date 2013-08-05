using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Windows.Phone.Media.Capture;
using System.Threading.Tasks;
using Microsoft.Devices;
using System.IO;
using Microsoft.Xna.Framework.Media;

namespace PhotoCaptureDeviceDemo {

  /// <summary>
  /// Hilfsklasse für die Umwandlung eines ENUM in ein ListPickerItem
  /// </summary>
  /// <typeparam name="T">type des zu verwendenen Enums</typeparam>
  public class EnumListItem<T> where T : struct, IComparable {
    public T? Value { get; set; }

    public string Name { get; private set; }

    public static IEnumerable<EnumListItem<T>> CreateList(string defaultName = null) {
      var res = from t in typeof (T).GetFields()
                where t.IsLiteral
                select new EnumListItem<T> {
                  Value = (T?) t.GetValue(null),
                  Name = t.Name
                };
    return (string.IsNullOrEmpty(defaultName)) ? (res.Union(new [] {new EnumListItem<T>{ Name = defaultName, Value = null} })) : res ;
    }
  }


  public partial class MainPage : PhoneApplicationPage {
    private PhotoCaptureDevice _cam = null;


    // Constructor
    public MainPage() {
      InitializeComponent();

      // init white Balance Settings
      var whiteBalanceItems = EnumListItem<WhiteBalancePreset>.CreateList("Default").ToArray();
      whiteBalanceList.ItemsSource = whiteBalanceItems;
      whiteBalanceList.SelectedItem = whiteBalanceItems.FirstOrDefault(i => i.Value == null) ?? whiteBalanceItems.First();

      var sceneItems = EnumListItem<CameraSceneMode>.CreateList().ToArray();
      sceneList.ItemsSource = sceneItems;
      sceneList.SelectedItem = sceneItems.FirstOrDefault(i => i.Value == CameraSceneMode.Auto) ?? sceneItems.First();

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


      _cam.SetProperty(KnownCameraPhotoProperties.SceneMode, ((EnumListItem<CameraSceneMode>)sceneList.SelectedItem).Value);
      _cam.SetProperty(KnownCameraPhotoProperties.WhiteBalancePreset, ((EnumListItem<WhiteBalancePreset>)whiteBalanceList.SelectedItem).Value);
     
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

      var stream = new MemoryStream();
      cameraCaptureSequence.Frames[0].CaptureStream = stream.AsOutputStream();

      await _cam.PrepareCaptureSequenceAsync(cameraCaptureSequence);
      await cameraCaptureSequence.StartCaptureAsync();

      stream.Seek(0, SeekOrigin.Begin);

      var library = new MediaLibrary();
      library.SavePictureToCameraRoll("pic1.jpg", stream);

    }

    private void whiteBalanceList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      //EnumListItem<WhiteBalancePreset> val;
      //if (_cam != null && e.AddedItems.Count > 0 && (val = e.AddedItems[0] as EnumListItem<WhiteBalancePreset>) != null) {
      //  _cam.SetProperty(KnownCameraPhotoProperties.WhiteBalancePreset, val.Value);
      //}
    }

    private void sceneList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      //EnumListItem<CameraSceneMode> val;
      //if (_cam != null && e.AddedItems.Count > 0 && (val = e.AddedItems[0] as EnumListItem<CameraSceneMode>) != null) {
      //  _cam.SetProperty(KnownCameraPhotoProperties.SceneMode, val.Value);
      //}
    }
  }
}