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

        /// <summary>
        /// 全要素のValidationの再判定を行うメソッド。
        /// 初期値がエラーである項目などは、初期値でエラー検出するとユーザーに不要なエラー通知が出るため初期状態では判定を避けたい。
        /// しかしその後に変更されなければエラー判定するタイミングがなく、エラーなし扱いになってしまう。
        /// 入力フォームの確定イベント等でこの判定メソッドを使うことで、そうしたエラーも検出することができる。
        /// </summary>
        protected void AllValidate()
        {
            var needNotifyProperties = new HashSet<string>(errors.Select(d => d.Key));

            var results = new Collection<ValidationResult>();
            var context = new ValidationContext(this);
            if (Validator.TryValidateObject(this, context, results, true))
            {
                errors.Clear();
                foreach (var propertyName in needNotifyProperties)
                {
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                }
            }
            else
            {
                var q = from r in results
                    from m in r.MemberNames
                    group r by m into g
                    select g;
                errors.Clear();
                foreach (var item in q)
                {
                    errors.Add(item.Key, item.Select(d => d.ErrorMessage).ToList());
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(item.Key));
                    needNotifyProperties.Remove(item.Key);
                }

                foreach (var propertyName in needNotifyProperties)
                {
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                }
            }
            
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