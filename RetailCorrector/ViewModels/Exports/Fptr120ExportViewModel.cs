using RetailCorrector.Data;
using System.Globalization;
using System.IO;

namespace RetailCorrector.ViewModels.Exports;

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
            await CorrectionDataAsync(writer, receipt.Created);
            await writer.WriteLineAsync($"Fptr.setParam(Fptr.LIBFPTR_PARAM_RECEIPT_TYPE, Fptr.{receipt.Type switch
            {
                Enums.Operation.Income => "LIBFPTR_RT_SELL_CORRECTION",
                Enums.Operation.RefundIncome => "LIBFPTR_RT_SELL_RETURN_CORRECTION",
                Enums.Operation.Outcome => "LIBFPTR_RT_BUY_CORRECTION",
                Enums.Operation.RefundOutcome => "LIBFPTR_RT_BUY_RETURN_CORRECTION",
                _ => throw new FormatException("Найден чек с некорректным типом операции.")
            }});");
            await writer.WriteLineAsync($"Fptr.setParam(1055, {(int)receipt.TaxationSystem});");
            await ElectronicallyDocumentAsync(writer);
            await SetCorrectionDataAsync(writer, receipt.DocSign);
            await writer.WriteLineAsync("Fptr.openReceipt();");

            foreach (var pos in receipt.Items)
                await PositionAsync(writer, pos);

            await TotalSumAsync(writer, receipt.TotalSum);
            await PaymentAsync(writer, receipt.Payment);
            await writer.WriteLineAsync("Fptr.closeReceipt();");
            await writer.WriteLineAsync();
        }
    }

    private static async Task SetCorrectionDataAsync(StreamWriter writer, uint sign)
    {
        await writer.WriteLineAsync("Fptr.setParam(1173, 0);");
        await writer.WriteLineAsync("Fptr.setParam(1174, correctionInfo);");
        if (sign > 0) await writer.WriteLineAsync($"Fptr.setParam(1192, \"{sign}\");");
    }

    private static async Task ElectronicallyDocumentAsync(StreamWriter writer)
    {
        await writer.WriteLineAsync("Fptr.setParam(Fptr.LIBFPTR_PARAM_RECEIPT_ELECTRONICALLY, true);");
        await writer.WriteLineAsync("Fptr.setParam(1117, \"none\");");
        await writer.WriteLineAsync("Fptr.setParam(1008, \"none\");");
    }

    private static async Task CorrectionDataAsync(StreamWriter writer, DateTime dt)
    {
        await writer.WriteLineAsync($"Fptr.setParam(1178, new Date(\"{dt:yyyy'-'MM'-'dd'T'HH':'mm':'ss}\"));");
        await writer.WriteLineAsync($"Fptr.setParam(1179, \" \");");
        await writer.WriteLineAsync("Fptr.utilFormTlv();");
        await writer.WriteLineAsync("correctionInfo = Fptr.getParamByteArray(Fptr.LIBFPTR_PARAM_TAG_VALUE);");
    }

    private static async Task PositionAsync(StreamWriter writer, Position info)
    {
        if (info.Unit is null || info.Type == 0 || info.PayMethod == 0 || info.TaxRate == 0 || string.IsNullOrWhiteSpace(info.Name))
            throw new FormatException("Найдены чеки с некорректными позициями.");

        await writer.WriteLineAsync($"Fptr.setParam(Fptr.LIBFPTR_PARAM_COMMODITY_NAME, \"{info.Name.Replace("\"", "\\\"")}\");");
        await writer.WriteLineAsync($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PRICE, {RestoreDecimalAndToString(info.Price, 2)});");
        await writer.WriteLineAsync($"Fptr.setParam(Fptr.LIBFPTR_PARAM_QUANTITY, {RestoreDecimalAndToString(info.Quantity, 3)});");
        await writer.WriteLineAsync($"Fptr.setParam(Fptr.LIBFPTR_PARAM_POSITION_SUM, {RestoreDecimalAndToString(info.Cost, 2)});");
        await writer.WriteLineAsync($"Fptr.setParam(Fptr.LIBFPTR_PARAM_TAX_TYPE, {(int)info.TaxRate});");
        await writer.WriteLineAsync($"Fptr.setParam(2108, {(int)info.Unit});");
        await writer.WriteLineAsync($"Fptr.setParam(1212, {(int)info.Type});");
        await writer.WriteLineAsync($"Fptr.setParam(1214, {(int)info.PayMethod});");
        await writer.WriteLineAsync("Fptr.registration();");
    }

    private static async Task TotalSumAsync(StreamWriter writer, ulong sum)
    {
        await writer.WriteLineAsync($"Fptr.setParam(Fptr.LIBFPTR_PARAM_SUM, {RestoreDecimalAndToString(sum, 2)});");
        await writer.WriteLineAsync("Fptr.receiptTotal();");
    }

    private static async Task PaymentAsync(StreamWriter writer, Payment info)
    {
        if (info.Cash > 0) await PaymentAsync(writer, "LIBFPTR_PT_CASH", info.Cash);
        if (info.ECash > 0) await PaymentAsync(writer, "LIBFPTR_PT_ELECTRONICALLY", info.ECash);
        if (info.Pre > 0) await PaymentAsync(writer, "LIBFPTR_PT_PREPAID", info.Pre);
        if (info.Post > 0) await PaymentAsync(writer, "LIBFPTR_PT_CREDIT", info.Post);
        if (info.Provision > 0) await PaymentAsync(writer, "LIBFPTR_PT_OTHER", info.Provision);
    }

    private static async Task PaymentAsync(StreamWriter writer, string type, ulong sum)
    {
        await writer.WriteLineAsync($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.{type});");
        await writer.WriteLineAsync($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {RestoreDecimalAndToString(sum, 2)});");
        await writer.WriteLineAsync("Fptr.payment();");
    }

    private static string RestoreDecimalAndToString(ulong optimized, int power) =>
        Math.Round(optimized / Math.Pow(10.0, power), power).ToString(CultureInfo.InvariantCulture);
}
