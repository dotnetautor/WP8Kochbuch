using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using FeedreaderCorePortableWinRT.Code.Utilities;

namespace FeedReaderWin8.Utilities {
  internal class WindowsPlatformAdapter : PlatformAdapter {
    public override async Task<string> ReadResponseStream(HttpWebResponse response) {
      string result;

      using (var gzipStream =
        (string.Equals(response.Headers["Content-Encoding"], "gzip", StringComparison.CurrentCultureIgnoreCase))
          ? new GZipStream(response.GetResponseStream(), CompressionMode.Decompress)
          : response.GetResponseStream()) {

        using (var sr = new StreamReader(gzipStream)) {
          result = await sr.ReadToEndAsync();
        }
      }

      return result;
    }

  }
}
