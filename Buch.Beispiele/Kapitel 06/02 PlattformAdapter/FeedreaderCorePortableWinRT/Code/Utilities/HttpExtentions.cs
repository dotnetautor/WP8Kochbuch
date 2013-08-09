using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FeedreaderCorePortableWinRT.Code.Utilities {
  public static class HttpExtensions {
    public static Task<HttpWebResponse> GetResponseAsync(this HttpWebRequest request) {
      var taskComplete = new TaskCompletionSource<HttpWebResponse>();
      request.BeginGetResponse(asyncResponse => {
        try {
          var responseRequest = (HttpWebRequest)asyncResponse.AsyncState;
          var someResponse = (HttpWebResponse)responseRequest.EndGetResponse(asyncResponse);
          taskComplete.TrySetResult(someResponse);
        } catch (WebException webExc) {
          var failedResponse = (HttpWebResponse)webExc.Response;
          taskComplete.TrySetResult(failedResponse);
        }
      }, request);
      return taskComplete.Task;
    }

    public static Task<Stream> GetRequestStreamAsync(this HttpWebRequest request) {
      var taskComplete = new TaskCompletionSource<Stream>();
      request.BeginGetRequestStream(asyncResponse => {
        try {
          var responseRequest = (HttpWebRequest)asyncResponse.AsyncState;
          var someResponse = (Stream)responseRequest.EndGetRequestStream(asyncResponse);
          taskComplete.TrySetResult(someResponse);
        } catch (WebException webExc) {
          var failedResponse = (HttpWebResponse)webExc.Response;
          taskComplete.TrySetResult(null);
        }
      }, request);
      return taskComplete.Task;
    }
  } 
}
