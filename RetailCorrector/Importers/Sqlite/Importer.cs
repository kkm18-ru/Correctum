using Dapper;
using Microsoft.Data.Sqlite;
using Correctum.Core.Models;
using RetailCorrector.Utils;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Importers;

internal partial class Sqlite : ImporterBase<Sqlite, Sqlite.View>
{
    public override string Name => "SQLite";

    public override async IAsyncEnumerable<Receipt> Import(BoundedProgress<uint> progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using (var conn = new SqliteConnection($"Data Source={Path};Mode=ReadOnly"))
        {
            var count = await conn.ExecuteScalarAsync<uint>(COUNT_RECEIPTS);
            progress.ReportMaximum(count);
            for (var i = 0; i < count; i += Configuration.Instance.Sqlite.Limit)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var parameters = new { limit = Configuration.Instance.Sqlite.Limit, offset = i };
                await foreach(var doc in conn.QueryUnbufferedAsync<ReceiptDto>(GET_RECEIPTS, parameters))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return await doc.ToModelAsync(conn);
                    progress.ReportIncrement();
                }
            }
        }
        SqliteConnection.ClearAllPools();
    }
}
