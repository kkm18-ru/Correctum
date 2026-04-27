using Correctum.Core.Models;
using Correctum.Storage.Extensions;
using Correctum.Storage.SqlBuilder;
using Correctum.Storage.SqlBuilder.Create.Table;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.Data;

namespace Correctum.Storage;

public class Storage : IDisposable
{
    public static Storage CurrentStorage { get; private set; } = null!;
    public static ObservableCollection<Receipt> CurrentPage { get; } = [];

    public const int PageSize = 150;

    private IDbConnection _connection = null!;
    private bool _disposed = false;

    private Storage(IDbConnection conn)
    {
        _connection = conn;
    }
    private Storage() => throw new Exception();

    public static void Load(string filepath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filepath);
        IOException.ThrowIfFileNotFound(filepath);
        CurrentStorage?.Dispose();
        var connBuilder = new SqliteConnectionStringBuilder
        {
            Mode = SqliteOpenMode.ReadWrite,
            DataSource = filepath
        };
        var conn = new SqliteConnection(connBuilder.ToString());
        conn.Open();
        CurrentStorage = new Storage(conn);
    }

    public static async Task LoadAsync(string filepath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filepath);
        IOException.ThrowIfFileNotFound(filepath);
        CurrentStorage?.Dispose();
        var connBuilder = new SqliteConnectionStringBuilder
        {
            Mode = SqliteOpenMode.ReadWrite,
            DataSource = filepath
        };
        var conn = new SqliteConnection(connBuilder.ToString());
        await conn.OpenAsync(cancellationToken);
        CurrentStorage = new Storage(conn);
    }

    public static void Create(string filepath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filepath);
        CurrentStorage?.Dispose();
        if (File.Exists(filepath)) File.Delete(filepath);
        var connBuilder = new SqliteConnectionStringBuilder
        {
            Mode = SqliteOpenMode.ReadWriteCreate,
            DataSource = filepath
        };
        var conn = new SqliteConnection(connBuilder.ToString());
        conn.Open();
        conn.Execute(new CreateTableSqlBuilder<Receipt>());
        conn.Execute(new CreateTableSqlBuilder<Position>());
        CurrentStorage = new Storage(conn);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _connection.Dispose();
        }
        _connection = null!;
        _disposed = true;
    }

    ~Storage() => Dispose(disposing: false);

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
