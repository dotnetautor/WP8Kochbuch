using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ImagingSDK.Resources;
using Nokia.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using Nokia.Graphics.Imaging;

namespace ImagingSDK {
  public partial class MainPage : PhoneApplicationPage {
    // Constructor
    public MainPage() {
      InitializeComponent();


    }

    protected override async void OnNavigatedTo(NavigationEventArgs e) {

      StreamResourceInfo sri = Application.GetResourceStream(new Uri("DemoImage.jpg", UriKind.Relative));
      var myBitmap = new WriteableBitmap(800, 480);
      img.ImageSource = myBitmap;

      using (var editsession = await EditingSessionFactory.CreateEditingSessionAsync(sri.Stream)) {
        // First add an antique effect 
       editsession.AddFilter(FilterFactory.CreateCartoonFilter(true));


       editsession.AddFilter(FilterFactory.CreateAntiqueFilter());
       // Then rotate the image
       editsession.AddFilter(FilterFactory.CreateFreeRotationFilter(35.0f, RotationResizeMode.FitInside));


        await editsession.RenderToBitmapAsync(myBitmap.AsBitmap());

   
      }
     
      myBitmap.Invalidate();

      base.OnNavigatedTo(e);
    }
  }
}