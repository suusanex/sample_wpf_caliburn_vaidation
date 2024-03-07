using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using Caliburn.Micro;

namespace sample_wpf_caliburn_vaidation_dotnetfw
{
    public abstract class ValidationScreen : Screen, INotifyDataErrorInfo
    {
        /// <summary>
        /// 初回のValidationが行われたかどうか。未実施の状態ではエラー判定扱いにしたいものが有るため、このフラグを設けた。
        /// </summary>
        protected bool _IsFirstValidate;

        private readonly Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
        public bool HasErrors => errors.Any();

        /// <summary>
        /// INotifyDataErrorInfoを実現するための判定メソッド。Validationを必要とするプロパティが変更された場合、必ずこのメソッドを呼び出す必要がある。
        /// このメソッドの中で、Validationの結果に依存するプロパティのNotifyOfPropertyChangeも呼び出す。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected void Validate(object value, [CallerMemberName] string propertyName = null)
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

        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (!errors.TryGetValue(propertyName, out var val))
            {
                return null;
            }
            return val;
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    }
}