using System.Collections.ObjectModel;
using FeedreaderCorePortableWinRT.Code;
using MiniMVVM.Code;
using FeedreaderCorePortableWinRT.Model;

namespace FeedreaderCorePortableWinRT.ViewModel {
  public class FeedViewModel : ViewModelBase {
    private ObservableCollection<FeedItem> _items;

    public FeedViewModel() {
      RefreshCommand = new DelegateCommand(async arg =>
        {
          var items = await DataManager.Instance.UpdateFeed("http://dotnetautor.de/GetRssFeed");
          Items = new ObservableCollection<FeedItem>(items);
        });
    }

    public DelegateCommand RefreshCommand { get; private set; }

    public ObservableCollection<FeedItem> Items {
      get { return _items; }
      set {
        if (_items == value) return;
        _items = value;
        OnPropertyChanged();
      }
    }
  }
}
