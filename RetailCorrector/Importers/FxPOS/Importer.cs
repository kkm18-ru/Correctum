using Dapper;
using Microsoft.Data.Sqlite;
using RetailCorrector.Data;
using RetailCorrector.Utils;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Importers;

internal partial class FxPOS : ImporterBase<FxPOS, FxPOS.View>
{
    public override string Name => "Frontol xPOS";

    public override ImportSession GenerateInfo() =>
        parseInfo = new ImportSession("Frontol xPOS", ("Путь", Path));

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
                        ParseId = parseInfo.Id,
                        Items = [.. (await Task.Run(() => conn.QueryAsync<InnerPosition>(GET_POSITIONS, docs[j]))).Select(i => (Position)i)],
                        Created = docs[j].Date,
                        DocSign = new(docs[j].Sign),
                        TotalSum = docs[j].Total,
                        Payment = await Task.Run(() => conn.QuerySingleAsync<Payment>(GET_PAYMENT, docs[j])),
                        Type = docs[j].Type switch
                        {
                            0 or 19 => Enums.Operation.Income,
                            1 => Enums.Operation.RefundIncome,
                            25 or 20 => Enums.Operation.Outcome,
                            26 => Enums.Operation.RefundOutcome
                        }
                    };
                    progress.ReportIncrement();
                }
            }
        }
        SqliteConnection.ClearAllPools();
    }
}
