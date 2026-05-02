using System.Windows.Input;

namespace RetailCorrector.Utils
{
    public class RelayCommand<T>(Action<T> execute, Predicate<T>? canExecute = null) : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value; 
            remove => CommandManager.RequerySuggested -= value; 
        }

        public bool CanExecute(object? parameter) =>
            canExecute == null || parameter is T param && canExecute(param);

        public void Execute(object? parameter) => execute((T)parameter!);
    }
}
