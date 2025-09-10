using System.IO;

namespace RetailCorrector.Exports
{
    internal class Fptr : IExport
    {
        public string Name => "Fptr-скрипт (ФФД 1.2)";

        public ValueTask<string> Export()
        {
            /* TODO-список
             * 1) Добавить поддержку ФФД 1.05 и 1.1
             * 2) Добавить настройку LIBFPTR_PARAM_RECEIPT_ELECTRONICALLY
             * 3) Добавить настройку 1179 и 1173
             */
            var guid = Guid.CreateVersion7();
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RetailCorrector", $"{guid}");
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            Directory.CreateDirectory(path);
            var receipts = Workspace.Instance.Receipts;
            var n = Math.Ceiling((double)receipts.Count / Configuration.Export.CountDocsPerFile);
            for (var i = 0; i < n; i++)
            {
                var chunk = receipts.Skip(i * Configuration.Export.CountDocsPerFile).Take(Configuration.Export.CountDocsPerFile);
                using var fs = new FileStream(Path.Combine(path, $"{i + 1}.txt"), FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                using var tf = new StreamWriter(fs);
                foreach (var receipt in chunk)
                {
                    tf.WriteLine($"Fptr.setParam(1178, new Date(\"{receipt.Created:yyyy'-'MM'-'dd'T'HH':'mm':'ss}\"));");
                    tf.WriteLine("Fptr.setParam(1179, \" \");");
                    tf.WriteLine("Fptr.utilFormTlv();");
                    tf.WriteLine("correctionInfo = Fptr.getParamByteArray(Fptr.LIBFPTR_PARAM_TAG_VALUE);");
                    tf.WriteLine();
                    var operation = $"{((int)receipt.Type > 2 ? "BUY" : "SELL")}_";
                    if ((int)receipt.Type % 2 == 0) operation += "RETURN_";
                    tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_RECEIPT_TYPE, Fptr.LIBFPTR_RT_{operation}CORRECTION);");
                    tf.WriteLine("Fptr.setParam(Fptr.LIBFPTR_PARAM_RECEIPT_ELECTRONICALLY, true);");
                    tf.WriteLine("Fptr.setParam(1173, 0);");
                    tf.WriteLine($"Fptr.setParam(1192, \"{receipt.DocSign}\");");
                    tf.WriteLine("Fptr.setParam(1174, correctionInfo);");
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
            return new(path);
        }
    }
}
