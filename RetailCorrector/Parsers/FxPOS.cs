using Microsoft.Data.Sqlite;
using RetailCorrector.Data;
using Dapper;

namespace RetailCorrector.Parsers
{
    internal partial class FxPOS : ParserBase<FxPOS, FxPOS.View>
    {
        public override async IAsyncEnumerable<Receipt> Parse(CancellationToken cancellationToken)
        {
            using var conn = new SqliteConnection($"Data Source={Path};Mode=ReadOnly");
            var count = await conn.ExecuteScalarAsync<int>(COUNT_RECEIPTS);
            for(var i = 0; i < count; i+= LIMIT)
            {
                var docs = await conn.QueryAsync<Document>(GET_RECEIPTS, new
                {
                    limit = LIMIT,
                    offset = i
                });
                foreach(var doc in docs)
                {

                }
            }
        }
    }
}
