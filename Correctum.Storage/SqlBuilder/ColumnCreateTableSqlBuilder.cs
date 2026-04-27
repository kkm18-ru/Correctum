using Correctum.Core.ValueObjects;
using Correctum.Storage.Helpers;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Correctum.Storage.SqlBuilder;

public class ColumnCreateTableSqlBuilder
{
    public enum ColumnDataType { NONE, INTEGER, REAL, NUMERIC, TEXT, BLOB }
    public enum PrimaryKey { False, True, AutoIncrement }

    private readonly string _name;
    private readonly ColumnDataType _dataType;
    private readonly PrimaryKey _primary;
    private readonly bool _nullable;

    public bool Skipping => _dataType == ColumnDataType.NONE;

    public ColumnCreateTableSqlBuilder(string name, ColumnDataType dataType, PrimaryKey? primary = null)
    {
        _name = name;
        _dataType = dataType;
        _primary = primary ?? (name == "Id" ? PrimaryKey.AutoIncrement : PrimaryKey.False);
    }

    public ColumnCreateTableSqlBuilder(PropertyInfo property)
    {
        _name = property.Name;
        _primary = _name == "Id" ? PrimaryKey.AutoIncrement : PrimaryKey.False;
        Type type = property.PropertyType;
        _nullable = ReflectionHelper.IsNullable(property);
        if (type.IsValueType && _nullable)
            type = Nullable.GetUnderlyingType(type)!;

        _dataType = type switch
        {
            Type t when t == typeof(byte)     || t == typeof(sbyte)      ||
                        t == typeof(ushort)   || t == typeof(short)      ||
                        t == typeof(uint)     || t == typeof(int)        ||
                        t == typeof(ulong)    || t == typeof(long)       ||
                        t == typeof(Currency) || t == typeof(FiscalSign) ||
                        t == typeof(DateTime) || t.IsEnum
                 => ColumnDataType.INTEGER,

            Type t when t == typeof(float) || t == typeof(double) || t == typeof(decimal)
                => ColumnDataType.REAL,

            Type t when t == typeof(byte[]) => ColumnDataType.BLOB,

            Type t when t == typeof(string) => ColumnDataType.TEXT,

            _ => ColumnDataType.NONE
        };
    }

    public override string ToString()
    {
        var builder = new StringBuilder($"{_name} {_dataType:F}");
        if (!_nullable) builder.Append(" NOT NULL");
        if (_primary != PrimaryKey.False)
        {
            builder.Append(" PRIMARY KEY");
            if (_primary == PrimaryKey.AutoIncrement)
                builder.Append(" AUTOINCREMENT");
        }
        return builder.ToString();
    }
}
