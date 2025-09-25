using RetailCorrector.Attributes;
using RetailCorrector.Data;
using RetailCorrector.Enums;
using RetailCorrector.Utils;
using System.IO;

namespace RetailCorrector.Exports
{
    [RegisterExport("Fptr-скрипт")]
    internal partial class Fptr : IExport
    {
        public string? FileFilter => null;

        public ValueTask Export(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                    "RetailCorrector", $"{Guid.CreateVersion7()}");
            IOHelper.CreateDirectoryIfNotExists(Path.GetDirectoryName(path));
            IOHelper.CreateDirectoryIfNotExists(path);
            if (FiscalFormat == 2 && OldCorrReason == 1)
                return Missing105Export(path);
            var receipts = Workspace.Instance.Receipts;
            var n = Math.Ceiling((double)receipts.Count / Configuration.Instance.FptrExportDocsPerFile);
            for (var i = 0; i < n; i++)
            {
                var chunk = receipts.Skip(i * Configuration.Instance.FptrExportDocsPerFile)
                    .Take(Configuration.Instance.FptrExportDocsPerFile);
                using var fs = new FileStream(Path.Combine(path, $"{i + 1}.txt"), FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                using var tf = new StreamWriter(fs);
                foreach (var receipt in chunk)
                {
                    if (FiscalFormat < 2)
                    {
                        tf.WriteLine($"Fptr.setParam(1178, new Date(\"{receipt.Created:yyyy'-'MM'-'dd'T'HH':'mm':'ss}\"));");
                        tf.WriteLine($"Fptr.setParam(1179, \"{(string.IsNullOrWhiteSpace(ActId) ? " " : ActId)}\");");
                        tf.WriteLine("Fptr.utilFormTlv();");
                        tf.WriteLine("correctionInfo = Fptr.getParamByteArray(Fptr.LIBFPTR_PARAM_TAG_VALUE);");
                        tf.WriteLine();
                    }
                    var operation = (int)receipt.Type > 2 ? "BUY" : "SELL";
                    if ((int)receipt.Type % 2 == 0) operation += "_RETURN";
                    if (FiscalFormat < 2) operation += "_CORRECTION";
                    tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_RECEIPT_TYPE, Fptr.LIBFPTR_RT_{operation});");
                    tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_RECEIPT_ELECTRONICALLY, {(Print == 1).ToString().ToLower()});");
                    if (FiscalFormat < 2)
                    {
                        tf.WriteLine($"Fptr.setParam(1173, {CorrType});");
                        tf.WriteLine("Fptr.setParam(1174, correctionInfo);");
                    }
                    tf.WriteLine($"Fptr.setParam(1192, \"{receipt.DocSign}\");");
                    tf.WriteLine("Fptr.openReceipt();");
                    tf.WriteLine();

                    foreach (var p in receipt.Items)
                    {
                        tf.WriteLine();
                        tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_COMMODITY_NAME, \"{p.Name.Replace("\"", "\\\"")}\");");
                        tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PRICE, {Math.Round(p.Price / 100.0, 2)});");
                        tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_QUANTITY, {Math.Round(p.Quantity / 1000.0, 3)});");
                        tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_POSITION_SUM, {Math.Round(p.Cost / 100.0, 2)});");
                        tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_TAX_TYPE, {(int)p.TaxRate});");
                        tf.WriteLine($"Fptr.setParam(1212, {(int)p.Type});");
                        tf.WriteLine($"Fptr.setParam(1214, {(int)p.PayMethod});");
                        if(FiscalFormat == 0) 
                            tf.WriteLine($"Fptr.setParam(2108, {(int)p.MeasureUnit!});");
                        tf.WriteLine("Fptr.registration();");
                    }
                    tf.WriteLine();
                    tf.WriteLine();
                    if (receipt.Payment.Cash > 0)
                    {
                        tf.WriteLine("Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_CASH);");
                        tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(receipt.Payment.Cash / 100.0, 2)});");
                        tf.WriteLine();
                    }
                    if (receipt.Payment.ECash > 0)
                    {
                        tf.WriteLine("Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_ELECTRONICALLY);");
                        tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(receipt.Payment.ECash / 100.0, 2)});");
                        tf.WriteLine();
                    }
                    if (receipt.Payment.Pre > 0)
                    {
                        tf.WriteLine("Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_PREPAID);");
                        tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(receipt.Payment.Pre / 100.0, 2)});");
                        tf.WriteLine();
                    }
                    if (receipt.Payment.Post > 0)
                    {
                        tf.WriteLine("Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_CREDIT);");
                        tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(receipt.Payment.Post / 100.0, 2)});");
                        tf.WriteLine();
                    }
                    if (receipt.Payment.Provision > 0)
                    {
                        tf.WriteLine("Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_OTHER);");
                        tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(receipt.Payment.Provision / 100.0, 2)});");
                        tf.WriteLine();
                    }
                    tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(receipt.TotalSum / 100.0, 2)});");
                    tf.WriteLine("Fptr.payment();");
                    tf.WriteLine("Fptr.closeReceipt();");
                    tf.WriteLine();
                    tf.WriteLine();
                    tf.WriteLine();
                }
            }
            return new();
        }

        public ValueTask Missing105Export(string path)
        {
            var @ref = Workspace.Instance.Receipts;
            path = Path.Combine(path, "1.txt");
            using var fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            using var tf = new StreamWriter(fs);

            #region Внутренние функции
            void WriteReceipt(Func<Receipt, bool> filter, string operation)
            {
                ulong total = 0, cash = 0, ecash = 0, pre = 0, post = 0, other = 0;
                foreach(var receipt in @ref.Where(filter))
                {
                    total += receipt.TotalSum;
                    cash += receipt.Payment.Cash;
                    ecash += receipt.Payment.ECash;
                    pre += receipt.Payment.Pre;
                    post += receipt.Payment.Post;
                    other += receipt.Payment.Provision;
                }

                tf.WriteLine($"Fptr.setParam(1178, new Date(\"{DateTime.Now:yyyy'-'MM'-'dd'T'HH':'mm':'ss}\"));");
                tf.WriteLine($"Fptr.setParam(1179, \"{ActId}\");");
                tf.WriteLine("Fptr.utilFormTlv();");
                tf.WriteLine("correctionInfo = Fptr.getParamByteArray(Fptr.LIBFPTR_PARAM_TAG_VALUE);");
                tf.WriteLine();
                tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_RECEIPT_TYPE, Fptr.LIBFPTR_RT_{operation}_CORRECTION);");
                tf.WriteLine($"Fptr.setParam(1173, {CorrType});");
                tf.WriteLine("Fptr.setParam(1174, correctionInfo);");
                tf.WriteLine("Fptr.openReceipt();");
                tf.WriteLine();
                tf.WriteLine();
                tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_COMMODITY_NAME, \"Товар\");");
                tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PRICE, {Math.Round(total / 100.0, 2)});");
                tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_QUANTITY, 1.0);");
                tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_POSITION_SUM, {Math.Round(total / 100.0, 2)});");
                tf.WriteLine("Fptr.registration();");
                tf.WriteLine();
                tf.WriteLine();
                if (cash > 0)
                {
                    tf.WriteLine("Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_CASH);");
                    tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(cash / 100.0, 2)});");
                    tf.WriteLine();
                }
                if (ecash > 0)
                {
                    tf.WriteLine("Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_ELECTRONICALLY);");
                    tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(ecash / 100.0, 2)});");
                    tf.WriteLine();
                }
                if (pre > 0)
                {
                    tf.WriteLine("Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_PREPAID);");
                    tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(pre / 100.0, 2)});");
                    tf.WriteLine();
                }
                if (post > 0)
                {
                    tf.WriteLine("Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_CREDIT);");
                    tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(post / 100.0, 2)});");
                    tf.WriteLine();
                }
                if (other > 0)
                {
                    tf.WriteLine("Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_OTHER);");
                    tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(other / 100.0, 2)});");
                    tf.WriteLine();
                }
                tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(total / 100.0, 2)});");
                tf.WriteLine("Fptr.payment();");
                tf.WriteLine("Fptr.closeReceipt();");
                tf.WriteLine();
                tf.WriteLine();
                tf.WriteLine();
            }
            #endregion

            WriteReceipt(r => r.Type == Operation.Income || r.Type == Operation.RefundOutcome, "SELL");
            WriteReceipt(r => r.Type == Operation.RefundIncome || r.Type == Operation.Outcome, "BUY");
            
            return new();
        }
    }
}
