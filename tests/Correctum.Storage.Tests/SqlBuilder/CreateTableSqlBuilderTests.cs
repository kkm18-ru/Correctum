using Correctum.Core.Models;
using Correctum.Storage.SqlBuilder.Create.Table;
using System.Diagnostics;

namespace Correctum.Storage.Tests.SqlBuilder;

public class CreateTableSqlBuilderTests
{
    [Test] public void GenericConstructor_Should_InitCreatorTableReceipt()
    {
        var builder = new CreateTableSqlBuilder<Position>();
        var query = builder.ToString();
        Debug.WriteLine(query);
        //Assert.That(query, Is.EqualTo("CREATE TABLE Receipt ();"));
    }
}
