using System.Windows.Input;

namespace RetailCorrector.Utils;

public class RelayCommand<T>(Action<T> execute, Predicate<T>? canExecute = null) : ICommand
{
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value; 
        remove => CommandManager.RequerySuggested -= value; 
    }

    public bool CanExecute(object? parameter)
    {
        if (canExecute is null) return true;
        if (parameter is T obj) canExecute(obj);
        return false;
    }

    public void Execute(object? parameter) => 
        execute((T)parameter!);
}
