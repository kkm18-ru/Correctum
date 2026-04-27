using System.Data;
using System.Text;

namespace Correctum.Storage.SqlBuilder.Create.Table;

public class CreateTableSqlBuilder : ISqlBuilder
{
    protected string _tableName;
    protected List<ColumnCreateTableSqlBuilder> _columns = [];

    public CreateTableSqlBuilder(Type type)
    {
        _tableName = type.Name;
        foreach (var property in type.GetProperties())
            _columns.Add(new ColumnCreateTableSqlBuilder(property));
    }

    public CreateTableSqlBuilder SetTableName(string tableName)
    {
        _tableName = tableName;
        return this;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"CREATE TABLE {_tableName} (");
        builder.Append("    ");
        builder.AppendJoin(", \n    ", _columns.Where(c => !c.Skipping));
        builder.AppendLine();
        builder.Append(");");
        return builder.ToString();
    }
}
