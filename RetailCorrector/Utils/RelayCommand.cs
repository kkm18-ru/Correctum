using System.Windows.Input;

namespace RetailCorrector.Utils;

public class RelayCommand(Action execute, Func<bool>? canExecute = null) : ICommand
{
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter) =>
        canExecute == null || canExecute();

    public void Execute(object? parameter) => execute();
}
