
/**
 * Copyright (c) 2013 Nokia Corporation.
 */


using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using LiveEffect.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Xna.Framework.Media;
using RealtimeFilterDemo;
using Windows.Phone.Media.Capture;
using Size = Windows.Foundation.Size;

namespace LiveEffect.Pages {
  public partial class LensePage : PhoneApplicationPage {


    // Constants
    private const String BackIconUri = "/Assets/Icons/back.png";
    private const String NextIconUri = "/Assets/Icons/next.png";
    private const String AboutPageUri = "/Pages/AboutPage.xaml";
    private const double AspectRatio = 4.0/3.0;
    private const double MediaElementWidth = 640;
    private const double MediaElementHeight = 480;

    // Members
    private PhotoCaptureDevice _camera;
    private readonly ICameraEffect _cameraEffect;
    private CameraStreamSource _source;
    private MediaElement _mediaElement;
    private Semaphore _focusSemaphore = new Semaphore(1,1);
    private Size _focusRegionSize = new Size(80,80);
    private readonly SolidColorBrush _notFocusedBrush = new SolidColorBrush(Colors.Red);
    private readonly SolidColorBrush _focusedBrush = new SolidColorBrush(Colors.Green);
    private bool _manuallyFocused;
    private bool _capturing;


    /// <summary>
    /// Constructor.
    /// </summary>
    public LensePage() {
      InitializeComponent();
      _cameraEffect = new NokiaImagingSDKEffects();
      BuildApplicationBar();
    }

    /// <summary>
    /// Creates the application bar and its items.
    /// </summary>
    private void BuildApplicationBar() {
      ApplicationBar = new ApplicationBar();

      var button =
        new ApplicationBarIconButton(new Uri(BackIconUri, UriKind.Relative)) {
          Text = AppResources.PreviousEffectButtonText
        };
      button.Click += OnBackButtonClicked;
      ApplicationBar.Buttons.Add(button);

      button = new ApplicationBarIconButton(new Uri(NextIconUri, UriKind.Relative)) {
        Text = AppResources.NextEffectButtonText
      };
      button.Click += OnNextButtonClicked;
      ApplicationBar.Buttons.Add(button);

      var menuItem =
        new ApplicationBarMenuItem {
          Text = AppResources.AboutPageButtonText
        };
      menuItem.Click += OnAboutPageButtonClicked;
      ApplicationBar.MenuItems.Add(menuItem);
    }

    /// <summary>
    /// Opens and sets up the camera if not already. Creates a new
    /// CameraStreamSource with an effect and shows it on the screen via
    /// the media element.
    /// </summary>
    private async void Initialize() {
      Debug.WriteLine("MainPage.Initialize()");
      var mediaElementSize = new Size(MediaElementWidth, MediaElementHeight);

      if (_camera == null) {
        // Resolve the capture resolution and open the camera
        var captureResolutions =
          PhotoCaptureDevice.GetAvailableCaptureResolutions(CameraSensorLocation.Back);

        var selectedCaptureResolution =
          captureResolutions.Where(
            resolution => Math.Abs(AspectRatio - resolution.Width/resolution.Height) <= 0.1)
                            .OrderBy(resolution => resolution.Width).Last();

        _camera = await PhotoCaptureDevice.OpenAsync(
          CameraSensorLocation.Back, selectedCaptureResolution);

        // Set the image orientation prior to encoding
        _camera.SetProperty(KnownCameraGeneralProperties.EncodeWithOrientation,
                           _camera.SensorLocation == CameraSensorLocation.Back
                           ? _camera.SensorRotationInDegrees : -_camera.SensorRotationInDegrees);

        // Resolve and set the preview resolution
        var previewResolutions =
          PhotoCaptureDevice.GetAvailablePreviewResolutions(CameraSensorLocation.Back);

        Size selectedPreviewResolution =
          previewResolutions.Where(
            resolution => Math.Abs(AspectRatio - resolution.Width/resolution.Height) <= 0.1)
                            .Where(resolution => (resolution.Height >= mediaElementSize.Height)
                                                 && (resolution.Width >= mediaElementSize.Width))
                            .OrderBy(resolution => resolution.Width).First();

        await _camera.SetPreviewResolutionAsync(selectedPreviewResolution);

        _cameraEffect.CaptureDevice = _camera;
      }

      if (_mediaElement == null) {
        _mediaElement = new MediaElement {
          Stretch = Stretch.UniformToFill, 
          BufferingTime = new TimeSpan(0)
        };
        _mediaElement.Tap += OnMyCameraMediaElementTapped;
        _source = new CameraStreamSource(_cameraEffect, mediaElementSize);
        _mediaElement.SetSource(_source);
        MediaElementContainer.Children.Add(_mediaElement);
        _source.FPSChanged += OnFPSChanged;
      }

      // Show the index and the name of the current effect
      if (_cameraEffect is NokiaImagingSDKEffects) {
        var effects =
          _cameraEffect as NokiaImagingSDKEffects;

        EffectNameTextBlock.Text =
          (effects.EffectIndex + 1) + "/"
          + NokiaImagingSDKEffects.NumberOfEffects
          + ": " + effects.EffectName;
      } else {
        EffectNameTextBlock.Text = _cameraEffect.EffectName;
      }
    }

   
     /// <summary>
     /// Half-pressing the shutter key initiates autofocus.
     /// </summary>
     private async void ShutterKeyHalfPressed(object sender, EventArgs e)
     {
         if (_manuallyFocused){
             _manuallyFocused = false;
         }

         FocusIndicator.SetValue(Canvas.VisibilityProperty, Visibility.Collapsed);
         AutoFocus();
     }

    /// <summary>
    /// Completely pressing the shutter key initiates capturing a photo.
    /// </summary>
    private void ShutterKeyPressed(object sender, EventArgs e) {
      TakePicture();
    }

    private async void AutoFocus()
    {
        if (!_capturing && PhotoCaptureDevice.IsFocusSupported(_camera.SensorLocation)){
            
            await _camera.FocusAsync();

            _capturing = false;
        }
    }

    private async void TakePicture() {
      if (!_capturing) {
        _capturing = true;
        CameraCaptureSequence cameraCaptureSequence = _camera.CreateCaptureSequence(1);

        var stream = new MemoryStream();
        cameraCaptureSequence.Frames[0].CaptureStream = stream.AsOutputStream();

        await _camera.PrepareCaptureSequenceAsync(cameraCaptureSequence);
        await cameraCaptureSequence.StartCaptureAsync();

        var effect = await _cameraEffect.ApplyEffect(stream);
        // store the processed image

        var library = new MediaLibrary();
        library.SavePictureToCameraRoll(String.Format("WP8_{0:yyyyMMdd_hhmmss}.jpg", DateTime.Now), effect);

        //// store the original
        //stream.Seek(0, SeekOrigin.Begin);
        //var library = new MediaLibrary();
        //library.SavePictureToCameraRoll("pic1.jpg", stream);
        _capturing = false;
      }
    }

    /// <summary>
    /// Changes the camera effect.
    /// </summary>
    /// <param name="next">If true, will increase the effect index.
    /// If false, will decrease it.</param>
    private void ChangeEffect(bool next) {
      if (_cameraEffect == null || !(_cameraEffect is NokiaImagingSDKEffects)) {
        return;
      }

      var effects = _cameraEffect as NokiaImagingSDKEffects;

      if (next) {
        if (effects.EffectIndex < NokiaImagingSDKEffects.NumberOfEffects - 1) {
          effects.EffectIndex++;
        } else {
          effects.EffectIndex = 0;
        }
      } else {
        if (effects.EffectIndex > 0) {
          effects.EffectIndex--;
        } else {
          effects.EffectIndex = NokiaImagingSDKEffects.NumberOfEffects - 1;
        }
      }

      EffectNameTextBlock.Text = (effects.EffectIndex + 1) + "/" + NokiaImagingSDKEffects.NumberOfEffects + ": " + _cameraEffect.EffectName;
    }

    /// <summary>
    /// From PhoneApplicationPage.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnNavigatedTo(NavigationEventArgs e) {
      Debug.WriteLine("MainPage.OnNavigatedTo()");
      Initialize();

      Microsoft.Devices.CameraButtons.ShutterKeyHalfPressed += ShutterKeyHalfPressed;
      Microsoft.Devices.CameraButtons.ShutterKeyPressed += ShutterKeyPressed;

      base.OnNavigatedTo(e);
    }

    /// <summary>
    /// From PhoneApplicationPage.
    /// Sets the media element source to null and disconnects the event
    /// handling.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnNavigatedFrom(NavigationEventArgs e) {
      Debug.WriteLine("MainPage.OnNavigatedFrom()");
      MediaElementContainer.Children.Remove(_mediaElement);
      _mediaElement = null;

      if (_camera != null) {
        _camera.Dispose();
        _camera = null;
      }

      Microsoft.Devices.CameraButtons.ShutterKeyHalfPressed -= ShutterKeyHalfPressed;
      Microsoft.Devices.CameraButtons.ShutterKeyPressed -= ShutterKeyPressed;


      _source.FPSChanged -= OnFPSChanged;
    }

    /// <summary>
    /// Removes the source from the camera media element, so that there
    /// would be no attempts to update the content of this page while
    /// about page is shown, and then opens the about page.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnAboutPageButtonClicked(object sender, EventArgs e) {
      if (_camera == null) {
        return;
      }

      NavigationService.Navigate(new Uri(AboutPageUri, UriKind.Relative));
    }

    /// <summary>
    /// Changes the current camera effect.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnBackButtonClicked(object sender, EventArgs e) {
      ChangeEffect(false);
    }

    /// <summary>
    /// Changes the current camera effect.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnNextButtonClicked(object sender, EventArgs e) {
      ChangeEffect(true);
    }

    /// <summary>
    /// Changes the current camera effect.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnMyCameraMediaElementTapped(object sender, GestureEventArgs e) {
      var uiTapPoint = e.GetPosition(_mediaElement);
      if (_focusSemaphore.WaitOne(0)) {
        // Get tap coordinates as a foundation point
        var tapPoint = new Windows.Foundation.Point(uiTapPoint.X, uiTapPoint.Y);

        
        // adjust to center focus on the tap point
        var displayOrigin = new Windows.Foundation.Point(
            tapPoint.X - _focusRegionSize.Width / 2,
            tapPoint.Y - _focusRegionSize.Height / 2);

        // adjust for resolution difference between preview image and the canvas
        var viewFinderOrigin = new Windows.Foundation.Point(displayOrigin.X , displayOrigin.Y);
        var focusrect = new Windows.Foundation.Rect(viewFinderOrigin, _focusRegionSize);

        // clip to preview resolution
        var viewPortRect = new Windows.Foundation.Rect(0, 0,_camera.PreviewResolution.Width,_camera.PreviewResolution.Height);
        focusrect.Intersect(viewPortRect);

       _camera.FocusRegion = focusrect;

        // show a focus indicator
        FocusIndicator.Margin = new Thickness(uiTapPoint.X - (_focusRegionSize.Width/2), uiTapPoint.Y - (_focusRegionSize.Height/2), 0, 0);
        FocusIndicator.SetValue(Shape.StrokeProperty, _notFocusedBrush);
        FocusIndicator.SetValue(Canvas.VisibilityProperty, Visibility.Visible);

        CameraFocusStatus status = await _camera.FocusAsync();
        if (status == CameraFocusStatus.Locked) {
          FocusIndicator.SetValue(Shape.StrokeProperty, _focusedBrush);
          _manuallyFocused = true;
         _camera.SetProperty(KnownCameraPhotoProperties.LockedAutoFocusParameters,
              AutoFocusParameters.Exposure & AutoFocusParameters.Focus & AutoFocusParameters.WhiteBalance);
        } else {
          _manuallyFocused = false;
         _camera.SetProperty(KnownCameraPhotoProperties.LockedAutoFocusParameters, AutoFocusParameters.None);
        }
        _focusSemaphore.Release();
      }
    }

    /// <summary>
    /// Updates the FPS count on the screen.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnFPSChanged(object sender, int e) {
      // Uncomment the following to display the frame rate on the screen
      //FPSTextBlock.Text = "FPS: " + e;
    }
  }
}