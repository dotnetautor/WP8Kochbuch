using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVM.Code {
  public class DelegateCommand : ICommand {
    readonly Func<object, bool> canExecute;
    readonly Action<object> executeAction;

    public DelegateCommand(Action<object> executeAction)
      : this(executeAction, null) {
    }

    public DelegateCommand(Action<object> executeAction,
         Func<object, bool> canExecute) {
      if (executeAction == null) {
        throw new
          ArgumentNullException("executeAction");
      }
      this.executeAction = executeAction;
      this.canExecute = canExecute;
    }

    public bool CanExecute(object parameter) {
      bool result = true;
      if (canExecute != null) {
        result = canExecute(parameter);
      }
      return result;
    }

    public event EventHandler CanExecuteChanged;
    public void RaiseCanExecuteChanged() {
      if (CanExecuteChanged != null) {
        CanExecuteChanged(this,
                    new EventArgs());
      }
    }

    public void Execute(object parameter) {
      executeAction(parameter);
    }
  }

}
