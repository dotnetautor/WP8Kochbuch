using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Asynchron.Code {
  public static class WebClientExtention {
    public static Task<string> DownloadString(this WebClient client, Uri url) {
      var tcs = new TaskCompletionSource<string>();
      client.DownloadStringCompleted += (s, e) => {
        if (e.Error != null) tcs.TrySetException(e.Error);
        else if (e.Cancelled) tcs.TrySetCanceled();
        else tcs.TrySetResult(e.Result);
      };
      client.DownloadStringAsync(url);
      return tcs.Task;
    }
  }
}
