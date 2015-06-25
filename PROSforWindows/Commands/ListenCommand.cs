using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Threading;

namespace PROSforWindows.Commands
{
    public class ListenCommand : ICommand
    {
        public Predicate<object> CanExecuteDelegate { get; set; }

        private List<INotifyPropertyChanged> propertiesToListenTo;
        private List<WeakReference> ControlEvent;

        private Dispatcher Dispatcher { get; set; } = Dispatcher.CurrentDispatcher;

        public ListenCommand()
        {
            ControlEvent = new List<WeakReference>();
        }

        public ListenCommand(Dispatcher dispatch) : this()
        {
            Dispatcher = dispatch;
        }

        public List<INotifyPropertyChanged> PropertiesToListenTo
        {
            get { return propertiesToListenTo; }
            set { propertiesToListenTo = value; }
        }

        // The action to execute on command fire
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
                    Dispatcher.Invoke((EventHandler)a?.Target, null, EventArgs.Empty);
                    
                    //((EventHandler)a?.Target).Invoke(null, EventArgs.Empty);
                });
        }

        /// <summary>
        /// Any time the specified property is changed, the command will reevaluate whether or not it can execute.
        /// </summary>
        /// <typeparam name="TObservedType">A type that implements the INotifyPropertyChanged interface</typeparam>
        /// <param name="viewModel">The object to listen to, usually <code>this</code></param>
        /// <param name="propertyExpression">A lambda expression signifying a property to be listened to, i.e. <code>e => e.Property</code></param>
        public ListenCommand ListenOn<TObservedType, TPropertyType>(TObservedType viewModel, Expression<Func<TObservedType, TPropertyType>> propertyExpression) where TObservedType : INotifyPropertyChanged
        {
            // Determine the name of the variable requested to be listened
            string propertyName = GetPropertyName(propertyExpression);
            viewModel.PropertyChanged += (sender, e) =>
            {
                // When a property is changed on the viewModel, determine if propertyName of the changed property is the property name we're listening for. If so, we have a change
                if (e.PropertyName == propertyName) RaiseCanExecuteChanged();
            };
            return this;
        }

        /// <summary>
        /// Any time a property is changed on the viewModel, the command will reevalulate whether or not it can execute
        /// </summary>
        /// <typeparam name="TObservedType">A type that implements the INotifyPopretyChanged interface</typeparam>
        /// <param name="viewModel">The object to listen to, usually <code>this</code></param>
        public void ListenForNotificationFrom<TObservedType>(TObservedType viewModel) where TObservedType : INotifyPropertyChanged
        {
            viewModel.PropertyChanged += (sender, e) => RaiseCanExecuteChanged();
        }

        #region Getting property name from a lambda expression
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

        #endregion

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
