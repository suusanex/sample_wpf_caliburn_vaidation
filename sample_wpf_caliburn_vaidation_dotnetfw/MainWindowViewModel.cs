using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;

namespace sample_wpf_caliburn_vaidation_dotnetfw
{
    public class MainWindowViewModel : ValidationScreen
    {
        public MainWindowViewModel()
        {
            ErrorsChanged += (sender, args) =>
            {
                //Validationの結果に依存するプロパティのNotifyOfPropertyChange呼び出し。ここの呼び出し内容は、各ページの内容によって変わる。
                NotifyOfPropertyChange(nameof(CanSave));
            };
        }

        [Required(ErrorMessage = "Name is Required")]
        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                _name = value;
                Validate(value);
                NotifyOfPropertyChange();
            }
        }
        private string _name;

        [Range(1, 10, ErrorMessage = "Field 'Number' is out of range. 1 to 10")]
        public int Number
        {
            get => _number;
            set
            {
                if (value == _number) return;
                _number = value;
                Validate(value);
                NotifyOfPropertyChange();
            }
        }
        private int _number;

        [Compare(nameof(Password1Confirm),
            ErrorMessageResourceType = typeof(Properties.Resources), ErrorMessageResourceName = "Password1ConfirmErrorMessage")]
        [MinLength(2, ErrorMessage = "Field 'Password1' is too short. 2 or more characters")]
        public string Password1
        {
            get => _password1;
            set
            {
                if (value == _password1) return;
                _password1 = value;
                Validate(value);
                //Compareの依存先に影響するので、依存先の判定も行う
                Validate(Password1Confirm, nameof(Password1Confirm));
                NotifyOfPropertyChange();
            }
        }
        private string _password1;

        [Compare(nameof(Password1), 
            ErrorMessageResourceType = typeof(Properties.Resources), ErrorMessageResourceName = "Password1ConfirmErrorMessage")]
        [MinLength(2, ErrorMessage = "Field 'Password1Confirm' is too short. 2 or more characters")]
        public string Password1Confirm
        {
            get => _password1Confirm;
            set
            {
                if (value == _password1Confirm) return;
                _password1Confirm = value;
                Validate(value);
                //Compareの依存先に影響するので、依存先の判定も行う
                Validate(Password1, nameof(Password1));
                NotifyOfPropertyChange();
            }
        }
        private string _password1Confirm;

        public void Save()
        {
            AllValidate();
            if (!CanSave) return;

            MessageBox.Show("Save");
        }

        public bool CanSave => _IsFirstValidate && !HasErrors;

    }
}
