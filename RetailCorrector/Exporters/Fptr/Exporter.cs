using RetailCorrector.Data;
using RetailCorrector.Enums;
using RetailCorrector.Utils;
using System.IO;

namespace RetailCorrector.Exporters
{
    internal partial class Fptr : IExporter
    {
        public string Name => "Fptr-скрипт";
        public string? FileFilter => null;

        public static Fptr Instance => field ??= new Fptr();

        public ValueTask Export(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                    "RetailCorrector", $"{Guid.CreateVersion7()}");
            Directory.CreateDirectoryIfNotExists(Path.GetDirectoryName(path));
            Directory.CreateDirectoryIfNotExists(path);
            if (FiscalFormat == 2 && !FixOldReceipts)
                return Missing105Export(path);
            var receipts = Workspace.Instance.Receipts;
            var n = Math.Ceiling((double)receipts.Count / Configuration.Instance.Exporters.Fptr.DocsPerFile);
            for (var i = 0; i < n; i++)
            {
                var chunk = receipts.Skip(i * Configuration.Instance.Exporters.Fptr.DocsPerFile)
                    .Take(Configuration.Instance.Exporters.Fptr.DocsPerFile);
                using var fs = new FileStream(Path.Combine(path, $"{i + 1}.txt"), FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                using var tf = new StreamWriter(fs);
                foreach (var receipt in chunk)
                {
                    tf.WriteGenerateCorrData(ActId, receipt.Created, FiscalFormat == 2);
                    tf.WriteReceiptType(receipt, FiscalFormat);
                    tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_RECEIPT_ELECTRONICALLY, {(!Print).ToString().ToLower()});");
                    if (IsInternet)
                    {
                        tf.WriteLine($"Fptr.setParam(1125, true);");
                        tf.WriteLine($"Fptr.setParam(1187, {SiteAddress});");
                        tf.WriteLine($"Fptr.setParam(1008, {receipt.BuyerContact});");
                    }
                    tf.WriteUseCorrData(CorrType, FiscalFormat == 2);
                    tf.WriteLine($"Fptr.setParam(1192, \"{receipt.DocSign}\");");
                    tf.WriteLine("Fptr.openReceipt();");
                    tf.WriteSeparator();
                    tf.WritePositions(receipt.Items, false, FiscalFormat);
                    tf.WriteSeparator(2);
                    tf.WriteTotalSum(receipt.TotalSum);
                    tf.WritePayment(receipt.Payment);
                    tf.WriteCloseReceipt();
                    tf.WriteSeparator(3);
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
                ulong total = (ulong)@ref.Where(filter).Sum(r => (long)r.TotalSum),
                      cash =  (ulong)@ref.Where(filter).Sum(r => (long)r.Payment.Cash),
                      ecash = (ulong)@ref.Where(filter).Sum(r => (long)r.Payment.ECash),
                      pre =   (ulong)@ref.Where(filter).Sum(r => (long)r.Payment.Pre),
                      post =  (ulong)@ref.Where(filter).Sum(r => (long)r.Payment.Post),
                      other = (ulong)@ref.Where(filter).Sum(r => (long)r.Payment.Provision);

                tf.WriteGenerateCorrData(ActId, DateTime.Now);
                tf.WriteReceiptType($"{operation}_CORRECTION");
                tf.WriteUseCorrData(CorrType);
                tf.WriteLine("Fptr.openReceipt();");
                tf.WriteSeparator();
                tf.WritePosition(new Position
                {
                    Name = "Товар",
                    Price = total,
                    Quantity = 1000,
                    Cost = total
                }, true, 2);
                tf.WriteSeparator(2);
                tf.WriteTotalSum(total);
                tf.WritePayment(new Payment(cash, ecash, pre, post, other));
                tf.WriteCloseReceipt();
                tf.WriteSeparator(3);
            }
            #endregion

            WriteReceipt(r => r.Type == Operation.Income || r.Type == Operation.RefundOutcome, "SELL");
            WriteReceipt(r => r.Type == Operation.RefundIncome || r.Type == Operation.Outcome, "BUY");
            
            return new();
        }
    }

    file static class StreamWriterExtensions
    {
        public static void WriteSeparator(this StreamWriter tf, byte count = 1)
        {
            for (var i = 0; i < count; i++)
                tf.WriteLine();
        }
        
        public static void WriteReceiptType(this StreamWriter tf, Receipt receipt, int fiscalFormat)
        {
            var operation = (int)receipt.Type > 2 ? "BUY" : "SELL";
            if ((int)receipt.Type % 2 == 0) operation += "_RETURN";
            if (fiscalFormat < 2) operation += "_CORRECTION";
            tf.WriteReceiptType(operation);
        }
        
        public static void WriteReceiptType(this StreamWriter tf, string operation) =>
            tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_RECEIPT_TYPE, Fptr.LIBFPTR_RT_{operation});");

        public static void WriteCloseReceipt(this StreamWriter tf)
        {
            tf.WriteLine("Fptr.payment();");
            tf.WriteLine("Fptr.closeReceipt();");
        }

        public static void WriteTotalSum(this StreamWriter tf, ulong sum)
        {
            tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_SUM, {Math.Round(sum / 100.0, 2)});");
            tf.WriteLine("Fptr.receiptTotal();");
            tf.WriteLine();
        }

        private static void WriteInternalPayment(this StreamWriter tf, string type, ulong sum)
        {
            tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_{type});");
            tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(sum / 100.0, 2)});");
            tf.WriteLine();
        }

        public static void WritePayment(this StreamWriter tf, Payment payment)
        {
            if (payment.Cash > 0) tf.WriteInternalPayment("CASH", payment.Cash);
            if (payment.ECash > 0) tf.WriteInternalPayment("ELECTRONICALLY", payment.ECash);
            if (payment.Pre > 0) tf.WriteInternalPayment("PREPAID", payment.Pre);
            if (payment.Post > 0) tf.WriteInternalPayment("CREDIT", payment.Post);
            if (payment.Provision > 0) tf.WriteInternalPayment("OTHER", payment.Provision);
        }

        public static void WritePositions(this StreamWriter tf, Position[] positions, bool onlyBasic, int fiscalFormat)
        {
            foreach (var pos in positions)
                tf.WritePosition(pos, onlyBasic, fiscalFormat);
        }

        public static void WritePosition(this StreamWriter tf, Position pos, bool onlyBasic, int fiscalFormat)
        {
            tf.WriteLine();
            tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_COMMODITY_NAME, \"{pos.Name.Replace("\"", "\\\"")}\");");
            tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PRICE, {Math.Round(pos.Price / 100.0, 2)});");
            tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_QUANTITY, {Math.Round(pos.Quantity / 1000.0, 3)});");
            tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_POSITION_SUM, {Math.Round(pos.Cost / 100.0, 2)});");
            if (!onlyBasic)
            {
                tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_TAX_TYPE, {(int)pos.TaxRate});");
                tf.WriteLine($"Fptr.setParam(1212, {(int)pos.Type});");
                tf.WriteLine($"Fptr.setParam(1214, {(int)pos.PayMethod});");
                if (fiscalFormat == 0)
                    tf.WriteLine($"Fptr.setParam(2108, {(int)pos.MeasureUnit!});");
            }
            tf.WriteLine("Fptr.registration();");
        }

        public static void WriteGenerateCorrData(this StreamWriter tf, string act, DateTime date, bool isSkipped = false)
        {
            if (isSkipped) return;
            tf.WriteLine($"Fptr.setParam(1178, new Date(\"{date:yyyy'-'MM'-'dd'T'HH':'mm':'ss}\"));");
            tf.WriteLine($"Fptr.setParam(1179, \"{(string.IsNullOrWhiteSpace(act) ? " " : act)}\");");
            tf.WriteLine("Fptr.utilFormTlv();");
            tf.WriteLine("correctionInfo = Fptr.getParamByteArray(Fptr.LIBFPTR_PARAM_TAG_VALUE);");
            tf.WriteLine();
        }

        public static void WriteUseCorrData(this StreamWriter tf, int corrType, bool isSkipped = false)
        {
            if (isSkipped) return;
            tf.WriteLine($"Fptr.setParam(1173, {corrType});");
            tf.WriteLine("Fptr.setParam(1174, correctionInfo);");
        }
    }
}
