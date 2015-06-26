using System;
using System.Windows.Input;

namespace PROSforWindows.Commands
{
    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action<object> execute)
            : this(execute, DefaultCanExecute)
        { }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
            : base(execute, canExecute)
        { }
    }

    public class RelayCommand<T> : ICommand
    {
        protected Action<T> execute;
        protected Predicate<T> canExecute;
        private event EventHandler CanExecuteChangedInternal;

        public RelayCommand(Action<T> execute)
            : this(execute, DefaultCanExecute)
        { }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));
            if (canExecute == null) throw new ArgumentNullException(nameof(canExecute));

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                CanExecuteChangedInternal += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
                CanExecuteChangedInternal -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            if (parameter != null && !(parameter is T)) throw new ArgumentException("Parameter is not of type " + typeof(T).Name);
            return (canExecute?.Invoke((T)parameter)).GetValueOrDefault(false);
        }

        public void Execute(object parameter)
        {
            if (parameter != null && !(parameter is T)) throw new ArgumentException("Parameter is not of type " + typeof(T).Name);
            execute((T)parameter);
        }

        public void OnCanExecuteChanged()
        {
            EventHandler h = CanExecuteChangedInternal;
            h?.Invoke(this, EventArgs.Empty);
        }

        public void Destroy()
        {
            canExecute = _ => false;
            execute = _ => { return; };
        }

        protected static bool DefaultCanExecute(T parameter)
        {
            return true;
        }
    }
}
