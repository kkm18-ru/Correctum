using Microsoft.Data.Sqlite;
using System.IO;
using System.Windows;

namespace RetailCorrector.Actions.ExportReceipts
{
    internal class ExportReceiptsView : ActionView<ExportReceiptsView, ExportReceiptsPresenter>;

    [RegisterAction("Экспорт чеков")]
    internal class ExportReceiptsPresenter : ActionPresenter<ExportReceiptsPresenter, ExportReceiptsView>
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

        public override void Start()
        {
            var guid = Guid.CreateVersion7();
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RetailCorrector", $"{guid}.db");
            using (var conn = new SqliteConnection($"Data Source = \"{path}\""))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = CREATE_RECEIPT_TABLE;
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = CREATE_POSITION_TABLE;
                    cmd.ExecuteNonQuery();
                }
                foreach (var receipt in Workspace.Instance.Receipts)
                {
                    long id = 0;
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = ADD_RECEIPT;
                        cmd.Parameters.AddWithValue("date", $"{receipt.Created:yyyy'-'MM'-'dd' 'HH':'mm':'ss}");
                        cmd.Parameters.AddWithValue("type", (int)receipt.Type);
                        cmd.Parameters.AddWithValue("sign", $"{receipt.DocSign}");
                        cmd.Parameters.AddWithValue("cash", receipt.Payment.Cash);
                        cmd.Parameters.AddWithValue("ecash", receipt.Payment.ECash);
                        cmd.Parameters.AddWithValue("pre", receipt.Payment.Pre);
                        cmd.Parameters.AddWithValue("post", receipt.Payment.Post);
                        cmd.Parameters.AddWithValue("other", receipt.Payment.Provision);
                        cmd.Parameters.AddWithValue("total", receipt.TotalSum);
                        id = (long)cmd.ExecuteScalar()!;
                    }
                    foreach (var pos in receipt.Items)
                    {
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = ADD_POSITION;
                            cmd.Parameters.AddWithValue("name", pos.Name);
                            cmd.Parameters.AddWithValue("payment", (int)pos.PayMethod);
                            cmd.Parameters.AddWithValue("type", (int)pos.Type);
                            cmd.Parameters.AddWithValue("tax", (int)pos.TaxRate);
                            cmd.Parameters.AddWithValue("unit", (int?)pos.MeasureUnit);
                            cmd.Parameters.AddWithValue("price", pos.Price);
                            cmd.Parameters.AddWithValue("quantity", pos.Quantity);
                            cmd.Parameters.AddWithValue("total", pos.Cost);
                            cmd.Parameters.AddWithValue("receipt", id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            MessageBox.Show($"Чеки сохранены в {guid}.db");
        }
    }
}
