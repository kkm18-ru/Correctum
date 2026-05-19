using Correctum.Core.Models;
using Correctum.Core.ValueObjects;
using Correctum.ViewModels.Windows;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.IO;

namespace Correctum
{    
    internal class StorageContext : DbContext
    {
        public DbSet<Receipt> Receipts => Set<Receipt>();
        public DbSet<Position> Positions => Set<Position>();
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

        public StorageContext() => Database.EnsureCreated();

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
