using Correctum.Core.Models;
using Correctum.Core.Enums;
using Correctum.Core.ValueObjects;
using System.Globalization;
using System.IO;

namespace Correctum.ViewModels.Exports;

public sealed class Fptr120ExportViewModel : FileExportViewModel
{
    public override string ExportName => "Скрипт драйвера АТОЛ (ФФД 1.2)";
    public override string Filter => "Duktape-скрипт|*.js";

    public override async ValueTask Export(string filepath, IEnumerable<Receipt> receipts)
    {
        await using var stream = File.Open(filepath, FileMode.Create, FileAccess.Write, FileShare.Read);
        using var writer = new StreamWriter(stream);
        foreach (var receipt in receipts)
        {
            await CorrectionDataAsync(writer, receipt.Created.Value);
            await SetReceiptTypeAsync(writer, receipt.Type.Value);            
            await writer.WriteLineAsync($"Fptr.setParam(1055, {(int)receipt.TaxationSystem});");
            await ElectronicallyDocumentAsync(writer);
            await SetCorrectionDataAsync(writer, receipt.DocSign);
            await writer.WriteLineAsync("Fptr.openReceipt();");
            await PositionsAsync(writer, receipt.Items);
            await TotalSumAsync(writer, receipt.TotalSum.Value);
            await PaymentAsync(writer, receipt);
            await writer.WriteLineAsync("Fptr.closeReceipt();");
            await writer.WriteLineAsync();
        }
    }
    internal static async Task SetReceiptTypeAsync(StreamWriter writer, ReceiptType receiptType) =>
        await writer.WriteLineAsync($"Fptr.setParam(Fptr.LIBFPTR_PARAM_RECEIPT_TYPE, Fptr.{ToAtolConstant(receiptType)});");

    private static string ToAtolConstant(ReceiptType receiptType) => receiptType switch
    {
        ReceiptType.Income => "LIBFPTR_RT_SELL_CORRECTION",
        ReceiptType.RefundIncome => "LIBFPTR_RT_SELL_RETURN_CORRECTION",
        ReceiptType.Outcome => "LIBFPTR_RT_BUY_CORRECTION",
        ReceiptType.RefundOutcome => "LIBFPTR_RT_BUY_RETURN_CORRECTION",
        _ => throw new FormatException("Найден чек с некорректным типом операции.")
    };

    internal static async Task SetCorrectionDataAsync(StreamWriter writer, FiscalSign? sign)
    {
        await writer.WriteLineAsync("Fptr.setParam(1173, 0);");
        await writer.WriteLineAsync("Fptr.setParam(1174, correctionInfo);");
        if (sign is not null) await writer.WriteLineAsync($"Fptr.setParam(1192, \"{sign}\");");
    }

    internal static async Task ElectronicallyDocumentAsync(StreamWriter writer)
    {
        await writer.WriteLineAsync("Fptr.setParam(Fptr.LIBFPTR_PARAM_RECEIPT_ELECTRONICALLY, true);");
        await writer.WriteLineAsync("Fptr.setParam(1117, \"none\");");
        await writer.WriteLineAsync("Fptr.setParam(1008, \"none\");");
    }

    internal static async Task CorrectionDataAsync(StreamWriter writer, DateTime dt)
    {
        await writer.WriteLineAsync($"Fptr.setParam(1178, new Date(\"{dt:yyyy'-'MM'-'dd'T'HH':'mm':'ss}\"));");
        await writer.WriteLineAsync($"Fptr.setParam(1179, \" \");");
        await writer.WriteLineAsync("Fptr.utilFormTlv();");
        await writer.WriteLineAsync("correctionInfo = Fptr.getParamByteArray(Fptr.LIBFPTR_PARAM_TAG_VALUE);");
    }

    internal static async Task PositionsAsync(StreamWriter writer, Position[] array)
    {
        array ??= [];
        if (array.Length == 0)
            throw new FormatException("Кассовый чек должен содержать хотя бы одну позицию.");
        foreach (var pos in array) await PositionAsync(writer, pos);
    }

    internal static async Task PositionAsync(StreamWriter writer, Position info)
    {
        if (string.IsNullOrWhiteSpace(info.Name))
            throw new FormatException("Найдена позиция чека с некорректным наименованием.");
        if (!info.Unit.HasValue) throw new FormatException("Найдена позиция чека без единицы измерения.");
        if (Enum.GetName(info.Unit.Value) is null)
            throw new FormatException("Найдена позиция чека с некорректной единицей измерения.");
        if (!info.Type.HasValue) throw new FormatException("Найдена позиция чека без типа.");
        if (Enum.GetName(info.Type.Value) is null)
            throw new FormatException("Найдена позиция чека с некорректным типом.");
        if (!info.CalcMethod.HasValue) throw new FormatException("Найдена позиция чека без способа расчета.");
        if (Enum.GetName(info.CalcMethod.Value) is null)
            throw new FormatException("Найдена позиция чека с некорректным способом расчета.");
        if (!info.TaxRate.HasValue) throw new FormatException("Найдена позиция чека без ставки НДС.");
        if (Enum.GetName(info.TaxRate.Value) is null)
            throw new FormatException("Найдена позиция чека с некорректной ставкой НДС.");

        await writer.WriteLineAsync($"Fptr.setParam(Fptr.LIBFPTR_PARAM_COMMODITY_NAME, \"{info.Name.Replace("\"", "\\\"")}\");");
        await writer.WriteLineAsync($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PRICE, {info.Price});");
        await writer.WriteLineAsync($"Fptr.setParam(Fptr.LIBFPTR_PARAM_QUANTITY, {info.Quantity.Value.ToString(CultureInfo.InvariantCulture)});");
        await writer.WriteLineAsync($"Fptr.setParam(Fptr.LIBFPTR_PARAM_POSITION_SUM, {info.Cost});");
        await writer.WriteLineAsync($"Fptr.setParam(Fptr.LIBFPTR_PARAM_TAX_TYPE, {(int)info.TaxRate});");
        await writer.WriteLineAsync($"Fptr.setParam(2108, {(int)info.Unit});");
        await writer.WriteLineAsync($"Fptr.setParam(1212, {(int)info.Type});");
        await writer.WriteLineAsync($"Fptr.setParam(1214, {(int)info.CalcMethod});");
        await writer.WriteLineAsync("Fptr.registration();");
    }

    internal static async Task TotalSumAsync(StreamWriter writer, Currency sum)
    {
        await writer.WriteLineAsync($"Fptr.setParam(Fptr.LIBFPTR_PARAM_SUM, {sum});");
        await writer.WriteLineAsync("Fptr.receiptTotal();");
    }

    internal static async Task PaymentAsync(StreamWriter writer, Receipt info)
    {
        await PaymentAsync(writer, "LIBFPTR_PT_CASH", info.CashSum);
        await PaymentAsync(writer, "LIBFPTR_PT_ELECTRONICALLY", info.NonCashSum);
        await PaymentAsync(writer, "LIBFPTR_PT_PREPAID", info.AdvanceSum);
        await PaymentAsync(writer, "LIBFPTR_PT_CREDIT", info.CreditSum);
        await PaymentAsync(writer, "LIBFPTR_PT_OTHER", info.ProvisionSum);
    }

    private static async Task PaymentAsync(StreamWriter writer, string type, Currency? sum)
    {
        if (sum is null || (ulong)sum.Value == 0) return;
        await writer.WriteLineAsync($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.{type});");
        await writer.WriteLineAsync($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {sum});");
        await writer.WriteLineAsync("Fptr.payment();");
    }
}
