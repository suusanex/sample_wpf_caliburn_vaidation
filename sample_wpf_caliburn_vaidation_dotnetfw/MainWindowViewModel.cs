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
    public class MainWindowViewModel : Screen, INotifyDataErrorInfo
    {
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

        [Compare(nameof(Password1Confirm), ErrorMessage = "Field 'Password1' and 'Password1Confirm' are not same.")]
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

        [Compare(nameof(Password1), ErrorMessage = "Field 'Password1' and 'Password1Confirm' are not same.")]
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
            MessageBox.Show("Save");
        }

        public bool CanSave => _IsFirstValidate && !HasErrors;

        /// <summary>
        /// 初回のValidationが行われたかどうか。未実施の状態ではエラー判定扱いにしたいものが有るため、このフラグを設けた。
        /// </summary>
        private bool _IsFirstValidate;
        private readonly Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

        /// <summary>
        /// INotifyDataErrorInfoを実現するための判定メソッド。Validationを必要とするプロパティが変更された場合、必ずこのメソッドを呼び出す必要がある。
        /// このメソッドの中で、Validationの結果に依存するプロパティのNotifyOfPropertyChangeも呼び出す。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private void Validate(object value, [CallerMemberName] string propertyName = null)
        {
            //指定プロパティのエラー判定をして、そのエラー一覧を更新し、更新イベントを発行する。
            //プロパティが変更になった場合に、他のプロパティのエラー状況が変わらないことを前提とした簡易実装。
            //もしそうでない場合は、エラー時にresultsのプロパティ名を見てerrorsの設定先を振り分ける必要がある。
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));

            var results = new Collection<ValidationResult>();
            var context = new ValidationContext(this) { MemberName = propertyName };
            if (Validator.TryValidateProperty(value, context, results))
            {
                errors.Remove(propertyName);
            }
            else
            {
                errors[propertyName] = results.Select(d => d.ErrorMessage).ToList();
            }

            _IsFirstValidate = true;
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

            //Validationの結果に依存するプロパティのNotifyOfPropertyChange呼び出し。ここの呼び出し内容は、各ページの内容によって変わる。
            NotifyOfPropertyChange(nameof(CanSave));

        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (!errors.TryGetValue(propertyName, out var val))
            {
                return null;
            }
            return val;
        }

        public bool HasErrors => errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    }
}
