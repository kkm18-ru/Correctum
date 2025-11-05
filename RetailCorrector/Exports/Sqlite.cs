using Dapper;
using Microsoft.Data.Sqlite;
using RetailCorrector.Utils;
using System.IO;

namespace RetailCorrector.Exports
{
    internal partial class Sqlite : IExport
    {
        public string Name => "SQLite";
        public string? FileFilter => "*.sql|SQLite DataBase";

        private static Sqlite _instance = null!;
        public static Sqlite Instance => _instance ??= new Sqlite();

        public ValueTask Export(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                    "RetailCorrector", $"{Guid.CreateVersion7()}.db");
            IOHelper.CreateDirectoryIfNotExists(Path.GetDirectoryName(path)!);
            using (var conn = new SqliteConnection($"Data Source = \"{path}\""))
            {
                conn.Open();
                conn.Execute(CREATE_RECEIPT_TABLE);
                conn.Execute(CREATE_PKEY_RECEIPT_INDEX);
                conn.Execute(CREATE_POSITION_TABLE);
                conn.Execute(CREATE_RID_POSITION_INDEX);
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
                        buyer = receipt.BuyerContact,
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
            return new();
        }
    }
}
