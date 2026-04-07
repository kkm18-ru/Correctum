using Bogus.DataSets;
using RetailCorrector.Data;
using RetailCorrector.Enums;
using RetailCorrector.Utils;
using System.IO;
using Xceed.Wpf.Toolkit.Primitives;
using static Xceed.Wpf.Toolkit.Calculator;

namespace RetailCorrector.Exporters
{
    internal partial class Fptr : IExporter
    {
        public string Name => "Fptr-скрипт";
        public string? FileFilter => null;

        public static Fptr Instance => field ??= new Fptr();

        public async ValueTask Export(string? path)
        {
            await using var stream = File.Open($"{path}\\fptr.txt", FileMode.Create, FileAccess.Write, FileShare.Read);
            using var writer = new StreamWriter(stream);
            foreach(var receipt in Workspace.Instance.Receipts)
            {
                writer.WriteLine($"Fptr.setParam(1178, new Date(\"{receipt.Created:yyyy'-'MM'-'dd'T'HH':'mm':'ss}\"));");
                writer.WriteLine($"Fptr.setParam(1179, \" \");");
                writer.WriteLine("Fptr.utilFormTlv();");
                writer.WriteLine("correctionInfo = Fptr.getParamByteArray(Fptr.LIBFPTR_PARAM_TAG_VALUE);");
                var receiptType = receipt.Type switch
                {
                    Enums.Operation.Income => "LIBFPTR_RT_SELL_CORRECTION",
                    Enums.Operation.RefundIncome => "LIBFPTR_RT_SELL_RETURN_CORRECTION",
                    Enums.Operation.Outcome => "LIBFPTR_RT_BUY_CORRECTION",
                    Enums.Operation.RefundOutcome => "LIBFPTR_RT_BUY_RETURN_CORRECTION",
                };
                writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_RECEIPT_TYPE, Fptr.{receiptType});");
                writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_RECEIPT_ELECTRONICALLY, true);");
                writer.WriteLine($"Fptr.setParam(1055, {(int)receipt.TaxationSystem});");
                writer.WriteLine("Fptr.setParam(1117, none);");
                writer.WriteLine("Fptr.setParam(1008, none);");
                writer.WriteLine($"Fptr.setParam(1173, {CorrType});");
                writer.WriteLine("Fptr.setParam(1174, correctionInfo);");
                if(receipt.DocSign > 0) 
                    writer.WriteLine($"Fptr.setParam(1192, \"{receipt.DocSign}\");");
                writer.WriteLine("Fptr.openReceipt();");

                foreach(var pos in receipt.Items)
                {
                    writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_COMMODITY_NAME, \"{pos.Name.Replace("\"","\\\"")}\");");
                    writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PRICE, {Math.Round(pos.Price / 100.0, 2)});");
                    writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_QUANTITY, {Math.Round(pos.Quantity / 1000.0, 3)});");
                    writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_POSITION_SUM, {Math.Round(pos.Cost / 100.0, 2)});");
                    writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_TAX_TYPE, {(int)pos.TaxRate});");
                    writer.WriteLine($"Fptr.setParam(2108, {(int)pos.Unit});");
                    writer.WriteLine($"Fptr.setParam(1212, {(int)pos.Type});");
                    writer.WriteLine($"Fptr.setParam(1214, {(int)pos.PayMethod});");
                    writer.WriteLine("Fptr.registration();");
                }

                writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_SUM, {Math.Round(receipt.TotalSum / 100.0, 2)});");
                writer.WriteLine("Fptr.receiptTotal();");
                if (receipt.Payment.Cash > 0)
                {
                    writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_CASH);");
                    writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(receipt.Payment.Cash / 100.0, 2)});");
                    writer.WriteLine("Fptr.payment();");
                }
                if (receipt.Payment.ECash > 0)
                {
                    writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_ELECTRONICALLY);");
                    writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(receipt.Payment.ECash / 100.0, 2)});");
                    writer.WriteLine("Fptr.payment();");
                }
                if (receipt.Payment.Pre > 0)
                {
                    writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_PREPAID);");
                    writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(receipt.Payment.Pre / 100.0, 2)});");
                    writer.WriteLine("Fptr.payment();");
                }
                if (receipt.Payment.Post > 0)
                {
                    writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_CREDIT);");
                    writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(receipt.Payment.Post / 100.0, 2)});");
                    writer.WriteLine("Fptr.payment();");
                }
                if (receipt.Payment.Provision > 0)
                {
                    writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_OTHER);");
                    writer.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(receipt.Payment.Provision / 100.0, 2)});");
                    writer.WriteLine("Fptr.payment();");
                }
                writer.WriteLine("Fptr.closeReceipt();");
                writer.WriteLine();
            }
        }
    }
}
