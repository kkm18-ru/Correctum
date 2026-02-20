using System.Runtime.CompilerServices;
using Dapper;
using Microsoft.Data.Sqlite;
using RetailCorrector.Attributes;
using RetailCorrector.Data;
using RetailCorrector.Enums;
using RetailCorrector.Utils;

namespace RetailCorrector.Importers;

[ImporterName("SQLite")]
public sealed partial class SqliteImporter : ImporterBase
{
    [ImporterProperty("Путь к файлу")]
    public string FilePath { get; set => SetField(ref field, value); } = "";
    
    public SqliteImporter()
    {
        InitializeComponent();
    }

    public override async IAsyncEnumerable<Receipt> ImportAsync(VisualProgress progress, Guid sessionId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using (var conn = new SqliteConnection($"Data Source={FilePath};Mode=ReadOnly"))
        {
            var count = await conn.ExecuteScalarAsync<int>("select count(id) from receipt;");
            progress.ReportMaxValue(count);
            for (var i = 0; i < count; i += Configuration.Instance.Sqlite.Limit)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var docs = (await Task.Run(() => conn.QueryAsync<Document>("select * from receipt limit @limit offset @offset;", new { limit = Configuration.Instance.Sqlite.Limit, offset = i }), cancellationToken)).ToArray();
                for (var j = 0; j < docs.Length; j++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    yield return new Receipt
                    {
                        ImportSessionId = sessionId,
                        Items = [.. (await Task.Run(() => conn.QueryAsync<InnerPosition>("select * from position where receipt = @id", docs[j]), cancellationToken)).Select(i => new Position {
                            Name = i.name,
                            Cost = i.total,
                            Unit = i.unit,
                            PayMethod = i.payment,
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
                    progress.ReportCurrValue(i + j);
                }
            }
        }
        SqliteConnection.ClearAllPools();
    }
}

file readonly struct Document
{
    public ulong id { get; init; }
    public string devid { get; init; }
    public string drive { get; init; }
    public uint shift { get; init; }
    public uint doc { get; init; }
    public uint req { get; init; }
    public DateTime date { get; init; }
    public Operation type { get; init; }
    public string sign { get; init; }
    public string buyer { get; init; }
    public ulong cash { get; init; }
    public ulong ecash { get; init; }
    public ulong pre { get; init; }
    public ulong post { get; init; }
    public ulong other { get; init; }
    public ulong total { get; init; }
}

file readonly struct InnerPosition
{
    public string name { get; init; }
    public PayMethod payment { get; init; }
    public PositionType type { get; init; }
    public byte tax { get; init; }
    public PositionUnit unit { get; init; }
    public ulong price { get; init; }
    public ulong quantity { get; init; }
    public ulong total { get; init; }
}