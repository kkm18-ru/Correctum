using Dapper;
using Microsoft.Data.Sqlite;
using RetailCorrector.Data;
using Correctum.Core.Models;
using Correctum.Core.Enums;
using RetailCorrector.Utils;
using Correctum.Core.ValueObjects;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Importers;

internal partial class Sqlite : ImporterBase<Sqlite, Sqlite.View>
{
    public override string Name => "SQLite";

    public override ImportSession GenerateInfo() =>
        parseInfo = new ImportSession("SQLite", ("Путь", Path));

    public override async IAsyncEnumerable<Receipt> Import(BoundedProgress<uint> progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using (var conn = new SqliteConnection($"Data Source={Path};Mode=ReadOnly"))
        {
            var count = await conn.ExecuteScalarAsync<uint>(COUNT_RECEIPTS);
            progress.ReportMaximum(count);
            for (var i = 0; i < count; i += Configuration.Instance.Sqlite.Limit)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var docs = (await Task.Run(() => conn.QueryAsync<Document>(GET_RECEIPTS, new { limit = Configuration.Instance.Sqlite.Limit, offset = i }))).ToArray();
                for (var j = 0; j < docs.Length; j++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    yield return new()
                    {
                        //ParseId = parseInfo.Id,
                        Items = [.. (await Task.Run(() => conn.QueryAsync<InnerPosition>(GET_POSITIONS, docs[j]))).Select(i => new Position {
                            Name = i.name,
                            Cost = i.total,
                            Unit = i.unit,
                            CalcMethod = i.payment,
                            Price = i.price,
                            Quantity = i.quantity,
                            TaxRate = EnumUtils.ToTaxRate(i.tax),
                            Type = i.type
                        })],
                        Created = docs[j].date,
                        RegId = docs[j].devid,
                        StorageId = docs[j].drive,
                        SessionId = docs[j].shift,
                        DocNumber = docs[j].doc,
                        DocInSession = docs[j].req,
                        DocSign = FiscalSign.Parse(docs[j].sign),
                        TotalSum = docs[j].total,
                        Payment = new Payment
                        {
                            CashSum = docs[j].cash,
                            NonCashSum = docs[j].ecash,
                            AdvanceSum = docs[j].pre,
                            CreditSum = docs[j].post,
                            ProvisionSum = docs[j].other,
                        },
                        Type = docs[j].type,
                        BuyerContact = docs[j].buyer,
                        TaxationSystem = (TaxationSystems)docs[j].taxation
                    };
                    progress.ReportIncrement();
                }
            }
        }
        SqliteConnection.ClearAllPools();
    }
}
