using Correctum.Core.Models;
using Correctum.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Correctum
{    
    internal class StorageContext : DbContext
    {
        public DbSet<Receipt> Receipts => Set<Receipt>();
        public DbSet<Position> Positions => Set<Position>();

        public StorageContext() => Database.EnsureCreated();

        public static int ReceiptsCount
        {
            get
            {
                using var ctx = new StorageContext();
                return ctx.Receipts.Count();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=helloapp.db");
        }

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
