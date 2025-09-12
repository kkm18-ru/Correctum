using Dapper;
using Microsoft.Data.Sqlite;
using RetailCorrector.Attributes;
using System.IO;

namespace RetailCorrector.Exports
{
    [RegisterExport("SQLite")]
    internal partial class Sqlite : IExport
    {
        public ValueTask<string> Export()
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
            return new(dir);
        }
    }
}
