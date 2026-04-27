namespace Correctum.Storage.SqlBuilder;

public sealed class CreateTableSqlBuilder<T> : CreateTableSqlBuilder
{
    public CreateTableSqlBuilder() : base(typeof(T)) { }
}
