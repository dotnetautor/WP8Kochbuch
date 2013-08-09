using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FeedreaderCorePortableWinRT.Code.Utilities;
using TwitterWP8.Utilities;

namespace FeedReaderWP7.Utilities {
  public class PhonePlatformAdapter : PlatformAdapter {
    public override async Task<string> ReadResponseStream(HttpWebResponse response) {

      string result = "";
      Stream stream = (string.Equals(response.Headers[HttpRequestHeader.ContentEncoding], "gzip", StringComparison.OrdinalIgnoreCase)) 
                        ? await TaskEx.Run(() => (response.GetResponseStream().Decompress()) ) 
                        : response.GetResponseStream();
      
      using (var reader = new StreamReader(stream)) {
        result = await reader.ReadToEndAsync();
      }

      return result;
    }
    
  }
}

