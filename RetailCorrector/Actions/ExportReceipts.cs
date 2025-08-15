using Dapper;
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
            if (!Directory.Exists(Path.GetDirectoryName(path))) 
                Directory.CreateDirectory(Path.GetDirectoryName(path));
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
            MessageBox.Show($"Чеки сохранены в {guid}.db");
        }
    }
}
