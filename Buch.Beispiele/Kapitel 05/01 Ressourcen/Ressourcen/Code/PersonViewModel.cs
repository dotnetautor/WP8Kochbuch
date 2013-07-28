using System.Collections.ObjectModel;
using System.Windows.Input;
using MiniMVVM.Code;

namespace MVVM.Code {
  public class PersonViewModel {
    private readonly ObservableCollection<Person> _personDataSource;
    private readonly ICommand _loadDataCommand;

    public PersonViewModel() {
      _personDataSource = new ObservableCollection<Person>();
      _loadDataCommand = new DelegateCommand(this.LoadDataAction);
    }

    private void LoadDataAction(object p) {
      DataSource.Add(new Person() { Name = "John" });
      DataSource.Add(new Person() { Name = "Kate" });
      DataSource.Add(new Person() { Name = "Sam" });
    }

    public ICommand LoadDataCommand {
      get { return _loadDataCommand; }
    }

    public ObservableCollection<Person> DataSource {
      get { return _personDataSource; }
    }
  }
}