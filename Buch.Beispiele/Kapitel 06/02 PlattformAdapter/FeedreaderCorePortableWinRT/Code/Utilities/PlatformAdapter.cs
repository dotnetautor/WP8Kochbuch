using System.Net;
using System.Threading.Tasks;

namespace FeedreaderCorePortableWinRT.Code.Utilities {
  public abstract class PlatformAdapter {
    public static PlatformAdapter Current { get; set; }

    public abstract Task<string> ReadResponseStream(HttpWebResponse response);

  }
}
