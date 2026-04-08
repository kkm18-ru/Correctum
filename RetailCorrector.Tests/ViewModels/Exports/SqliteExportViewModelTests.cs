using Dapper;
using Microsoft.Data.Sqlite;
using RetailCorrector.Data;
using RetailCorrector.Enums;
using RetailCorrector.ViewModels.Exports;

namespace RetailCorrector.Tests.ViewModels.Exports;

public class SqliteExportViewModelTests : 
    BaseExportTest<SqliteExportViewModel, SqliteExportViewModelTests>
{
    [Test] public void Success()
    {
        var viewModel = new SqliteExportViewModel();
        var receipt = new Receipt
        {
            ParseId = Guid.Empty,
            BuyerContact = "client@mail.ru",
            RegId = "1234567980123456",
            StorageId = "6453210987654321",
            Type = Operation.Income,
            SessionId = 2,
            DocNumber = 3,
            EditsCount = 5,
            Payment = new()
            {
                Cash = 6,
                ECash = 7,
                Pre = 8,
                Post = 9,
                Provision = 10,
            },
            TotalSum = 11,
            Created = new DateTime(2023,12,30,15,35,29),
            DocInSession = 13,
            DocSign = 16,
            TaxationSystem = TaxationSystem.Patent,
            Items = [new() {
                Name = "Товар",
                PayMethod = PayMethod.FullPayment,
                Type = PositionType.OtherPayment,
                TaxRate = TaxRate.Calc22,
                Quantity = 17,
                Cost = 18,
                Price = 19,
                Unit = PositionUnit.Other
            }]
        };
        var path = GenPath();
        var task = viewModel.Export(path, [receipt]);
        Assert.DoesNotThrowAsync(task.AsTask);
        using (var conn = new SqliteConnection($"Data Source={path};Pooling=False"))
        {
            conn.Open();
            var receipts = conn.Query<receipt>("SELECT * FROM receipt;").ToArray();
            Assert.That(receipts.Length, Is.EqualTo(1));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(receipts[0].id, Is.EqualTo(1));
                Assert.That(receipts[0].devid, Is.EqualTo(receipt.RegId));
                Assert.That(receipts[0].drive, Is.EqualTo(receipt.StorageId));
                Assert.That(receipts[0].shift, Is.EqualTo(receipt.SessionId));
                Assert.That(receipts[0].doc, Is.EqualTo(receipt.DocNumber));
                Assert.That(receipts[0].req, Is.EqualTo(receipt.DocInSession));
                Assert.That(receipts[0].date, Is.EqualTo(receipt.Created.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss")));
                Assert.That(receipts[0].type, Is.EqualTo((byte)receipt.Type));
                Assert.That(receipts[0].taxation, Is.EqualTo((byte)receipt.TaxationSystem));
                Assert.That(receipts[0].sign, Is.EqualTo(receipt.DocSign.ToString()));
                Assert.That(receipts[0].buyer, Is.EqualTo(receipt.BuyerContact));
                Assert.That(receipts[0].cash, Is.EqualTo(receipt.Payment.Cash));
                Assert.That(receipts[0].ecash, Is.EqualTo(receipt.Payment.ECash));
                Assert.That(receipts[0].pre, Is.EqualTo(receipt.Payment.Pre));
                Assert.That(receipts[0].post, Is.EqualTo(receipt.Payment.Post));
                Assert.That(receipts[0].other, Is.EqualTo(receipt.Payment.Provision));
                Assert.That(receipts[0].total, Is.EqualTo(receipt.TotalSum));
            }

            var positions = conn.Query<position>("SELECT * FROM position;").ToArray();
            Assert.That(positions.Length, Is.EqualTo(1));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(positions[0].id, Is.EqualTo(1));
                Assert.That(positions[0].name, Is.EqualTo(receipt.Items[0].Name));
                Assert.That(positions[0].payment, Is.EqualTo((byte?)receipt.Items[0].PayMethod));
                Assert.That(positions[0].type, Is.EqualTo((byte?)receipt.Items[0].Type));
                Assert.That(positions[0].tax, Is.EqualTo((byte?)receipt.Items[0].TaxRate));
                Assert.That(positions[0].unit, Is.EqualTo((byte?)receipt.Items[0].Unit));
                Assert.That(positions[0].price, Is.EqualTo(receipt.Items[0].Price));
                Assert.That(positions[0].quantity, Is.EqualTo(receipt.Items[0].Quantity));
                Assert.That(positions[0].total, Is.EqualTo(receipt.Items[0].Cost));
                Assert.That(positions[0].receipt, Is.EqualTo(receipts[0].id));
            }
        }
        SqliteConnection.ClearAllPools();
    }

    private record receipt(long id, string devid, string drive, long shift, long doc, long req, string date, long type, long taxation, string? sign, string? buyer, long cash, long ecash, long pre, long post, long other, long total);
    private record position(long id, string name, long? payment, long? type, long? tax, long? unit, long price, long quantity, long total, long receipt);
}
