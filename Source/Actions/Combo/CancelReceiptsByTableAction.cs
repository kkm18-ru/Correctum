using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Core.Enums;
using Correctum.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Microsoft.Xaml.Behaviors.Core;
using Polly;
using System.IO;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Correctum.Actions.Combo;

[ActionInfo("Аннулировать согласно таблице")]
[Button("Выбрать таблицу", nameof(ShowDialogCommand))]
internal class CancelReceiptsByTableAction : IAction
{
    private string tablePath;
    public ICommand ShowDialogCommand { get; }

    public CancelReceiptsByTableAction()
    {
        ShowDialogCommand = new ActionCommand(ShowDialog);
    }

    private void ShowDialog()
    {
        var dialog = new OpenFileDialog()
        {
            Filter = "Таблица чеков|*.csv",
            Multiselect = false,
            AddToRecent = true
        };
        if (dialog.ShowDialog() ?? false)
            tablePath = dialog.FileName;
    }

    public async Task DoAsync(DbSet<Receipt> receipts)
    {
        List<(string, uint)> arr = [];
        using var fs = File.OpenText(tablePath);
        while((await fs.ReadLineAsync()) is string line)
        {
            var elem = line.Split(';');
            arr.Add((elem[0], uint.Parse(elem[1])));
        }
        foreach(var r in receipts)
        {
            if (arr.Contains((r.StorageId ?? string.Empty, r.DocNumber ?? 0)))
                r.Type = r.Type switch
                {
                    ReceiptType.RefundOutcome => ReceiptType.Outcome,
                    ReceiptType.Outcome => ReceiptType.RefundOutcome,
                    ReceiptType.RefundIncome => ReceiptType.Income,
                    ReceiptType.Income => ReceiptType.RefundIncome,
                    _ => throw new InvalidOperationException("Неизвестный тип чека")
                };
            else receipts.Remove(r);
        }
    }
}
