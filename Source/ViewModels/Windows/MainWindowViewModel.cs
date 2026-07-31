using Correctum.Abstractions;
using Correctum.Core.Models;
using Correctum.Utils;
using Correctum.Views.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Xaml.Behaviors.Core;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Correctum.ViewModels.Windows;

public sealed class MainWindowViewModel : BindableBase
{
    private const int MaxSizePage = 200;

    public ObservableCollection<Receipt> ReceiptPage { get; } = [];
    public ICommand RefreshCommand { get; }
    public ICommand GoToFirstPageCommand { get; }
    public ICommand GoToPreviousPageCommand { get; }
    public ICommand GoToNextPageCommand { get; }
    public ICommand GoToLastPageCommand { get; }
    public ICommand PasteRawReceiptCommand { get; } = new ActionCommand(() =>
    {
        if (!Clipboard.ContainsText()) return;
        new PasteWindow(Clipboard.GetText()).Show();
    });

    public int PageNumber
    {
        get; set
        {
            if (value < 1 || GetLastPageNumber() < value)
            {
                OnPropertyChanged();
                return;
            }
            if (SetField(ref field, value)) 
                UpdatePanel();
        }
    } = 1;

    public MainWindowViewModel()
    {
        RefreshCommand = new ActionCommand(UpdatePanel);
        GoToFirstPageCommand = new RelayCommand(() => PageNumber = 1, () => PageNumber > 1);
        GoToPreviousPageCommand = new RelayCommand(() => PageNumber--, () => PageNumber > 1);
        GoToNextPageCommand = new RelayCommand(() => PageNumber++, () => PageNumber < GetLastPageNumber());
        GoToLastPageCommand = new RelayCommand(() => PageNumber = GetLastPageNumber(), () => PageNumber < GetLastPageNumber());
    }

    public void UpdatePanel()
    {
        using var ctx = new StorageContext();
        ctx.Positions.Load();
        ReceiptPage.Clear();
        foreach (var receipt in ctx.Receipts.Skip(MaxSizePage * (PageNumber - 1)).Take(MaxSizePage))
            ReceiptPage.Add(receipt);
    }

    private static int GetLastPageNumber()
    {
        using var ctx = new StorageContext();
        return (int)Math.Ceiling(ctx.Receipts.Count() / (double)MaxSizePage);
    }
}
