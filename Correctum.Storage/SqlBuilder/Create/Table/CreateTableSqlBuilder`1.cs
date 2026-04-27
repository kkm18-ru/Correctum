namespace Correctum.Storage.SqlBuilder.Create.Table;

public sealed class CreateTableSqlBuilder<T> : CreateTableSqlBuilder
{
    public CreateTableSqlBuilder() : base(typeof(T)) { }
}
