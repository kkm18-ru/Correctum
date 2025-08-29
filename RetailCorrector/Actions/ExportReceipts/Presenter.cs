using Dapper;
using Microsoft.Data.Sqlite;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace RetailCorrector.Actions.ExportReceipts
{
    [RegisterAction("Экспорт чеков")]
    internal partial class ExportReceiptsPresenter : ActionPresenter<ExportReceiptsPresenter, ExportReceiptsView>
    {
        const string CREATE_POSITION_TABLE = """
            CREATE TABLE position (
                id       INTEGER NOT NULL
                                 CONSTRAINT PK_position PRIMARY KEY AUTOINCREMENT,
                name     TEXT,
                payment  INTEGER NOT NULL,
                type     INTEGER NOT NULL,
                tax      INTEGER NOT NULL,
                unit     INTEGER,
                price    INTEGER NOT NULL,
                quantity INTEGER NOT NULL,
                total    INTEGER NOT NULL,
                receipt  INTEGER NOT NULL,
                CONSTRAINT FK_position_receipt_receipt FOREIGN KEY (
                    receipt
                )
                REFERENCES receipt (id) ON DELETE CASCADE
            );
            """;
        const string CREATE_RECEIPT_TABLE = """
            CREATE TABLE receipt (
                id    INTEGER NOT NULL
                              CONSTRAINT PK_receipt PRIMARY KEY AUTOINCREMENT,
                date  TEXT    NOT NULL,
                type  INTEGER NOT NULL,
                sign  TEXT    NOT NULL,
                corr  INTEGER NOT NULL,
                act   TEXT,
                cash  INTEGER NOT NULL,
                ecash INTEGER NOT NULL,
                pre   INTEGER NOT NULL,
                post  INTEGER NOT NULL,
                other INTEGER NOT NULL,
                total INTEGER NOT NULL
            );            
            """;
        const string ADD_RECEIPT = """
            INSERT INTO receipt (date, type, sign, corr, act, cash, ecash, pre, post, other, total) 
            VALUES (@date, @type, @sign, 0, ' ', @cash, @ecash, @pre, @post, @other, @total) RETURNING id;
            """;
        const string ADD_POSITION = """
            INSERT INTO position (name, payment, type, tax, unit, price, quantity, total, receipt) 
            VALUES (@name, @payment, @type, @tax, @unit, @price, @quantity, @total, @receipt);
            """;

        const int COUNT_RECEIPTS_PER_FILE = 25;

        public override void Start()
        {
            var path = _type switch 
            { 
                Enums.ExportDestination.SQLite => SQLite(),
                Enums.ExportDestination.FPTR => Fptr(),
            };
            if (MessageBox.Show($"Открыть каталог с сохранёнными чеками?", "Чеки сохранены", 
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                Process.Start("explorer.exe", path);
        }

        private string Fptr()
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
            var n = Math.Ceiling((double)receipts.Count / COUNT_RECEIPTS_PER_FILE);
            for (var i = 0; i < n; i++)
            {
                var chunk = receipts.Skip(i * COUNT_RECEIPTS_PER_FILE).Take(COUNT_RECEIPTS_PER_FILE);
                using var fs = new FileStream(Path.Combine(path, $"{i+1}.txt"), FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                using var tf = new StreamWriter(fs);
                foreach(var receipt in chunk)
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
                    if(receipt.Payment.Cash > 0)
                    {
                        tf.WriteLine("Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_CASH);");
                        tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(receipt.Payment.Cash / 100.0, 2)});");
                        tf.WriteLine();
                    }
                    if(receipt.Payment.ECash > 0)
                    {
                        tf.WriteLine("Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_ELECTRONICALLY);");
                        tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(receipt.Payment.ECash / 100.0, 2)});");
                        tf.WriteLine();
                    }
                    if(receipt.Payment.Pre > 0)
                    {
                        tf.WriteLine("Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_PREPAID);");
                        tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(receipt.Payment.Pre / 100.0, 2)});");
                        tf.WriteLine();
                    }
                    if(receipt.Payment.Post > 0)
                    {
                        tf.WriteLine("Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_CREDIT);");
                        tf.WriteLine($"Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, {Math.Round(receipt.Payment.Post / 100.0, 2)});");
                        tf.WriteLine();
                    }
                    if(receipt.Payment.Provision > 0)
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
            return path;
        }

        private string SQLite()
        {
            var guid = Guid.CreateVersion7();
            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RetailCorrector");
            var path = Path.Combine(dir, $"{guid}.db");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            using (var conn = new SqliteConnection($"Data Source = \"{path}\""))
            {
                conn.Open();
                conn.Execute(CREATE_RECEIPT_TABLE);
                conn.Execute(CREATE_POSITION_TABLE);
                foreach (var receipt in Workspace.Instance.Receipts)
                {
                    var id = conn.QuerySingle<long>(ADD_RECEIPT, new
                    {
                        date = $"{receipt.Created:yyyy'-'MM'-'dd' 'HH':'mm':'ss}",
                        type = (int)receipt.Type,
                        sign = $"{receipt.DocSign}",
                        cash = receipt.Payment.Cash,
                        ecash = receipt.Payment.ECash,
                        pre = receipt.Payment.Pre,
                        post = receipt.Payment.Post,
                        other = receipt.Payment.Provision,
                        total = receipt.TotalSum,
                    });
                    foreach (var pos in receipt.Items)
                        conn.Execute(ADD_POSITION, new
                        {
                            name = pos.Name,
                            payment = (int)pos.PayMethod,
                            type = (int)pos.Type,
                            tax = (int)pos.TaxRate,
                            unit = (int?)pos.MeasureUnit,
                            price = pos.Price,
                            quantity = pos.Quantity,
                            total = pos.Cost,
                            receipt = id,
                        });
                }
            }
            return dir;
        }
    }
}
