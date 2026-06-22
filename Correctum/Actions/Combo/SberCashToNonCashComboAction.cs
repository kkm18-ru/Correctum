using ClosedXML.Excel;
using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Core.Enums;
using Correctum.Core.Models;
using Correctum.Core.ValueObjects;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using AcquiringData = (System.DateTime Date, Correctum.Core.Enums.ReceiptType Type, decimal Sum);  // todo add TerminalId

namespace Correctum.Actions.Combo;

[ActionInfo("СБЕР")]
[TextBox("Путь к Excel-таблице", nameof(FilePath))]
[TextBox("Фильтр по терминалу", nameof(FilterTerminalId))]
public sealed class SberCashToNonCashComboAction : IAction
{
    public string FilePath { get; set; } = @"C:\Users\Администратор\Documents\!!!!!!!!!!!!!!!!!!!!!!!Planfix\278661\Отчет Эквайринга.xlsx";
    public string FilterTerminalId { get; set; } = "20431885";

    public async Task DoAsync(DbSet<Receipt> receipts)
    {
        var acquiring = ParseDataFromAcquiring();
        var pairs = GenerateReceiptAcquiringPairs(receipts, acquiring);
        var receiptIds = pairs.Keys.ToArray();
        receipts.ToList().Where(r => !receiptIds.Contains(r.Id)).ForEach(r => receipts.Remove(r));
        receipts.ToList().Where(r => r.NonCashSum == acquiring[pairs[r.Id]].Sum).ForEach(r => receipts.Remove(r));
        await new CancelReceiptCloner().DoAsync(receipts);
        foreach (var r in receipts)
        {
            r.CashSum = Currency.Zero;
            r.NonCashSum = (Currency)acquiring[pairs[r.Id]].Sum;
        }
        //await receipts.Where(r => !receiptIds.Contains(r.Id)).ExecuteDeleteAsync();
        /*
        
        */


        /*var sums = new Dictionary<int, Currency>();
        foreach (var r in receipts)
            sums.Add(r.Id, (Currency)acquiring[pairs[r.Id]].Sum);
        await receipts
            .Where(r => !r.IsReadOnly)
            .ExecuteUpdateAsync(b =>
            {
                b.SetProperty(r => r.CashSum, Currency.Zero);
                b.SetProperty(r => r.NonCashSum, r => sums[r.Id]);
            });*/
    }

    private Dictionary<Guid, AcquiringData> ParseDataFromAcquiring()
    {
        var (total, skipped) = (0, 0);
        using var workbook = new XLWorkbook(FilePath);
        var sheet = workbook.Worksheets.First();
        var data = new Dictionary<Guid, AcquiringData>();
        for (var i = 2; i < int.MaxValue; i++)
        {
            var terminalId = sheet.Cell($"C{i}").Value.ToString();
            if (string.IsNullOrEmpty(terminalId)) break;
            Interlocked.Increment(ref total);
            if (!string.IsNullOrWhiteSpace(FilterTerminalId) &&
                !terminalId.SequenceEqual(FilterTerminalId))
            {
                Interlocked.Increment(ref skipped);
                continue;
            }
            var date = sheet.Cell($"D{i}").Value.GetDateTime();
            var type = sheet.Cell($"E{i}").Value.ToString();
            var sum = sheet.Cell($"F{i}").GetValue<decimal>();
            var receiptType = type switch
            {
                "Покупка" => ReceiptType.Income,
                _ => throw new InvalidOperationException()
            };
            data.Add(Guid.CreateVersion7(), (date, receiptType, sum));
        }
        Debug.WriteLine($"Всего строк: {total}, пропущено: {skipped}");
        return data;
    }

    private static Dictionary<int, Guid> GenerateReceiptAcquiringPairs
        (DbSet<Receipt> receipts, Dictionary<Guid, AcquiringData> acquiring)
    {
        var pairs = new Dictionary<int, Guid>();
        foreach (var item in acquiring)
        {
            var receipt = receipts
                .AsEnumerable()
                .FirstOrDefault(r => 
                    r.Created != null &&
                    r.Created.Value.Date == item.Value.Date.Date &&
                    !pairs.ContainsKey(r.Id) &&
                    Math.Abs((r.Created.Value - item.Value.Date).TotalMinutes) <= 5 &&
                    r.TotalSum == item.Value.Sum &&
                    r.Type == item.Value.Type);
            if (receipt is null)
            {
                // todo handle
                continue;
            }
            pairs.Add(receipt.Id, item.Key);
        }
        return pairs;
    }
}
