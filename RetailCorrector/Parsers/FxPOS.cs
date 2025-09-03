using Microsoft.Data.Sqlite;
using RetailCorrector.Data;
using Dapper;
using System.Runtime.CompilerServices;
using RetailCorrector.Attributes;

namespace RetailCorrector.Parsers
{
    [RegisterParser("Frontol xPOS")]
    internal partial class FxPOS : ParserBase<FxPOS, FxPOS.View>
    {
        public override async IAsyncEnumerable<Receipt> Parse([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var parseInfo = new ParseInfo("Frontol xPOS", ("Путь", Path));
            Workspace.Instance.ParserHistory.Add(parseInfo);
            using var conn = new SqliteConnection($"Data Source={Path};Mode=ReadOnly");
            var count = await conn.ExecuteScalarAsync<int>(COUNT_RECEIPTS);
            for(var i = 0; i < count; i+= LIMIT)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var docs = await conn.QueryAsync<Document>(GET_RECEIPTS, new
                {
                    limit = LIMIT,
                    offset = i
                });
                foreach(var doc in docs)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    yield return new()
                    {
                        ParseId = parseInfo.Id,
                        Items = [.. (await conn.QueryAsync<InnerPosition>(GET_POSITIONS, doc)).Select(i => (Position)i)],
                        Created = doc.Date,
                        DocSign = doc.Sign,
                        TotalSum = doc.Total,
                        Payment = await conn.QuerySingleAsync<Payment>(GET_PAYMENT, doc),
                        Type = doc.Type switch
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
    }
}
