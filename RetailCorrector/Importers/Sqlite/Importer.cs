using Dapper;
using Microsoft.Data.Sqlite;
using RetailCorrector.Data;
using RetailCorrector.Utils;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Importers;

internal partial class Sqlite : ImporterBase<Sqlite, Sqlite.View>
{
    public override string Name => "SQLite";

    public override ImportSession GenerateInfo() =>
        parseInfo = new ImportSession("SQLite", ("Путь", Path));

    public override async IAsyncEnumerable<Receipt> Import(IProgress<long> progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using (var conn = new SqliteConnection($"Data Source={Path};Mode=ReadOnly"))
        {
            var count = await conn.ExecuteScalarAsync<int>(COUNT_RECEIPTS);
            progress.Report(-count);
            for (var i = 0; i < count; i += Configuration.Instance.Sqlite.Limit)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var docs = (await Task.Run(() => conn.QueryAsync<Document>(GET_RECEIPTS, new { limit = Configuration.Instance.Sqlite.Limit, offset = i }))).ToArray();
                for (var j = 0; j < docs.Length; j++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var receipt = new Receipt
                    {
                        ParseId = parseInfo.Id,
                        Created = docs[j].date,
                        RegId = docs[j].devid,
                        StorageId = docs[j].drive,
                        SessionId = docs[j].shift,
                        DocNumber = docs[j].doc,
                        DocInSession = docs[j].req,
                        DocSign = uint.Parse(docs[j].sign),
                        TotalSum = docs[j].total,
                        Payment = new Payment
                        {
                            Cash = docs[j].cash,
                            ECash = docs[j].ecash,
                            Pre = docs[j].pre,
                            Post = docs[j].post,
                            Provision = docs[j].other,
                        },
                        Type = docs[j].type,
                        BuyerContact = docs[j].buyer
                    };
                    receipt.Items =
                    [
                        .. (await Task.Run(() => conn.QueryAsync<InnerPosition>(GET_POSITIONS, docs[j]))).Select(i =>
                            new Position
                            {
                                Receipt = receipt,
                                Name = i.name,
                                Cost = i.total,
                                MeasureUnit = i.unit,
                                PayMethod = i.payment,
                                Price = i.price,
                                Quantity = i.quantity,
                                TaxRate = EnumUtils.ToTaxRate(i.tax),
                                Type = i.type
                            })
                    ];
                    yield return receipt;
                    progress.Report(i + j);
                }
            }
        }
        SqliteConnection.ClearAllPools();
    }
}
