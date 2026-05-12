using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Core.Models;
using Correctum.Utils;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Runtime.CompilerServices;

namespace Correctum.Imports;

[ImportInfo("SQLite")]
[TextBox("Путь", nameof(Path))]
public sealed partial class SqliteImport : BaseImport
{
    public string Path { get; set => SetField(ref field, value); } = string.Empty;

    public override async IAsyncEnumerable<Receipt> ImportAsync
        ([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using (var conn = new SqliteConnection($"Data Source={Path};Mode=ReadOnly"))
        {
            var count = await conn.ExecuteScalarAsync<uint>(COUNT_RECEIPTS);
            for (var i = 0; i < count; i += Configuration.Current.Import.Sqlite.StackSize)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var parameters = new { limit = Configuration.Current.Import.Sqlite.StackSize, offset = i };
                await foreach (var doc in conn.QueryUnbufferedAsync<ReceiptDto>(GET_RECEIPTS, parameters))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return await doc.ToModelAsync(conn);
                }
            }
        }
        SqliteConnection.ClearAllPools();
    }

}
