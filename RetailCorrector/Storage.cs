using Correctum.Core.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Windows.Controls;

namespace RetailCorrector
{
    public class Storage : IDisposable
    {
        public static Storage Instance { get; private set; }

        public IEnumerable<Receipt> Receipts
        {
            get
            {
                var receipts = _conn.Query<Receipt>("SELECT * FROM Receipt;");
                var positions = _conn.Query<Position>("SELECT * FROM Position;");
                return receipts.Select(r => r with
                {
                    Items = [.. positions.Where(p => p.ReceiptId == r.Id)]
                });
            }
        }
        public static ObservableCollection<Receipt> ReceiptView { get; } = [];

        private IDbConnection _conn;

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
            const string sql = """
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
                CREATE INDEX IF NOT EXISTS FK_Position_ReceiptId ON Position (ReceiptId);
                """;
            conn.Execute(sql);
            Instance = new Storage(conn);
        }

        public void UpdateReceiptData(uint page, uint countRecords)
        {
            ReceiptView.Clear();
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
                ReceiptView.Add(receipt with { Items = grouppedPositions[receipt.Id] });
        }

        public void AddReceipt(Receipt receipt)
        {
            const string sql = """
                INSERT INTO Receipt (
                    RegId, StorageId, DocNumber, SessionId, DocInSession, Created, DocSign, Type, CashSum, NonCashSum, AdvanceSum, CreditSum, ProvisionSum, TotalSum, BuyerContact, TaxationSystem
                ) VALUES (
                    @RegId, @StorageId, @DocNumber, @SessionId, @DocInSession, @Created, @DocSign, @Type, @CashSum, @NonCashSum, @AdvanceSum, @CreditSum, @ProvisionSum, @TotalSum, @BuyerContact, @TaxationSystem
                ) RETURNING Id;
                """;
            var tr = _conn.BeginTransaction();
            try
            {
                var id = _conn.QuerySingle<int>(sql, receipt, tr);
                foreach (var position in receipt.Items ?? [])
                    AddPosition(position, id, tr);
                tr.Commit();
            }
            catch(Exception e)
            {
                tr.Rollback();
                throw e;
            }
        }

        private void AddPosition(Position position, int id, IDbTransaction tr)
        {
            const string sql = """
                INSERT INTO Position (                
                    Name, Type, CalcMethod, TaxRate, Unit, Price, Quantity, Cost, ReceiptId 
                ) VALUES (
                    @Name, @Type, @CalcMethod, @TaxRate, @Unit, @Price, @Quantity, @Cost, @ReceiptId 
                );
                """;

            _conn.Execute(sql, new
            {
                ReceiptId = id,
                position.Name,
                position.Type,
                position.CalcMethod,
                position.TaxRate,
                position.Unit,
                position.Price,
                position.Quantity,
                position.Cost
            }, tr);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _conn.Dispose();
        }
    }
}
