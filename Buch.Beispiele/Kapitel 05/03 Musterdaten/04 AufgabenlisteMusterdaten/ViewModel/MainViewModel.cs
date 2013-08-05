using Aufgabenliste.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;

namespace Aufgabenliste.ViewModel {

  public class MainViewModel : ViewModelBase {

    private ObservableCollection<TodoItem> _items;
    public ObservableCollection<TodoItem> Items {
      get { return _items; }
      set {
        if (_items == value) return;
        _items = value;
        RaisePropertyChanged("Items");
      }
    }

    private string _todoText;
    public string TodoText {
      get { return _todoText; }
      set {
        if (_todoText == value) return;
        _todoText = value;
        RaisePropertyChanged("TodoText");
      }
    }

    public RelayCommand AddCommand { get; private set; }
    public RelayCommand<TodoItem> DelCommand { get; set; }

    /// <summary>
    /// Initializes a new instance of the MainViewModel class.
    /// </summary>
    public MainViewModel() {
      if (IsInDesignMode) {
        // Der Programmcode läuft im Designmode z.B. in Blend.
      } else {
        Items = new ObservableCollection<TodoItem>();

        TodoText = "";

        AddCommand = new RelayCommand(() => {
          var item = new TodoItem { Name = TodoText, IsDone = false };
          _items.Add(item);
        });

        DelCommand = new RelayCommand<TodoItem>(item => {
          _items.Remove(item);
        });
      }
    }
  }
}