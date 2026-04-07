using System.Globalization;
using System.IO;

namespace RetailCorrector.ViewModels.Exports;

public sealed class Fptr120ExportViewModel : FileExportViewModel
{
    public override string ExportName => "Скрипт драйвера АТОЛ (ФФД 1.2)";
    public override string Filter => "Duktape-скрипт|*.js";

    protected override async ValueTask InternalExport(string filepath)
    {
        await using var stream = File.Open(filepath, FileMode.Create, FileAccess.Write, FileShare.Read);
        using var writer = new StreamWriter(stream);
        foreach (var receipt in Workspace.Instance.Receipts)
        {
            writer.WriteLine($"Fptr.setParam(1178, new Date(\"{receipt.Created:yyyy'-'MM'-'dd'T'HH':'mm':'ss}\"));");
            writer.WriteLine($"Fptr.setParam(1179, \" \");");
            writer.WriteLine("Fptr.utilFormTlv();");
            writer.WriteLine("correctionInfo = Fptr.getParamByteArray(Fptr.LIBFPTR_PARAM_TAG_VALUE);");
            writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_RECEIPT_TYPE, Fptr.{receipt.Type switch
            {
                Enums.Operation.Income => "LIBFPTR_RT_SELL_CORRECTION",
                Enums.Operation.RefundIncome => "LIBFPTR_RT_SELL_RETURN_CORRECTION",
                Enums.Operation.Outcome => "LIBFPTR_RT_BUY_CORRECTION",
                Enums.Operation.RefundOutcome => "LIBFPTR_RT_BUY_RETURN_CORRECTION",
                _ => throw new FormatException("Найден чек с некорректным типом операции.")
            }});");
            writer.WriteLine("Fptr.setParam(Fptr.LIBFPTR_PARAM_RECEIPT_ELECTRONICALLY, true);");
            writer.WriteLine($"Fptr.setParam(1055, {(int)receipt.TaxationSystem});");
            writer.WriteLine("Fptr.setParam(1117, \"none\");");
            writer.WriteLine("Fptr.setParam(1008, \"none\");");
            writer.WriteLine("Fptr.setParam(1173, 0);");
            writer.WriteLine("Fptr.setParam(1174, correctionInfo);");
            if (receipt.DocSign > 0)
                writer.WriteLine($"Fptr.setParam(1192, \"{receipt.DocSign}\");");
            writer.WriteLine("Fptr.openReceipt();");

            foreach (var pos in receipt.Items)
            {
                if (pos.Unit is null || pos.Type == 0 || pos.PayMethod == 0 || pos.TaxRate == 0 || string.IsNullOrWhiteSpace(pos.Name))
                    throw new FormatException("Найдены чеки с некорректными позициями.");

                writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_COMMODITY_NAME, \"{pos.Name.Replace("\"", "\\\"")}\");");
                writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PRICE, {Math.Round(pos.Price / 100.0, 2).ToString(CultureInfo.InvariantCulture)});");
                writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_QUANTITY, {Math.Round(pos.Quantity / 1000.0, 3).ToString(CultureInfo.InvariantCulture)});");
                writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_POSITION_SUM, {Math.Round(pos.Cost / 100.0, 2).ToString(CultureInfo.InvariantCulture)});");
                writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_TAX_TYPE, {(int)pos.TaxRate});");
                writer.WriteLine($"Fptr.setParam(2108, {(int)pos.Unit});");
                writer.WriteLine($"Fptr.setParam(1212, {(int)pos.Type});");
                writer.WriteLine($"Fptr.setParam(1214, {(int)pos.PayMethod});");
                writer.WriteLine("Fptr.registration();");
            }

            writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_SUM, {Math.Round(receipt.TotalSum / 100.0, 2).ToString(CultureInfo.InvariantCulture)});");
            writer.WriteLine("Fptr.receiptTotal();");
            if (receipt.Payment.Cash > 0)
            {
                writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_CASH);");
                writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(receipt.Payment.Cash / 100.0, 2).ToString(CultureInfo.InvariantCulture)});");
                writer.WriteLine("Fptr.payment();");
            }
            if (receipt.Payment.ECash > 0)
            {
                writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_ELECTRONICALLY);");
                writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(receipt.Payment.ECash / 100.0, 2).ToString(CultureInfo.InvariantCulture)});");
                writer.WriteLine("Fptr.payment();");
            }
            if (receipt.Payment.Pre > 0)
            {
                writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_PREPAID);");
                writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(receipt.Payment.Pre / 100.0, 2).ToString(CultureInfo.InvariantCulture)});");
                writer.WriteLine("Fptr.payment();");
            }
            if (receipt.Payment.Post > 0)
            {
                writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_CREDIT);");
                writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(receipt.Payment.Post / 100.0, 2).ToString(CultureInfo.InvariantCulture)});");
                writer.WriteLine("Fptr.payment();");
            }
            if (receipt.Payment.Provision > 0)
            {
                writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_OTHER);");
                writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(receipt.Payment.Provision / 100.0, 2).ToString(CultureInfo.InvariantCulture)});");
                writer.WriteLine("Fptr.payment();");
            }
            writer.WriteLine("Fptr.closeReceipt();");
            writer.WriteLine();
        }
    }
}
