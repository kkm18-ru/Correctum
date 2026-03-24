using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MsBox.Avalonia.Enums;
using Correctum.ViewModels.Importers;

namespace Correctum.ViewModels.Windows;

public sealed partial class ImportWindowViewModel : ObservableObject
{
    public RangeProgress<long> Progress { get; }
    public ImporterViewModel? Importer { get; }

    [ObservableProperty] private long m_currentProgress = 0;
    [ObservableProperty] private long m_maximumProgress = 0;
    [ObservableProperty] private bool m_running = false;

    private CancellationTokenSource? cts;

    public event Action? Close;

    public ImportWindowViewModel(ImporterViewModel importer)
    {
        Importer = importer;
        void SetCurrent(long value) => CurrentProgress = value;
        void SetMaximum(long value) => MaximumProgress = value;
        Progress = new(SetCurrent, SetMaximum);
    }

    public async void Start(object? param)
    {
        cts = new CancellationTokenSource();
        Running = true;
        try{
            //await foreach (var receipt in Importer.Import(progress, cts.Token))
            //    Workspace.Instance.Receipts.Add(receipt);
            await Dispatcher.UIThread.InvokeAsync(() => Close?.Invoke());
            //ReceiptPanel.UpdateReceiptList();
        }
        catch (Exception ex)
        {
            //var lastParse = Workspace.Instance.ImportHistory[^1];
            //for (var i = Workspace.Instance.Receipts.Count - 1; i >= 0; i--)
            //{
            //    if (Workspace.Instance.Receipts[i].ParseId == lastParse.Id)
            //        Workspace.Instance.Receipts.RemoveAt(i);
            //}
            //Workspace.Instance.ImportHistory.Remove(lastParse);
            Notification notification;
            if (ex is (OperationCanceledException or TaskCanceledException))
                notification = new Notification("", "Выгрузка чеков была отменена пользователем", Icon.Stop);
            else
                notification = new Notification(ex.Message, "Ошибка выгрузки чеков", Icon.Error);
            WeakReferenceMessenger.Default.Send(notification);
        }
        finally
        {
            Running = false;
            cts.Dispose();
        }
    }

    public void Cancel() {
        if (cts is null) return;
        cts.Cancel();
        cts.Dispose();
        Running = false; 
    }
}
