using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;

namespace CameraCaptureTaskDemo {
  public partial class MainPage : PhoneApplicationPage {
    private readonly CameraCaptureTask _cameraTask;

    // Constructor
    public MainPage() {
      InitializeComponent();

      _cameraTask = new CameraCaptureTask();
      _cameraTask.Completed += cameraTask_Completed;
    }

    private void cameraTask_Completed(object sender, PhotoResult e) {
      if (e.TaskResult == TaskResult.OK) {
        var bmp = new BitmapImage();
        bmp.SetSource(e.ChosenPhoto);
        image1.Source = bmp;
      }
    }

    private void Button_Click_1(object sender, RoutedEventArgs e) {
      _cameraTask.Show();
    }

  }
}