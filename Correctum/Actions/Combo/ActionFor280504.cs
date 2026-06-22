using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Core.Enums;
using Correctum.Core.Models;
using Correctum.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace Correctum.Actions.Combo;

[ActionInfo("Для Тихомирова")]
public sealed class ActionFor280504 : IAction
{
    const string ScriptPath = @"C:\Users\Администратор\Documents\!!!!!!!!!!!!!!!!!!!!!!!Planfix\280504\all.json";

    public async Task DoAsync(DbSet<Receipt> receipts)
    {
        var items = new List<Item>();
        using (var stream = File.OpenRead(ScriptPath))
            await foreach (var raw in JsonSerializer.DeserializeAsyncEnumerable<RawItem>(stream))
                items.Add(Item.FromRaw(raw!));

        var existingReceipts = items.OfType<Item.ExistingReceipt>().Where(i => i is not Item.CancelReceipt).ToList();
        var receiptIdsToSave = (await receipts.ToListAsync())
            .Where(r => existingReceipts
            .Any(i => i.Drive == r.StorageId && i.DocNumber == (int)r.DocNumber!.Value))
            .Select(r => r.Id);
        receipts.RemoveRange(receipts.Where(r => !receiptIdsToSave.Contains(r.Id)));

        var changesPrice = existingReceipts.OfType<Item.ChangePrice>();
        foreach (var cp in changesPrice)
        {
            var receipt = receipts.FirstOrDefault(r => r.StorageId == cp.Drive && r.DocNumber == cp.DocNumber && !r.IsReadOnly)
                ?? throw new ArgumentException($"{cp.Drive} {cp.DocNumber}");
            if (receipt.DocNumber == 4320)
                Debug.Write("");
            var position = receipt.Items.FirstOrDefault(p => p.Name == cp.PositionName)
                ?? throw new ArgumentException($"{cp.Drive} {cp.DocNumber} {cp.PositionName}");
            if (receipt.TotalSum!.Value == Currency.Zero)
                throw new ArgumentException($" 0 {cp.Drive} {cp.DocNumber}");
            var (multipleCash, multipleNonCash, multipleAdvance, multipleCredit, multipleProvision) = (
                (receipt.CashSum ?? Currency.Zero) / receipt.TotalSum,
                (receipt.NonCashSum ?? Currency.Zero) / receipt.TotalSum,
                (receipt.AdvanceSum ?? Currency.Zero) / receipt.TotalSum,
                (receipt.CreditSum ?? Currency.Zero) / receipt.TotalSum,
                (receipt.ProvisionSum ?? Currency.Zero) / receipt.TotalSum);
            receipt.TotalSum = receipt.TotalSum - position.Cost;
            position.Price = cp.Price;
            position.Cost = position.Price * position.Quantity;
            receipt.TotalSum = receipt.TotalSum + position.Cost;
            (receipt.CashSum, receipt.NonCashSum, receipt.AdvanceSum, receipt.CreditSum, receipt.ProvisionSum) = (
                receipt.TotalSum * multipleCash,
                receipt.TotalSum * multipleNonCash,
                receipt.TotalSum * multipleAdvance,
                receipt.TotalSum * multipleCredit,
                receipt.TotalSum * multipleProvision);
        }

        var addsPosition = existingReceipts.OfType<Item.AddPosition>();
        foreach (var ap in addsPosition)
        {
            var receipt = receipts.FirstOrDefault(r => r.StorageId == ap.Drive && r.DocNumber == ap.DocNumber && !r.IsReadOnly)
                ?? throw new ArgumentException($"{ap.Drive} {ap.DocNumber}");
            if (receipt.DocNumber == 4320)
                Debug.Write("");
            var position = new Position
            {
                Name = ap.PositionName,
                Price = ap.Price,
                TaxRate = NDSRate.NoNDS,
                Type = PositionType.Service,
                Receipt = receipt,
                ReceiptId = receipt.Id,
                CalcMethod = CalculationMethod.FullPayment,
                Quantity = ap.Quantity,
                Unit = 0,
                Cost = ap.Price * ap.Quantity
            };
            if ((decimal)receipt.TotalSum!.Value == decimal.Zero)
                throw new ArgumentException($" 0 {ap.Drive} {ap.DocNumber}");
            var (multipleCash, multipleNonCash, multipleAdvance, multipleCredit, multipleProvision) = (
                (decimal)(receipt.CashSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value,
                (decimal)(receipt.NonCashSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value,
                (decimal)(receipt.AdvanceSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value,
                (decimal)(receipt.CreditSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value,
                (decimal)(receipt.ProvisionSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value);
            receipt.TotalSum += position.Cost;
            (receipt.CashSum, receipt.NonCashSum, receipt.AdvanceSum, receipt.CreditSum, receipt.ProvisionSum) = (
                (decimal)receipt.TotalSum!.Value * multipleCash,
                (decimal)receipt.TotalSum!.Value * multipleNonCash,
                (decimal)receipt.TotalSum!.Value * multipleAdvance,
                (decimal)receipt.TotalSum!.Value * multipleCredit,
                (decimal)receipt.TotalSum!.Value * multipleProvision);
            receipt.Items.Add(position);
        }

        var deletesPosition = existingReceipts.OfType<Item.RemovePosition>();
        foreach (var dp in deletesPosition)
        {
            var receipt = receipts.FirstOrDefault(r => r.StorageId == dp.Drive && r.DocNumber == dp.DocNumber && !r.IsReadOnly)
                ?? throw new ArgumentException($"{dp.Drive} {dp.DocNumber}");
            if (receipt.DocNumber == 4320)
                Debug.Write("");
            var position = receipt.Items.FirstOrDefault(p => p.Name == dp.PositionName)
                ?? throw new ArgumentException($"{dp.Drive} {dp.DocNumber} {dp.PositionName}");
            if ((decimal)receipt.TotalSum!.Value == decimal.Zero)
                throw new ArgumentException($" 0 {dp.Drive} {dp.DocNumber}");
            var (multipleCash, multipleNonCash, multipleAdvance, multipleCredit, multipleProvision) = (
                (decimal)(receipt.CashSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value,
                (decimal)(receipt.NonCashSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value,
                (decimal)(receipt.AdvanceSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value,
                (decimal)(receipt.CreditSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value,
                (decimal)(receipt.ProvisionSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value);
            receipt.TotalSum -= position.Cost;
            (receipt.CashSum, receipt.NonCashSum, receipt.AdvanceSum, receipt.CreditSum, receipt.ProvisionSum) = (
                (decimal)receipt.TotalSum!.Value * multipleCash,
                (decimal)receipt.TotalSum!.Value * multipleNonCash,
                (decimal)receipt.TotalSum!.Value * multipleAdvance,
                (decimal)receipt.TotalSum!.Value * multipleCredit,
                (decimal)receipt.TotalSum!.Value * multipleProvision);
            receipt.Items.Remove(position);
        }

        var toNonCash = existingReceipts.OfType<Item.ChangeCashToNonCash>();
        foreach (var tnc in toNonCash)
        {
            var receipt = receipts.FirstOrDefault(r => r.StorageId == tnc.Drive && r.DocNumber == tnc.DocNumber && !r.IsReadOnly) 
                ?? throw new ArgumentException($"{tnc.Drive} {tnc.DocNumber}");
            (receipt.CashSum, receipt.NonCashSum) = (Currency.Zero, (decimal)receipt.CashSum!.Value + (decimal)receipt.NonCashSum!.Value);
        }

        var toCash = existingReceipts.OfType<Item.ChangeNonCashToCash>();
        foreach (var tc in toCash)
        {
            var receipt = receipts.FirstOrDefault(r => r.StorageId == tc.Drive && r.DocNumber == tc.DocNumber && !r.IsReadOnly) 
                ?? throw new ArgumentException($"{tc.Drive} {tc.DocNumber}");
            (receipt.NonCashSum, receipt.CashSum) = (Currency.Zero, (decimal)receipt.CashSum!.Value + (decimal)receipt.NonCashSum!.Value);
        }


        /*var items = new List<Item>();

        using (var stream = File.OpenRead(ScriptPath))
            await foreach(var raw in JsonSerializer.DeserializeAsyncEnumerable<RawItem>(stream))
                items.Add(Item.FromRaw(raw!));

        var existingReceipts = items.OfType<Item.ExistingReceipt>().ToList();
        var receiptIdsToSave = (await receipts.ToListAsync()).Where(r => existingReceipts.Any(i => i.Drive == r.StorageId && i.DocNumber == (int)r.DocNumber!.Value)).Select(r => r.Id);
        receipts.RemoveRange(receipts.Where(r => !receiptIdsToSave.Contains(r.Id)));

        foreach (var receipt in receipts)
        {
            var currReceipt = receipt.Clone();
            currReceipt.IsReadOnly = true;
            currReceipt.Type = currReceipt.Type switch
            {
                ReceiptType.Income => ReceiptType.RefundIncome,
                ReceiptType.RefundIncome => ReceiptType.Income,
                ReceiptType.Outcome => ReceiptType.RefundOutcome,
                ReceiptType.RefundOutcome => ReceiptType.Outcome,
                ReceiptType type => throw new InvalidOperationException
                    ($"Неожиданное значение типа операции чека [{type}]"),
                _ => null
            };
            await receipts.AddAsync(currReceipt);
        }       

        var _receipts = await receipts.ToListAsync();                
        var receiptIdsToCancel = _receipts.Where(r => existingReceipts.OfType<Item.CancelReceipt>().Any(i => i.Drive == r.StorageId && i.DocNumber == (int)r.DocNumber!.Value)).Select(r => r.Id);
        receipts.RemoveRange(receipts.Where(r => receiptIdsToCancel.Contains(r.Id) && !r.IsReadOnly));

        var changesPrice = existingReceipts.OfType<Item.ChangePrice>();
        foreach(var cp in changesPrice)
        {
            var receipt = receipts.Where(r => r.StorageId == cp.Drive && r.DocNumber == cp.DocNumber && !r.IsReadOnly).FirstOrDefault() ?? throw new ArgumentException($"{cp.Drive} {cp.DocNumber}");
            var position = receipt.Items.Where(p => p.Name == cp.PositionName).FirstOrDefault() ?? throw new ArgumentException($"{cp.Drive} {cp.DocNumber} {cp.PositionName}");
            if((decimal)receipt.TotalSum!.Value == decimal.Zero) 
                throw new ArgumentException($" 0 {cp.Drive} {cp.DocNumber}");
            var (multipleCash, multipleNonCash, multipleAdvance, multipleCredit, multipleProvision) = (
                (decimal)(receipt.CashSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value,
                (decimal)(receipt.NonCashSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value,
                (decimal)(receipt.AdvanceSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value,
                (decimal)(receipt.CreditSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value,
                (decimal)(receipt.ProvisionSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value);
            receipt.TotalSum = (ulong)receipt.TotalSum!.Value - (ulong)position.Cost!.Value;
            position.Price = cp.Price;
            position.Cost = (decimal)position.Price * position.Quantity;
            receipt.TotalSum = (ulong)receipt.TotalSum!.Value + (ulong)position.Cost!.Value;
            (receipt.CashSum, receipt.NonCashSum, receipt.AdvanceSum, receipt.CreditSum, receipt.ProvisionSum) = (
                (decimal)receipt.TotalSum!.Value * multipleCash,
                (decimal)receipt.TotalSum!.Value * multipleNonCash,
                (decimal)receipt.TotalSum!.Value * multipleAdvance,
                (decimal)receipt.TotalSum!.Value * multipleCredit,
                (decimal)receipt.TotalSum!.Value * multipleProvision);
        }

        var addsPosition = existingReceipts.OfType<Item.AddPosition>();
        foreach (var ap in addsPosition)
        {
            var receipt = receipts.Where(r => r.StorageId == ap.Drive && r.DocNumber == ap.DocNumber && !r.IsReadOnly).FirstOrDefault() ?? throw new ArgumentException($"{ap.Drive} {ap.DocNumber}");
            var position = new Position
            {
                Name = ap.PositionName,
                Price = ap.Price,
                TaxRate = NDSRate.NoNDS,
                Type = PositionType.Service,
                Receipt = receipt,
                ReceiptId = receipt.Id,
                CalcMethod = CalculationMethod.FullPayment,
                Quantity = ap.Quantity,
                Unit = 0,
                Cost = ap.Price * ap.Quantity
            };
            if ((decimal)receipt.TotalSum!.Value == decimal.Zero)
                throw new ArgumentException($" 0 {ap.Drive} {ap.DocNumber}");
            var (multipleCash, multipleNonCash, multipleAdvance, multipleCredit, multipleProvision) = (
                (decimal)(receipt.CashSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value,
                (decimal)(receipt.NonCashSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value,
                (decimal)(receipt.AdvanceSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value,
                (decimal)(receipt.CreditSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value,
                (decimal)(receipt.ProvisionSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value);
            receipt.TotalSum = (ulong)receipt.TotalSum!.Value - (ulong)position.Cost!.Value;
            (receipt.CashSum, receipt.NonCashSum, receipt.AdvanceSum, receipt.CreditSum, receipt.ProvisionSum) = (
                (decimal)receipt.TotalSum!.Value * multipleCash,
                (decimal)receipt.TotalSum!.Value * multipleNonCash,
                (decimal)receipt.TotalSum!.Value * multipleAdvance,
                (decimal)receipt.TotalSum!.Value * multipleCredit,
                (decimal)receipt.TotalSum!.Value * multipleProvision);
            receipt.Items.Add(position);
        }

        var deletesPosition = existingReceipts.OfType<Item.RemovePosition>();
        foreach(var dp in deletesPosition)
        {
            var receipt = receipts.Where(r => r.StorageId == dp.Drive && r.DocNumber == dp.DocNumber && !r.IsReadOnly).FirstOrDefault() ?? throw new ArgumentException($"{dp.Drive} {dp.DocNumber}");
            var position = receipt.Items.Where(p => p.Name == dp.PositionName).FirstOrDefault() ?? throw new ArgumentException($"{dp.Drive} {dp.DocNumber} {dp.PositionName}");
            if ((decimal)receipt.TotalSum!.Value == decimal.Zero) 
                throw new ArgumentException($" 0 {dp.Drive} {dp.DocNumber}");
            var (multipleCash, multipleNonCash, multipleAdvance, multipleCredit, multipleProvision) = (
                (decimal)(receipt.CashSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value,
                (decimal)(receipt.NonCashSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value,
                (decimal)(receipt.AdvanceSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value,
                (decimal)(receipt.CreditSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value,
                (decimal)(receipt.ProvisionSum ?? Currency.Zero) / (decimal)receipt.TotalSum!.Value);
            receipt.TotalSum = (ulong)receipt.TotalSum!.Value - (ulong)position.Cost!.Value;
            (receipt.CashSum, receipt.NonCashSum, receipt.AdvanceSum, receipt.CreditSum, receipt.ProvisionSum) = (
                (decimal)receipt.TotalSum!.Value * multipleCash,
                (decimal)receipt.TotalSum!.Value * multipleNonCash,
                (decimal)receipt.TotalSum!.Value * multipleAdvance,
                (decimal)receipt.TotalSum!.Value * multipleCredit,
                (decimal)receipt.TotalSum!.Value * multipleProvision);
            receipt.Items.Remove(position);
        }

        var toNonCash = existingReceipts.OfType<Item.ChangeCashToNonCash>();
        foreach (var tnc in toNonCash)
        {
            var receipt = receipts.Where(r => r.StorageId == tnc.Drive && r.DocNumber == tnc.DocNumber && !r.IsReadOnly).FirstOrDefault() ?? throw new ArgumentException($"{tnc.Drive} {tnc.DocNumber}");
            (receipt.CashSum, receipt.NonCashSum) = (Currency.Zero, (ulong)receipt.CashSum!.Value + (ulong)receipt.NonCashSum!.Value);
        }

        var toCash = existingReceipts.OfType<Item.ChangeNonCashToCash>();
        foreach (var tc in toCash)
        {
            var receipt = receipts.Where(r => r.StorageId == tc.Drive && r.DocNumber == tc.DocNumber && !r.IsReadOnly).FirstOrDefault() ?? throw new ArgumentException($"{tc.Drive} {tc.DocNumber}");
            (receipt.NonCashSum, receipt.CashSum) = (Currency.Zero, (ulong)receipt.CashSum!.Value + (ulong)receipt.NonCashSum!.Value);
        }

        //foreach(var (StorageId, DocNumber) in receipts.ToList().Select(r => (r.StorageId, r.DocNumber)).Distinct())
        //{
        //    var rs = receipts.Where(r => r.StorageId == StorageId && r.DocNumber == DocNumber);
        //    var cancel = rs.FirstOrDefault(r => r.IsReadOnly) ?? throw new ArgumentException($"cancel {StorageId} {DocNumber}");
        //    var corr = rs.FirstOrDefault(r => !r.IsReadOnly) ?? throw new ArgumentException($"corr {StorageId} {DocNumber}");
        //    if (cancel.CashSum == corr.CashSum && cancel.NonCashSum == corr.NonCashSum && 
        //        cancel.TotalSum == corr.TotalSum && cancel.Items.Count == corr.Items.Count && 
        //        corr.Items.Select(i => i.Name).SequenceEqual(cancel.Items.Select(i => i.Name)) &&
        //        corr.Items.Select(i => i.Price).SequenceEqual(cancel.Items.Select(i => i.Price)) &&
        //        corr.Items.Select(i => i.Quantity).SequenceEqual(cancel.Items.Select(i => i.Quantity)) &&
        //        corr.Items.Select(i => i.Cost).SequenceEqual(cancel.Items.Select(i => i.Cost))) 
        //        receipts.RemoveRange(cancel, corr);
        //}

        var addReceipts = items.OfType<Item.NewReceipt>().GroupBy(r => r.Key).ToImmutableDictionary(r => r.Key, r => r.ToImmutableList());
        foreach(var ar in addReceipts)
        {
            var receipt = new Receipt
            {
                Type = ReceiptType.Income,
                Items = [],
                Created = ar.Value[0].Date.ToDateTime(TimeOnly.MinValue),
                
            };
            foreach(var p in ar.Value) receipt.Items.Add(new Position
            {
                Name = p.PositionName,
                Price = p.Price,
                TaxRate = NDSRate.NoNDS,
                Type = PositionType.Service,
                Receipt = receipt,
                CalcMethod = CalculationMethod.FullPayment,
                Quantity = p.Quantity,
                Unit = 0,
                Cost = p.Price * p.Quantity
            });
            receipt.NonCashSum = receipt.TotalSum = receipt.Items.Sum(p => (decimal)p.Cost!.Value);
            await receipts.AddAsync(receipt);*/
    }
    //}

    public class RawItem
    {
        public string Drive { get; init; }
        public int DocNumber { get; init; }
        public int ChangeType { get; init; }
        public string? Position { get; init; }
        public decimal? Quantity { get; init; }
        public decimal? Price { get; init; }
        public string? Date { get; init; }
    }

    public record class Item
    {
        public record class ExistingReceipt(string Drive, int DocNumber) : Item;
        public record class NewReceipt(Guid Key, string PositionName, decimal Quantity, decimal Price, DateOnly Date) : Item;

        public record class CancelReceipt(string Drive, int DocNumber) : ExistingReceipt(Drive, DocNumber);
        public record class RemovePosition(string Drive, int DocNumber, string PositionName) : ExistingReceipt(Drive, DocNumber);
        public record class ChangeCashToNonCash(string Drive, int DocNumber) : ExistingReceipt(Drive, DocNumber);
        public record class ChangeNonCashToCash(string Drive, int DocNumber) : ExistingReceipt(Drive, DocNumber);
        public record class AddPosition(string Drive, int DocNumber, string PositionName, decimal Quantity, decimal Price) : ExistingReceipt(Drive, DocNumber);
        public record class ChangePrice(string Drive, int DocNumber, string PositionName, decimal Price) : ExistingReceipt(Drive, DocNumber);

        public static Item FromRaw(RawItem raw) => raw.ChangeType switch
        {
            0 => new CancelReceipt(raw.Drive, raw.DocNumber),
            1 => new RemovePosition(raw.Drive, raw.DocNumber, 
                raw.Position ?? throw new ArgumentException($"{nameof(RemovePosition)} - {nameof(RemovePosition.PositionName)} ({raw.Drive} {raw.DocNumber})")),
            2 => new ChangeCashToNonCash(raw.Drive, raw.DocNumber),
            3 => new ChangeNonCashToCash(raw.Drive, raw.DocNumber),
            4 => new AddPosition(raw.Drive, raw.DocNumber, 
                raw.Position ?? throw new ArgumentException($"{nameof(AddPosition)} - {nameof(AddPosition.PositionName)} ({raw.Drive} {raw.DocNumber})"), 
                raw.Quantity ?? throw new ArgumentException($"{nameof(AddPosition)} - {nameof(AddPosition.Quantity)} ({raw.Drive} {raw.DocNumber})"), 
                raw.Price ?? throw new ArgumentException($"{nameof(AddPosition)} - {nameof(AddPosition.Price)} ({raw.Drive} {raw.DocNumber})")),
            5 => new ChangePrice(raw.Drive, raw.DocNumber, 
                raw.Position ?? throw new ArgumentException($"{nameof(ChangePrice)} - {nameof(ChangePrice.PositionName)} ({raw.Drive} {raw.DocNumber})"), 
                raw.Price ?? throw new ArgumentException($"{nameof(ChangePrice)} - {nameof(ChangePrice.Price)} ({raw.Drive} {raw.DocNumber})")),
            6 => new NewReceipt(Guid.Parse(raw.Drive), 
                raw.Position ?? throw new ArgumentException($"{nameof(NewReceipt)} - {nameof(NewReceipt.PositionName)} ({raw.Drive} {raw.DocNumber})"), 
                raw.Quantity ?? throw new ArgumentException($"{nameof(NewReceipt)} - {nameof(NewReceipt.Quantity)} ({raw.Drive} {raw.DocNumber})"), 
                raw.Price ?? throw new ArgumentException($"{nameof(NewReceipt)} - {nameof(NewReceipt.Price)} ({raw.Drive} {raw.DocNumber})"), 
                DateOnly.Parse(raw.Date ?? throw new ArgumentException($"{nameof(NewReceipt)} - {nameof(NewReceipt.Date)} ({raw.Drive} {raw.DocNumber})"))),
            _ => throw new InvalidOperationException($"Unknown change type: {raw.ChangeType}")
        };
    }
}
