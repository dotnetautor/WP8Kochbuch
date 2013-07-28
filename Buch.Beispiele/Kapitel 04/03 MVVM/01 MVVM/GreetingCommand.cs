using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MVVM {
  public class GreetingCommand : ICommand {
    private bool _executed = false;

    public bool CanExecute(object parameter) {
      return !_executed;
    }

    public void Execute(object parameter) {
      _executed = true;
      OnCanExecuteChanged();
      MessageBox.Show(parameter.ToString());
    }

    private void OnCanExecuteChanged() {
      if (CanExecuteChanged != null) {
        CanExecuteChanged(this, new EventArgs());
      }
    }

    public event EventHandler CanExecuteChanged;
  }
}
