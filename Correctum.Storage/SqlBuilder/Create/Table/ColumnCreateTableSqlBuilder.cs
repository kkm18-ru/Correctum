using Correctum.Core.ValueObjects;
using Correctum.Storage.Helpers;
using System.Reflection;
using System.Text;

namespace Correctum.Storage.SqlBuilder.Create.Table;

public class ColumnCreateTableSqlBuilder
{
    public enum ColumnDataType { NONE, INTEGER, REAL, NUMERIC, TEXT, BLOB }
    public enum PrimaryKey { False, True, AutoIncrement }
    public enum ReferenceAction { NoAction, SetNull, SetDefault, Cascade, Restrict }

    private readonly string _name;
    private readonly ColumnDataType _dataType;
    private readonly PrimaryKey _primary;
    private readonly bool _nullable;
    private (string TableName, ReferenceAction Deleted, ReferenceAction Updated)? _foreign;

    public bool Skipping => _dataType == ColumnDataType.NONE;

    public ColumnCreateTableSqlBuilder(PropertyInfo property)
    {
        _name = property.Name;
        _primary = _name == "Id" ? PrimaryKey.AutoIncrement : PrimaryKey.False;
        _nullable = ReflectionHelper.IsNullable(property);

        Type type = property.PropertyType;
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
        if (_name != "Id" && _name.EndsWith("Id"))
            _foreign = (_name[..^2], ReferenceAction.Cascade, ReferenceAction.NoAction);
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
        if(_foreign is { } foreign)
        {
            builder.Append($" REFERENCES {foreign.TableName}");
            builder.Append($" ON DELETE {ReferenceActionToString(foreign.Deleted)}");
            builder.Append($" ON UPDATE {ReferenceActionToString(foreign.Updated)}");
        }
        return builder.ToString();
    }

    private static string ReferenceActionToString(ReferenceAction action) => action switch
    {
        ReferenceAction.NoAction => "NO ACTION",
        ReferenceAction.SetNull => "SET NULL",
        ReferenceAction.SetDefault => "SET DEFAULT",
        ReferenceAction.Cascade => "CASCADE",
        ReferenceAction.Restrict => "RESTRICT",
        _ => throw new ArgumentOutOfRangeException(nameof(action))
    };
}
