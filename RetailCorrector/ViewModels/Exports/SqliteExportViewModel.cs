using Dapper;
using Microsoft.Data.Sqlite;
using Correctum.Core.Models;
using System.Data;
using System.IO;

namespace RetailCorrector.ViewModels.Exports;

public sealed class SqliteExportViewModel : FileExportViewModel
{
    public override string ExportName => "SQLite";
    public override string Filter => "SQLite|*.sqlite";

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    private const string InitializationQuery = """
        CREATE TABLE receipt (
            id       INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            devid    TEXT    NOT NULL,
            drive    TEXT    NOT NULL,
            shift    INTEGER NOT NULL,
            doc      INTEGER NOT NULL,
            req      INTEGER NOT NULL,
            date     TEXT    NOT NULL,
            "type"   INTEGER NOT NULL,
            taxation INTEGER NOT NULL,
            sign     TEXT,
            buyer    TEXT,
            cash     INTEGER NOT NULL,
            ecash    INTEGER NOT NULL,
            pre      INTEGER NOT NULL,
            post     INTEGER NOT NULL,
            other    INTEGER NOT NULL,
            total    INTEGER NOT NULL
        );

        CREATE TABLE position (
            id       INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            name     TEXT    NOT NULL,
            payment  INTEGER,
            type     INTEGER,
            tax      INTEGER,
            unit     INTEGER,
            price    INTEGER NOT NULL,
            quantity INTEGER NOT NULL,
            total    INTEGER NOT NULL,
            receipt  INTEGER NOT NULL,
            CONSTRAINT position_receipt_FK FOREIGN KEY (receipt) REFERENCES receipt(id) ON DELETE CASCADE
        );
        """;

    public override async ValueTask Export(string filepath, IEnumerable<Receipt> receipts)
    {
        var conn = new SqliteConnection($"Data Source={filepath};Pooling=False");
        await conn.OpenAsync();
        await conn.ExecuteAsync(InitializationQuery);
        foreach (var receipt in receipts)
        {
            var tr = await conn.BeginTransactionAsync();
            try
            {
                var rid = await conn.AddReceiptAsync(receipt, tr);
                foreach (var item in receipt.Items)
                    await conn.AddPositionAsync(item, rid, tr);
                await tr.CommitAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Не удалось экспортировать чек {Id}", receipt.Id);
                await tr.RollbackAsync();
                await conn.DisposeAsync();
                File.Delete(filepath);
                ShowNotification(ex.Message, "Ошибка при экспорте чека.");
                return;
            }
        }
        await conn.DisposeAsync();
    }

    
}

file static class SqliteExportViewModelTools
{
    extension(IDbConnection conn)
    {
        public Task<uint> AddReceiptAsync(Receipt data, IDbTransaction tr)
        {
            const string Query = """
            INSERT INTO receipt (devid, drive, shift, doc, req, date, type, sign, cash, ecash, pre, post, other, total, buyer, taxation) 
            VALUES (@RegNumber, @FnSerial, @Session, @DocId, @DocInSession, @DateTime, @Operaion, @FiscalSign, @CashSum, @NonCashSum, @AdvanceSum, @CreditSum, @OtherSum, @TotalSum, @BuyerContact, @Taxation) 
            RETURNING id;
            """;
            var parameters = new DynamicParameters();
            parameters.Add("@RegNumber", data.RegId, DbType.String);
            parameters.Add("@FnSerial", data.StorageId, DbType.String);
            parameters.Add("@Session", data.SessionId, DbType.UInt32);
            parameters.Add("@DocInSession", data.DocInSession, DbType.UInt32);
            parameters.Add("@DocId", data.DocNumber, DbType.UInt32);
            parameters.Add("@DateTime", data.Created.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"), DbType.String);
            parameters.Add("@Operaion", (uint)data.Type, DbType.UInt32);
            parameters.Add("@BuyerContact", string.IsNullOrWhiteSpace(data.BuyerContact) ? null : data.BuyerContact, DbType.String);
            parameters.Add("@Taxation", (uint)data.TaxationSystem, DbType.UInt32);
            parameters.Add("@FiscalSign", data.DocSign?.ToString(), DbType.String);
            parameters.Add("@CashSum", (ulong)data.Payment.Cash, DbType.UInt64);
            parameters.Add("@NonCashSum", (ulong)data.Payment.ECash, DbType.UInt64);
            parameters.Add("@AdvanceSum", (ulong)data.Payment.Pre, DbType.UInt64);
            parameters.Add("@CreditSum", (ulong)data.Payment.Post, DbType.UInt64);
            parameters.Add("@OtherSum", (ulong)data.Payment.Provision, DbType.UInt64);
            parameters.Add("@TotalSum", (ulong)data.TotalSum, DbType.UInt64);
            return conn.QuerySingleAsync<uint>(Query, parameters, tr);
        }
        public Task AddPositionAsync(Position data, uint receiptId, IDbTransaction tr)
        {
            const string Query = """
            INSERT INTO position (name, payment, type, tax, unit, price, quantity, total, receipt) 
            VALUES (@Name, @CalculationMethod, @Type, @TaxRate, @Unit, @Price, @Quantity, @Cost, @ReceiptId);
            """;
            var parameters = new DynamicParameters();
            parameters.Add("@Name", data.Name, DbType.String);
            parameters.Add("@CalculationMethod", (uint?)data.PayMethod, DbType.UInt32);
            parameters.Add("@Type", (uint?)data.Type, DbType.UInt32);
            parameters.Add("@TaxRate", (uint?)data.TaxRate, DbType.UInt32);
            parameters.Add("@Unit", (uint?)data.Unit, DbType.UInt32);
            parameters.Add("@Price", (ulong)data.Price, DbType.UInt64);
            parameters.Add("@Quantity", data.Quantity, DbType.UInt64);
            parameters.Add("@Cost", (ulong)data.Cost, DbType.UInt64);
            parameters.Add("@ReceiptId", receiptId, DbType.UInt32);
            return conn.ExecuteAsync(Query, parameters, tr);
        }
    }
}
