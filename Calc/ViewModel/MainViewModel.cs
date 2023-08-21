using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Calculator
{
    class MainViewModel : INotifyPropertyChanged
    {
        #region [변수]
        MainModel model = new MainModel();
        string displayExpression;
        string displayResult;
        string preInput;
        string unuseds;
        string jipgagosipda;
        #endregion


        #region [이벤트 핸들러]
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region [필드]
        #endregion

        #region [속성]
        public string DisplayExpression
        {
            get => displayExpression;
            set
            {
                displayExpression = value;
                OnPropertyChanged(nameof(DisplayExpression));
            }
        }

        public string DisplayResult
        {
            get => displayResult;
            set
            {
                displayResult = value;
                OnPropertyChanged(nameof(displayResult));
            }
        }
        public ICommand NumberCommand
        {
            get;
            set;
        }

        public ICommand OperatorCommand
        {
            get;
            set;
        }

        public ICommand ClearCommand
        {
            get;
            set;
        }

        public ICommand EqualCommand
        {
            get;
            set;
        }
        #endregion

        #region [생성자]
        public MainViewModel()
        {
            displayExpression = "";
            displayResult = "0";
            preInput = "0";
            NumberCommand = new RelayCommand(GetNumber);
            OperatorCommand = new RelayCommand(GetOperator);
            ClearCommand = new RelayCommand(getClearSign);
            EqualCommand = new RelayCommand(GetEqual);
        }

        #endregion

        #region [public 메서드]
        #endregion

        #region [private 메서드]
        private void onPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void GetNumber(object inputNumber)
        {
            if (inputNumber != null)
            {
                string number = inputNumber.ToString();
                if (preInput[0] > '0' && preInput[0] <= '9')
                {
                    number = $"{preInput}{number}";
                }

                DisplayResult = number;
                preInput = number;
            }

        }

        private void GetOperator(object inputOpterator)
        {
            if (inputOpterator != null)
            {
                string opterator = inputOpterator.ToString();

                if (opterator == "/")
                    opterator = "÷";
                else if (opterator == "*")
                    opterator = "×";

                if (preInput == "+" || preInput == "-" || preInput == "/" || preInput == "*")
                {
                    DisplayExpression = displayExpression.Substring(0, displayExpression.Length - 1);
                    DisplayExpression = $"{displayExpression}{opterator}";
                    return;
                }

                if (displayExpression == "")
                    DisplayExpression = $"{displayResult}{opterator}";
                else
                    DisplayExpression = $"{displayExpression}{displayResult}{opterator}";

                preInput = opterator;

            }

        }

        private void getClearSign(object inputClearSIgn)
        {
            displayExpression = "";
        }

        private void GetEqual()
        {

        }

        #endregion

        #region [중첩된 클래스]
        #endregion

    }
}
