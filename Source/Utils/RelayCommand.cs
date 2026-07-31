using System.Windows.Input;

namespace Correctum.Utils
{
    public class RelayCommand(Action execute, Func<bool>? canExecute = null) : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value; 
            remove => CommandManager.RequerySuggested -= value; 
        }

        public bool CanExecute(object? parameter) => canExecute is null || canExecute();

        public void Execute(object? parameter) => execute();
    }
}
