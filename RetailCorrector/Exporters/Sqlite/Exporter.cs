using Dapper;
using Microsoft.Data.Sqlite;
using System.IO;

namespace RetailCorrector.Exporters
{
    internal partial class Sqlite : IExporter
    {
        public string Name => "SQLite";
        public string? FileFilter => "SQLite|*.sqlite";

        public static Sqlite Instance => field ??= new Sqlite();

        public ValueTask Export(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                    "RetailCorrector", $"{Guid.CreateVersion7()}.db");
            Directory.CreateDirectoryIfNotExists(Path.GetDirectoryName(path)!);
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
                        devid = receipt.RegId,
                        drive = receipt.StorageId,
                        shift = receipt.SessionId,
                        taxation = (int)receipt.TaxationSystem,
                        doc = receipt.DocNumber,
                        req = receipt.DocInSession,
                    });
                    foreach (var pos in receipt.Items)
                        conn.Execute(ADD_POSITION, new
                        {
                            name = pos.Name,
                            payment = (int)pos.PayMethod,
                            type = (int)pos.Type,
                            tax = pos.TaxRate.ToFns(),
                            unit = (int?)pos.Unit,
                            price = pos.Price,
                            quantity = pos.Quantity,
                            total = pos.Cost,
                            receipt = id,
                        });
                }
            }
            SqliteConnection.ClearAllPools();
            return new();
        }
    }
}
