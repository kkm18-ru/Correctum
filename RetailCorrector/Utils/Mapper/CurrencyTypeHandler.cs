using Correctum.Core.ValueObjects;
using Dapper;
using System.Data;

namespace RetailCorrector.Utils.Mapper
{
    public sealed class CurrencyTypeHandler : SqlMapper.TypeHandler<Currency>
    {
        public override Currency Parse(object value) => (Currency)(long)value;

        public override void SetValue(IDbDataParameter parameter, Currency value)
        {
            parameter.DbType = DbType.UInt64;
            parameter.Value = (ulong)value;
        }
    }
}
