using MiniMVVM.Code;

namespace MiniMVVM.ViewModel {
  public class CalculatorViewModel : ViewModelBase {
    private double _firstValue;
    private double _secondValue;
    private double _resultValue;

    public CalculatorViewModel() {
      FirstValue = 0;
      SecondValue = 0;
      ResultValue = 0;

      AddCommand = new DelegateCommand((param) =>
        {
          ResultValue = FirstValue + SecondValue;
        });

    }

    public double FirstValue {
      get { return _firstValue; }
      set {
        if (FirstValue == value) ;
        _firstValue = value;
        OnPropertyChanged();
      }
    }

    public double SecondValue {
      get { return _secondValue; }
      set {
        if (_secondValue == value) return;
        _secondValue = value;
        OnPropertyChanged();
      }
    }

    public double ResultValue {
      get { return _resultValue; }
      set {
        if (ResultValue == value) return;
        _resultValue = value;
        OnPropertyChanged();
      }

    }
    public DelegateCommand AddCommand { get; set; }

  }
}
