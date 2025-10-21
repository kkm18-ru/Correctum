using Dapper;
using Microsoft.Data.Sqlite;
using RetailCorrector.Data;
using RetailCorrector.Utils;
using System.Runtime.CompilerServices;
using Kraig.Attributes;

namespace RetailCorrector.Parsers
{
    [Singleton]
    internal partial class Sqlite : ParserBase<Sqlite, Sqlite.View>
    {
        public override string Name => "SQLite";

        public override ParseInfo GenerateInfo() =>
            parseInfo = new ParseInfo("SQLite", ("Путь", Path));

        public override async IAsyncEnumerable<Receipt> Parse(IProgress<long> progress, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            using var conn = new SqliteConnection($"Data Source={Path};Mode=ReadOnly");
            var count = await conn.ExecuteScalarAsync<int>(COUNT_RECEIPTS);
            progress.Report(-count);
            for (var i = 0; i < count; i += Configuration.Instance.SqlLimit)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var docs = (await Task.Run(() => conn.QueryAsync<Document>(GET_RECEIPTS, new { limit = Configuration.Instance.SqlLimit, offset = i }))).ToArray();
                for (var j = 0; j < docs.Length; j++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    yield return new()
                    {
                        ParseId = parseInfo.Id,
                        Items = [.. (await Task.Run(() => conn.QueryAsync<InnerPosition>(GET_POSITIONS, docs[j]))).Select(i => new Position {
                            Name = i.name,
                            Cost = i.total,
                            MeasureUnit = i.unit,
                            PayMethod = i.payment,
                            Price = i.price,
                            Quantity = i.quantity,
                            TaxRate = i.tax,
                            Type = i.type
                        })],
                        Created = docs[j].date,
                        DocSign = ulong.Parse(docs[j].sign),
                        TotalSum = docs[j].total,
                        Payment = new Payment
                        {
                            Cash = docs[j].cash,
                            ECash = docs[j].ecash,
                            Pre = docs[j].pre,
                            Post = docs[j].post,
                            Provision = docs[j].other,
                        },
                        Type = docs[j].type
                    };
                    progress.Report(i + j);
                }
            }
        }
    }
}
