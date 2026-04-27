using Correctum.Core.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows.Data;

namespace RetailCorrector;

public sealed class Storage : IDisposable
{
    public static Storage Instance { get; private set; } = null!;
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public IEnumerable<Receipt> AllReceipts
    {
        get
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            var receipts = _conn.Query<Receipt>("SELECT * FROM Receipt;");
            var positions = _conn.Query<Position>("SELECT * FROM Position;");
            return receipts.Select(r => r with
            {
                Items = [.. positions.Where(p => p.ReceiptId == r.Id)]
            });
        }
    }

    public static ObservableCollection<Receipt> CurrentPage { get; } = [];
    public static ICollectionView ReceiptView { get; } = 
        CollectionViewSource.GetDefaultView(CurrentPage);

    private readonly IDbConnection _conn;
    private bool disposed = false;

    private Storage(IDbConnection connection)
    {
        _conn = connection;
        UpdateReceiptData(1, 100);
    }

    public static void Load(string filepath)
    {
        if (!File.Exists(filepath))
            throw new FileNotFoundException(null, filepath);
        Instance?.Dispose();
        var connBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = filepath,
            Mode = SqliteOpenMode.ReadWrite
        };
        var conn = new SqliteConnection(connBuilder.ToString());
        conn.Open();
        Instance = new Storage(conn);
    }

    public static void Create(string filepath)
    {
        Instance?.Dispose();
        if (File.Exists(filepath))
            File.Delete(filepath);
        var connBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = filepath,
            Mode = SqliteOpenMode.ReadWriteCreate
        };
        var conn = new SqliteConnection(connBuilder.ToString());
        conn.Open();
        conn.Execute(StorageConstants.CreateReceiptTable);
        conn.Execute(StorageConstants.CreatePositionTable);
        conn.Execute(StorageConstants.CreateIndexes);
        Instance = new Storage(conn);
    }

    public void UpdateReceiptData(uint page, uint countRecords)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        CurrentPage.Clear();
        Receipt[] receipts = [.. _conn.Query<Receipt>(
            "SELECT * FROM Receipt LIMIT @limit OFFSET @offset;",
            new { offset = countRecords * (page - 1), limit = countRecords })];
        if(receipts.Length == 0) return;
        var positions = _conn.Query<Position>(
            "SELECT * FROM Position WHERE ReceiptId BETWEEN @startRange AND @endRange;",
            new { startRange = receipts[0].Id, endRange = receipts[^1].Id });
        var grouppedPositions = positions.GroupBy(p => p.ReceiptId)
            .ToDictionary(g => g.Key, g => g.ToArray());
        foreach (var receipt in receipts)
            CurrentPage.Add(receipt with { Items = grouppedPositions[receipt.Id] });
    }

    public bool AddReceipt(Receipt receipt)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        var tr = _conn.BeginTransaction();
        try
        {
            var id = _conn.QuerySingle<int>(StorageConstants.AddReceiptQuery, receipt, tr);
            foreach (var position in receipt.Items ?? [])
                _conn.Execute(StorageConstants.AddPositionQuery, new
                {
                    position.Name,
                    position.Type,
                    position.CalcMethod,
                    position.TaxRate,
                    position.Unit,
                    position.Price,
                    position.Quantity,
                    position.Cost,
                    ReceiptId = id,
                }, tr);
            tr.Commit();
            return true;
        }
        catch(Exception e)
        {
            tr.Rollback();
            Logger.Error(e, "Не удалось добавить чек в хранилище.");
            return false;
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(true);
    }

    ~Storage() => Dispose(false);

    private void Dispose(bool disposing)
    {
        if (disposed) return;
        if (disposing)
        {
            _conn.Dispose();
        }
        disposed = true;
    }
}

file static class StorageConstants
{
    public const string AddReceiptQuery = """
        INSERT INTO Receipt (
            RegId, StorageId, DocNumber, SessionId, DocInSession, Created, DocSign, Type, CashSum, NonCashSum, AdvanceSum, CreditSum, ProvisionSum, TotalSum, BuyerContact, TaxationSystem
        ) VALUES (
            @RegId, @StorageId, @DocNumber, @SessionId, @DocInSession, @Created, @DocSign, @Type, @CashSum, @NonCashSum, @AdvanceSum, @CreditSum, @ProvisionSum, @TotalSum, @BuyerContact, @TaxationSystem
        ) RETURNING Id;
        """;

    public const string AddPositionQuery = """
        INSERT INTO Position (                
            Name, Type, CalcMethod, TaxRate, Unit, Price, Quantity, Cost, ReceiptId 
        ) VALUES (
            @Name, @Type, @CalcMethod, @TaxRate, @Unit, @Price, @Quantity, @Cost, @ReceiptId 
        );
        """;

    public const string CreateReceiptTable = """
        CREATE TABLE Receipt (
            Id             INTEGER PRIMARY KEY AUTOINCREMENT,
            RegId          TEXT,
            StorageId      TEXT,
            DocNumber      INTEGER,
            SessionId      INTEGER,
            DocInSession   INTEGER,
            Created        TEXT,
            DocSign        INTEGER,
            Type           INTEGER,
            CashSum        INTEGER,
            NonCashSum     INTEGER,
            AdvanceSum     INTEGER,
            CreditSum      INTEGER,
            ProvisionSum   INTEGER,
            TotalSum       INTEGER,
            BuyerContact   TEXT,
            TaxationSystem INTEGER
        );
        """;

    public const string CreatePositionTable = """
        CREATE TABLE Position (
            Id         INTEGER PRIMARY KEY AUTOINCREMENT,
            Name       TEXT,
            Type       INTEGER,
            CalcMethod INTEGER,
            TaxRate    INTEGER,
            Unit       INTEGER,
            Price      INTEGER,
            Quantity   REAL,
            Cost       INTEGER,
            ReceiptId  INTEGER REFERENCES Receipt (Id) ON DELETE CASCADE
        );
        """;

    public const string CreateIndexes = """
        CREATE INDEX IF NOT EXISTS FK_Position_ReceiptId ON Position (ReceiptId);
        """;
}
