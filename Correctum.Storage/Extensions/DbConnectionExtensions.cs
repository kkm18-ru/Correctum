using Correctum.Storage.SqlBuilder;
using System.Data;

namespace Correctum.Storage.Extensions;

public static class DbConnectionExtensions
{
    extension(IDbConnection conn)
    {
        public void Execute(ISqlBuilder sqlBuilder)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sqlBuilder.ToString();
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
        }

        public void Execute(ISqlBuilder sqlBuilder, IDbTransaction tr)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sqlBuilder.ToString();
            cmd.CommandType = CommandType.Text;
            cmd.Transaction = tr;
            cmd.ExecuteNonQuery();
        }
    }
}
