using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Input;

namespace PROSforWindows.Commands
{
    public class ListenCommand : ICommand
    {
        public Predicate<object> CanExecuteDelegate { get; set; }

        private List<INotifyPropertyChanged> propertiesToListenTo;
        private List<WeakReference> ControlEvent;

        public ListenCommand()
        {
            ControlEvent = new List<WeakReference>();
        }

        public List<INotifyPropertyChanged> PropertiesToListenTo
        {
            get { return propertiesToListenTo; }
            set { propertiesToListenTo = value; }
        }

        private Action<object> execute;


        public Action<object> Execute
        {
            get { return execute; }
            set
            {
                execute = value;
                ListenForNotificationFrom((INotifyPropertyChanged)execute.Target);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            if (ControlEvent != null && ControlEvent.Count > 0)
                ControlEvent.ForEach(a =>
                {
                    ((EventHandler)a?.Target).Invoke(null, EventArgs.Empty);
                });
        }

        public ListenCommand ListenOn<TObservedType, TPropertyType>(TObservedType viewModel, Expression<Func<TObservedType, TPropertyType>> propertyExpression) where TObservedType : INotifyPropertyChanged
        {
            string propertyName = GetPropertyName(propertyExpression);
            viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == propertyName) RaiseCanExecuteChanged();
            };
            return this;
        }

        public void ListenForNotificationFrom<TObservedType>(TObservedType viewModel) where TObservedType : INotifyPropertyChanged
        {
            viewModel.PropertyChanged += (sender, e) => RaiseCanExecuteChanged();
        }

        private string GetPropertyName<TObservedType, TPropertyType>(Expression<Func<TObservedType, TPropertyType>> propertyExpression) where TObservedType : INotifyPropertyChanged
        {
            var lambda = propertyExpression as LambdaExpression;
            MemberInfo info = GetMemberExpression(lambda).Member;
            return info.Name;
        }

        private MemberExpression GetMemberExpression(LambdaExpression lambda)
        {
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else memberExpression = lambda.Body as MemberExpression;
            return memberExpression;
        }

        #region ICommand members
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                ControlEvent.Add(new WeakReference(value));
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
                ControlEvent.Remove(ControlEvent.Find(r => ((EventHandler)r.Target) == value));
            }
        }

        public bool CanExecute(object parameter)
        {
            return (CanExecuteDelegate?.Invoke(parameter)).GetValueOrDefault();
        }

        void ICommand.Execute(object parameter)
        {
            Execute?.Invoke(parameter);
        }
        #endregion
    }
}
