using Dapper;
using Microsoft.Data.Sqlite;
using Correctum.Core.Models;
using Correctum.Core.Enums;
using Correctum.Core.ValueObjects;
using Correctum.ViewModels.Exports;

namespace Correctum.Tests.ViewModels.Exports;

public class SqliteExportViewModelTests : 
    BaseExportTest<SqliteExportViewModel, SqliteExportViewModelTests>
{
    [Test] public void Success()
    {
        var viewModel = new SqliteExportViewModel();
        var receipt = new Receipt
        {
            BuyerContact = "client@mail.ru",
            RegId = "1234567980123456",
            StorageId = "6453210987654321",
            Type = ReceiptType.Income,
            SessionId = 2,
            DocNumber = 3,
            CashSum = 6UL,
            NonCashSum = 7UL,
            AdvanceSum = 8UL,
            CreditSum = 9UL,
            ProvisionSum = 10UL,
            TotalSum = 11UL,
            Created = new DateTime(2023, 12, 30, 15, 35, 29, DateTimeKind.Unspecified),
            DocInSession = 13,
            DocSign = 16,
            TaxationSystem = TaxationSystems.Patent,
            Items = [new() {
                Name = "Товар",
                CalcMethod = CalculationMethod.FullPayment,
                Type = PositionType.OtherPayment,
                TaxRate = NDSRate.NDS122,
                Quantity = 17,
                Cost = 18UL,
                Price = 19UL,
                Unit = UnitOfMeasure.Other
            }]
        };
        var path = GenPath();
        var task = viewModel.Export(path, [receipt]);
        Assert.DoesNotThrowAsync(task.AsTask);
        using (var conn = new SqliteConnection($"Data Source={path};Pooling=False"))
        {
            conn.Open();
            var receipts = conn.Query<ReceiptDto>("SELECT * FROM receipt;").ToArray();
            Assert.That(receipts, Has.Length.EqualTo(1));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(receipts[0].id, Is.EqualTo(1));
                Assert.That(receipts[0].devid, Is.EqualTo(receipt.RegId));
                Assert.That(receipts[0].drive, Is.EqualTo(receipt.StorageId));
                Assert.That(receipts[0].shift, Is.EqualTo(receipt.SessionId));
                Assert.That(receipts[0].doc, Is.EqualTo(receipt.DocNumber));
                Assert.That(receipts[0].req, Is.EqualTo(receipt.DocInSession));
                Assert.That(receipts[0].date, Is.EqualTo(receipt.Created?.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss")));
                Assert.That(receipts[0].type, Is.EqualTo((byte)receipt.Type));
                Assert.That(receipts[0].taxation, Is.EqualTo((byte)receipt.TaxationSystem));
                Assert.That(FiscalSign.Parse(receipts[0].sign), Is.EqualTo(receipt.DocSign));
                Assert.That(receipts[0].buyer, Is.EqualTo(receipt.BuyerContact));
                Assert.That((Currency)receipts[0].cash, Is.EqualTo(receipt.CashSum));
                Assert.That((Currency)receipts[0].ecash, Is.EqualTo(receipt.NonCashSum));
                Assert.That((Currency)receipts[0].pre, Is.EqualTo(receipt.AdvanceSum));
                Assert.That((Currency)receipts[0].post, Is.EqualTo(receipt.CreditSum));
                Assert.That((Currency)receipts[0].other, Is.EqualTo(receipt.ProvisionSum));
                Assert.That((Currency)receipts[0].total, Is.EqualTo(receipt.TotalSum));
            }

            var positions = conn.Query<PositionDto>("SELECT * FROM position;").ToArray();
            Assert.That(positions, Has.Length.EqualTo(1));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(positions[0].id, Is.EqualTo(1));
                Assert.That(positions[0].name, Is.EqualTo(receipt.Items[0].Name));
                Assert.That(positions[0].payment, Is.EqualTo((byte?)receipt.Items[0].CalcMethod));
                Assert.That(positions[0].type, Is.EqualTo((byte?)receipt.Items[0].Type));
                Assert.That(positions[0].tax, Is.EqualTo((byte?)receipt.Items[0].TaxRate));
                Assert.That(positions[0].unit, Is.EqualTo((byte?)receipt.Items[0].Unit));
                Assert.That((Currency)positions[0].price, Is.EqualTo(receipt.Items[0].Price));
                Assert.That(positions[0].quantity, Is.EqualTo(receipt.Items[0].Quantity));
                Assert.That((Currency)positions[0].total, Is.EqualTo(receipt.Items[0].Cost));
                Assert.That(positions[0].receipt, Is.EqualTo(receipts[0].id));
            }
        }
        SqliteConnection.ClearAllPools();
    }

    private record ReceiptDto(long id, string devid, string drive, long shift, long doc, long req, string date, long type, long taxation, string? sign, string? buyer, long cash, long ecash, long pre, long post, long other, long total);
    private record PositionDto(long id, string name, long? payment, long? type, long? tax, long? unit, long price, long quantity, long total, long receipt);
}
