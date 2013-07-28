using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binding.ViewModel {
  public class SimpleViewModel : INotifyPropertyChanged {

    private string _username;
    public string Username {
      get { return _username; }
      set {
        if (_username == value) return;
        _username = value;
        OnPropertyChange("Username");
      }
    }    
    
    private void OnPropertyChange(string name){
      if (PropertyChanged != null) {
        PropertyChanged(this, new PropertyChangedEventArgs(name));
       }
    }

    public event PropertyChangedEventHandler PropertyChanged;
  }
}
