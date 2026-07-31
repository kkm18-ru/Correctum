using Correctum.Core.Models;
using Correctum.Core.ValueObjects;
using Correctum.Models;
using Correctum.ViewModels.Windows;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.IO;
using System.Text;

namespace Correctum
{
    internal class StorageContext : DbContext
    {
        public DbSet<Receipt> Receipts => Set<Receipt>();
        public DbSet<Position> Positions => Set<Position>();
        public DbSet<Change> ChangeHistory => Set<Change>();
        public DbSet<ReceiptChangeItem> ReceiptChanges => Set<ReceiptChangeItem>();
        public DbSet<PositionChangeItem> PositionChanges => Set<PositionChangeItem>();
        public DbSet<ReceiptMetadataChangeItem> MetadataChanges => Set<ReceiptMetadataChangeItem>();
        public static string ConnectionString { get; set; }

        public const string DisplayNameFileExtension = "Хранилище Correctum";
        public const string FileExtension = ".scc";

        static StorageContext()
        {
            var connBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = Path.Combine(Path.GetTempPath(), "Correctum", $"{Guid.CreateVersion7()}{FileExtension}")
            };
            Directory.CreateDirectory(Path.GetDirectoryName(connBuilder.DataSource)!);
            ConnectionString = connBuilder.ToString();
        }

        public StorageContext()
        {
#if DEBUG
            Database.EnsureCreated();
#else
            Database.Migrate();
#endif
            Database.ExecuteSqlRaw("PRAGMA journal_mode=DELETE;");
        }

        public static void CreateBackup()
        {
            var backupDateTime = DateTime.Now;
            var connBuilder = new SqliteConnectionStringBuilder(ConnectionString);
            var originalFilePath = connBuilder.DataSource;
            var backupFilePathBuilder = new StringBuilder(originalFilePath);
            backupFilePathBuilder.Insert(
                backupFilePathBuilder.Length - FileExtension.Length, 
                backupDateTime.ToString("'_'yyyyMMddHHmmss"));
            File.Copy(originalFilePath, backupFilePathBuilder.ToString());
        }

        public static void SetContextPath(string filepath)
        {
            var connBuilder = new SqliteConnectionStringBuilder { DataSource = filepath };
            ConnectionString = connBuilder.ToString();
            ((MainWindowViewModel)App.Current.MainWindow.DataContext).UpdatePanel();
        }

        public static int ReceiptsCount
        {
            get
            {
                using var ctx = new StorageContext();
                return ctx.Receipts.Count();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlite(ConnectionString);

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<Currency>()
                .HaveConversion<CurrencyConverter>();

            configurationBuilder
                .Properties<Inn>()
                .HaveConversion<InnConverter>();

            configurationBuilder
                .Properties<FiscalSign>()
                .HaveConversion<FiscalSignConverter>();
        }
    }

    internal class InnConverter() : ValueConverter<Inn?, string?>
        (v => ToStorage(v), v => FromStorage(v))
    {
        static string? ToStorage(Inn? value) => value?.ToString();
        static Inn? FromStorage(string? value) => 
            value is null ? null : Inn.Parse(value);
    }

    internal class CurrencyConverter() : ValueConverter<Currency?, ulong?>
        (v => ToStorage(v), v => FromStorage(v))
    {
        static ulong? ToStorage(Currency? value) => (ulong?)value;
        static Currency? FromStorage(ulong? value) => (Currency?)value;
    }

    internal class FiscalSignConverter() : ValueConverter<FiscalSign?, string?>
        (v => ToStorage(v), v => FromStorage(v))
    {
        static string? ToStorage(FiscalSign? value) => value?.ToString();
        static FiscalSign? FromStorage(string? value) => 
            value is null ? null : FiscalSign.Parse(value);
    }
}
