using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EasyPhoto {
  public class PreviewMStreamSource : MediaStreamSource {

    private const int FramePixelSize = 4; // RGBA
    private const int NumberOfBufferedFrames = 1;


    protected override void OpenMediaAsync() {
      throw new NotImplementedException();
    }

    protected override void SeekAsync(long seekToTime) {
      throw new NotImplementedException();
    }

    protected override void GetSampleAsync(MediaStreamType mediaStreamType) {
      CameraStreamSourceDataSingleton dataSource = CameraStreamSourceDataSingleton.Instance;

      if (frameStreamOffset + dataSource.FrameBufferSize > dataSource.FrameStreamSize) {
        dataSource.FrameStream.Seek(0, SeekOrigin.Begin);
        frameStreamOffset = 0;
      }

      Task tsk = dataSource.CameraEffect.GetNewFrameAndApplyEffect(dataSource.ImageBuffer.AsBuffer());

      // Wait that the asynchroneous call completes, and proceed by reporting 
      // the MediaElement that new samples are ready.
      tsk.ContinueWith((task) => {
        dataSource.FrameStream.Position = 0;

        var msSample = new MediaStreamSample(
            videoStreamDescription,
            dataSource.FrameStream,
            frameStreamOffset,
            dataSource.FrameBufferSize,
            currentTime,
            emptySampleDict);

        ReportGetSampleCompleted(msSample);
        frameCount++;
        currentTime += frameTime;
        frameStreamOffset += dataSource.FrameBufferSize;
      });
    }

    #region -= Nicht benötigte Member =-
    protected override void SwitchMediaStreamAsync(MediaStreamDescription mediaStreamDescription) {
      throw new NotImplementedException();
    }

    protected override void GetDiagnosticAsync(MediaStreamSourceDiagnosticKind diagnosticKind) {
      throw new NotImplementedException();
    }

    protected override void CloseMedia() {
      // keine Implementierung erforderlich
    }
    #endregion
  }
}
