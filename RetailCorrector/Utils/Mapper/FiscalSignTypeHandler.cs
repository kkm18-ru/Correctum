using Correctum.Core.ValueObjects;
using Dapper;
using System.Data;

namespace RetailCorrector.Utils.Mapper
{
    public sealed class FiscalSignTypeHandler : SqlMapper.TypeHandler<FiscalSign>
    {
        public override FiscalSign Parse(object value) => (FiscalSign)(uint)(long)value;

        public override void SetValue(IDbDataParameter parameter, FiscalSign value)
        {
            parameter.DbType = DbType.UInt32;
            parameter.Value = (uint)value;
        }
    }
}
