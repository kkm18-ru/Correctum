using Correctum.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Xaml.Behaviors.Core;
using System.Windows;
using System.Windows.Input;

namespace Correctum.ViewModels.Windows;

public sealed class ActionWindowViewModel : BindableBase
{
    public string ActionName { get; }
    public IAction Action { get; }
    public ICommand Command { get; }
    public bool IsBusy { get; set => SetField(ref field, value); }

    public event Action? Done;

    internal ActionWindowViewModel(string actionName, IAction action)
    {
        ActionName = actionName;
        Action = action;
        Command = new ActionCommand(Do);
    }

#if DEBUG
    public ActionWindowViewModel() : this("Тестовое действие", null!) { }
#endif

    private async void Do()
    {
        try
        {
            IsBusy = true;
            StorageContext.CreateBackup();
            using (var storage = new StorageContext())
            {
                await storage.Positions.LoadAsync();
                await Action.DoAsync(storage.Receipts);
                await storage.SaveChangesAsync();
                await storage.Database.ExecuteSqlRawAsync("VACUUM");
            }
            IsBusy = false;
            Done?.Invoke();
        }
        catch(Exception e)
        {
            MessageBox.Show(e.ToString(),"", 0, MessageBoxImage.Error);
        }
    }
}
