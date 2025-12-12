using Dapper;
using Microsoft.Data.Sqlite;
using RetailCorrector.Data;
using RetailCorrector.Utils;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Parsers
{
    internal partial class FxPOS : ParserBase<FxPOS, FxPOS.View>
    {
        public override string Name => "Frontol xPOS";

        public override ParseInfo GenerateInfo() =>
            parseInfo = new ParseInfo("Frontol xPOS", ("Путь", Path));

        public override async IAsyncEnumerable<Receipt> Parse(IProgress<long> progress, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            using (var conn = new SqliteConnection($"Data Source={Path};Mode=ReadOnly"))
            {
                var count = await conn.ExecuteScalarAsync<int>(COUNT_RECEIPTS);
                progress.Report(-count);
                for (var i = 0; i < count; i += Configuration.Instance.Sqlite.Limit)
                {
                    progress.Report(i);
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
                            DocSign = docs[j].Sign,
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
                    }
                }
            }
            SqliteConnection.ClearAllPools();
        }
    }
}
