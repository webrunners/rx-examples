using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RxTwitterSample.viewmodels
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression != null)
            {
                OnPropertyChanged(memberExpression.Member.Name);
            }
        }

        protected void SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public static class NotifyPropertyChangedExtensions
    {
        public static IObservable<T> ToObservable<T>(this INotifyPropertyChanged source, Expression<Func<T>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            return memberExpression == null
                ? Observable.Empty<T>()
                : Observable
                    .FromEventPattern<PropertyChangedEventArgs>(source, "PropertyChanged")
                    .Where(e => e.EventArgs.PropertyName == memberExpression.Member.Name)
                    .Select(_ => source.GetType()
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .FirstOrDefault(info => info.Name == memberExpression.Member.Name))
                    .Where(info => info != null)
                    .Select(info => (T) info.GetValue(source));
        }
    }
}
