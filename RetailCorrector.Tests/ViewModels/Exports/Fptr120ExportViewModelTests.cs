using RetailCorrector.Data;
using RetailCorrector.Enums;
using RetailCorrector.ViewModels.Exports;

namespace RetailCorrector.Tests.ViewModels.Exports;

public class Fptr120ExportViewModelTests :
    BaseExportTest<Fptr120ExportViewModel, Fptr120ExportViewModelTests>
{

    #region ReceiptType

    [Test] public void CorrectReceiptTypeTests()
    {
        foreach (var (receiptType, expected) in Operations2AtolConstants())
        {
            using var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                var task = Fptr120ExportViewModel.SetReceiptTypeAsync(writer, receiptType);
                Assert.DoesNotThrowAsync(async () => await task);
            }
            stream.Position = 0;
            using (var reader = new StreamReader(stream, leaveOpen: true))
            {
                var expectedScript = $"Fptr.setParam(Fptr.LIBFPTR_PARAM_RECEIPT_TYPE, Fptr.{expected});\r\n";
                Assert.That(reader.ReadToEnd(), Is.EqualTo(expectedScript));
            }
        }
    }

    [Test] public void IncorrectReceiptTypeTests()
    {
        foreach (var receiptType in InvalidOperations())
        {
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream, leaveOpen: true);
            var task = Fptr120ExportViewModel.SetReceiptTypeAsync(writer, receiptType);
            var exc = Assert.ThrowsAsync<FormatException>(async () => await task);
            Assert.That(exc.Message, Is.EqualTo("Найден чек с некорректным типом операции."));
        }
    }

    private static IEnumerable<(Operation, string)> Operations2AtolConstants()
    {
        yield return (Operation.Income, "LIBFPTR_RT_SELL_CORRECTION");
        yield return (Operation.RefundIncome, "LIBFPTR_RT_SELL_RETURN_CORRECTION");
        yield return (Operation.Outcome, "LIBFPTR_RT_BUY_CORRECTION");
        yield return (Operation.RefundOutcome, "LIBFPTR_RT_BUY_RETURN_CORRECTION");
    }

    private static IEnumerable<Operation> InvalidOperations() => Enumerable
        .Sequence<byte>(byte.MinValue, byte.MaxValue, 1).Cast<Operation>()
        .Where(o => !Operations2AtolConstants().Select(i => i.Item1).Contains(o));

    #endregion

    #region AddCorrectionData

    [Test] public void SetCorrectionDataWithoutSignTest()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamWriter(stream, leaveOpen: true))
        {
            var task = Fptr120ExportViewModel.SetCorrectionDataAsync(writer, 0);
            Assert.DoesNotThrowAsync(async () => await task);
        }
        stream.Position = 0;
        using (var reader = new StreamReader(stream, leaveOpen: true))
        {
            var expectedScript = """
                Fptr.setParam(1173, 0);
                Fptr.setParam(1174, correctionInfo);

                """;
            Assert.That(reader.ReadToEnd(), Is.EqualTo(expectedScript));
        }
    }

    [Test] public void SetCorrectionDataWithSignTests()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamWriter(stream, leaveOpen: true))
        {
            var task = Fptr120ExportViewModel.SetCorrectionDataAsync(writer, 1);
            Assert.DoesNotThrowAsync(async () => await task);
        }
        stream.Position = 0;
        using (var reader = new StreamReader(stream, leaveOpen: true))
        {
            var expectedScript = """
            Fptr.setParam(1173, 0);
            Fptr.setParam(1174, correctionInfo);
            Fptr.setParam(1192, "1");

            """;
            Assert.That(reader.ReadToEnd(), Is.EqualTo(expectedScript));
        }
    }

    #endregion

    #region Position

    [Test] public void EmptyPositionArray()
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, leaveOpen: true);
        var task = Fptr120ExportViewModel.PositionsAsync(writer, []);
        var exc = Assert.ThrowsAsync<FormatException>(async () => await task);
        Assert.That(exc.Message, 
            Is.EqualTo("Кассовый чек должен содержать хотя бы одну позицию."));
    }

    [Test] public void EmptyPositionName()
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, leaveOpen: true);
        var task = Fptr120ExportViewModel.PositionAsync(writer, default);
        var exc = Assert.ThrowsAsync<FormatException>(async () => await task);
        Assert.That(exc.Message, 
            Is.EqualTo("Найдена позиция чека с некорректным наименованием."));
    }

    [Test] public void PositionWithoutUnit()
    {
        var position = default(Position) with 
        { 
            Name = "Товар",
        };
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, leaveOpen: true);
        var task = Fptr120ExportViewModel.PositionAsync(writer, position);
        var exc = Assert.ThrowsAsync<FormatException>(async () => await task);
        Assert.That(exc.Message, 
            Is.EqualTo("Найдена позиция чека без единицы измерения."));
    }

    [Test] public void InvalidPositionUnit()
    {
        var position = default(Position) with
        {
            Name = "Товар",
            Unit = (PositionUnit)1,
        };
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, leaveOpen: true);
        var task = Fptr120ExportViewModel.PositionAsync(writer, position);
        var exc = Assert.ThrowsAsync<FormatException>(async () => await task);
        Assert.That(exc.Message,
            Is.EqualTo("Найдена позиция чека с некорректной единицей измерения."));
    }

    [Test] public void InvalidPositionType()
    {
        var position = default(Position) with
        {
            Name = "Товар",
            Unit = PositionUnit.Piece,
        };
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, leaveOpen: true);
        var task = Fptr120ExportViewModel.PositionAsync(writer, position);
        var exc = Assert.ThrowsAsync<FormatException>(async () => await task);
        Assert.That(exc.Message,
            Is.EqualTo("Найдена позиция чека с некорректным типом."));
    }

    [Test] public void InvalidPositionCalcMethod()
    {
        var position = default(Position) with
        {
            Name = "Товар",
            Unit = PositionUnit.Piece,
            Type = PositionType.Product,
        };
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, leaveOpen: true);
        var task = Fptr120ExportViewModel.PositionAsync(writer, position);
        var exc = Assert.ThrowsAsync<FormatException>(async () => await task);
        Assert.That(exc.Message,
            Is.EqualTo("Найдена позиция чека с некорректным способом расчета."));
    }

    [Test] public void InvalidPositionTaxRate()
    {
        var position = default(Position) with
        {
            Name = "Товар",
            Unit = PositionUnit.Piece,
            Type = PositionType.Product,
            PayMethod = PayMethod.FullPrepaid,
        };
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, leaveOpen: true);
        var task = Fptr120ExportViewModel.PositionAsync(writer, position);
        var exc = Assert.ThrowsAsync<FormatException>(async () => await task);
        Assert.That(exc.Message,
            Is.EqualTo("Найдена позиция чека с некорректной ставкой НДС."));
    }

    [Test] public void CorrectPosition()
    {

        var position = new Position
        {
            Name = "Товар \"Хлеб\"",
            Unit = PositionUnit.Piece,
            Type = PositionType.Product,
            PayMethod = PayMethod.FullPayment,
            TaxRate = TaxRate.Without,
            Price = 10010,
            Quantity = 1234,
            Cost = 12300
        };
        using var stream = new MemoryStream();
        using (var writer = new StreamWriter(stream, leaveOpen: true))
        {
            var task = Fptr120ExportViewModel.PositionAsync(writer, position);
            Assert.DoesNotThrowAsync(async () => await task);
        }
        stream.Position = 0;
        using (var reader = new StreamReader(stream, leaveOpen: true))
        {
            var expectedScript = """
                Fptr.setParam(Fptr.LIBFPTR_PARAM_COMMODITY_NAME, "Товар \"Хлеб\"");
                Fptr.setParam(Fptr.LIBFPTR_PARAM_PRICE, 100.1);
                Fptr.setParam(Fptr.LIBFPTR_PARAM_QUANTITY, 1.234);
                Fptr.setParam(Fptr.LIBFPTR_PARAM_POSITION_SUM, 123);
                Fptr.setParam(Fptr.LIBFPTR_PARAM_TAX_TYPE, 6);
                Fptr.setParam(2108, 0);
                Fptr.setParam(1212, 1);
                Fptr.setParam(1214, 4);
                Fptr.registration();

                """;
            Assert.That(reader.ReadToEnd(), Is.EqualTo(expectedScript));
        }
    }

    #endregion

    #region TotalSum

    [Test] public void TotalSumTests()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamWriter(stream, leaveOpen: true))
        {
            var task = Fptr120ExportViewModel.TotalSumAsync(writer, 12345);
            Assert.DoesNotThrowAsync(async () => await task);
        }
        stream.Position = 0;
        using (var reader = new StreamReader(stream, leaveOpen: true))
        {
            var expectedScript = $"""
                Fptr.setParam(Fptr.LIBFPTR_PARAM_SUM, 123.45);
                Fptr.receiptTotal();

                """;
            Assert.That(reader.ReadToEnd(), Is.EqualTo(expectedScript));
        }
    }

    #endregion

    #region Payment

    [Test] public void PaymentTest()
    {
        var payment = new Payment
        {
            Cash = 100,
            ECash = 120,
            Pre = 0,
            Post = 12,
            Provision = 12345,
        };

        using var stream = new MemoryStream();
        using (var writer = new StreamWriter(stream, leaveOpen: true))
        {
            var task = Fptr120ExportViewModel.PaymentAsync(writer, payment);
            Assert.DoesNotThrowAsync(async () => await task);
        }
        stream.Position = 0;
        using (var reader = new StreamReader(stream, leaveOpen: true))
        {
            var expectedScript = $"""
                    Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_CASH);
                    Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, 1);
                    Fptr.payment();
                    Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_ELECTRONICALLY);
                    Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, 1.2);
                    Fptr.payment();
                    Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_CREDIT);
                    Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, 0.12);
                    Fptr.payment();
                    Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_OTHER);
                    Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, 123.45);
                    Fptr.payment();
                    
                    """;
            Assert.That(reader.ReadToEnd(), Is.EqualTo(expectedScript));
        }
    }

    #endregion

    [Test] public void CorrectReceipt()
    {
        var position1 = new Position
        {
            Name = "Хлеб",
            Unit = PositionUnit.Piece,
            Type = PositionType.Product,
            PayMethod = PayMethod.FullPayment,
            TaxRate = TaxRate.Tax20,
            Price = 5000,
            Quantity = 2000,
            Cost = 10000
        };
        var position2 = new Position
        {
            Name = "Молоко",
            Unit = PositionUnit.Liter,
            Type = PositionType.Product,
            PayMethod = PayMethod.FullPayment,
            TaxRate = TaxRate.Tax10,
            Price = 6500,
            Quantity = 1000,
            Cost = 6500
        };
        var receipt = new Receipt
        {
            ParseId = Guid.Empty,
            Created = new DateTime(2020, 12, 31, 13, 35, 59),
            BuyerContact = "client@mail.ru",
            Type = Operation.Income,
            TaxationSystem = TaxationSystem.USNIncomeExpenditure,
            DocSign = 0,
            Items = [position1, position2],
            TotalSum = 16500,
            Payment = new()
            {
                Cash = 5000,
                ECash = 11500
            }
        };

        var path = GenPath();
        var task = ViewModel.Export(path, [receipt]);
        Assert.DoesNotThrowAsync(task.AsTask);
        var fileContent = File.ReadAllText(path);
        const string expectedScript = """            
            Fptr.setParam(1178, new Date("2020-12-31T13:35:59"));
            Fptr.setParam(1179, " ");
            Fptr.utilFormTlv();
            correctionInfo = Fptr.getParamByteArray(Fptr.LIBFPTR_PARAM_TAG_VALUE);
            Fptr.setParam(Fptr.LIBFPTR_PARAM_RECEIPT_TYPE, Fptr.LIBFPTR_RT_SELL_CORRECTION);
            Fptr.setParam(1055, 4);
            Fptr.setParam(Fptr.LIBFPTR_PARAM_RECEIPT_ELECTRONICALLY, true);
            Fptr.setParam(1117, "none");
            Fptr.setParam(1008, "none");
            Fptr.setParam(1173, 0);
            Fptr.setParam(1174, correctionInfo);
            Fptr.openReceipt();
            Fptr.setParam(Fptr.LIBFPTR_PARAM_COMMODITY_NAME, "Хлеб");
            Fptr.setParam(Fptr.LIBFPTR_PARAM_PRICE, 50);
            Fptr.setParam(Fptr.LIBFPTR_PARAM_QUANTITY, 2);
            Fptr.setParam(Fptr.LIBFPTR_PARAM_POSITION_SUM, 100);
            Fptr.setParam(Fptr.LIBFPTR_PARAM_TAX_TYPE, 7);
            Fptr.setParam(2108, 0);
            Fptr.setParam(1212, 1);
            Fptr.setParam(1214, 4);
            Fptr.registration();
            Fptr.setParam(Fptr.LIBFPTR_PARAM_COMMODITY_NAME, "Молоко");
            Fptr.setParam(Fptr.LIBFPTR_PARAM_PRICE, 65);
            Fptr.setParam(Fptr.LIBFPTR_PARAM_QUANTITY, 1);
            Fptr.setParam(Fptr.LIBFPTR_PARAM_POSITION_SUM, 65);
            Fptr.setParam(Fptr.LIBFPTR_PARAM_TAX_TYPE, 2);
            Fptr.setParam(2108, 41);
            Fptr.setParam(1212, 1);
            Fptr.setParam(1214, 4);
            Fptr.registration();
            Fptr.setParam(Fptr.LIBFPTR_PARAM_SUM, 165);
            Fptr.receiptTotal();
            Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_CASH);
            Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, 50);
            Fptr.payment();
            Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_TYPE, Fptr.LIBFPTR_PT_ELECTRONICALLY);
            Fptr.setParam(Fptr.LIBFPTR_PARAM_PAYMENT_SUM, 115);
            Fptr.payment();
            Fptr.closeReceipt();
            

            """;
        Assert.That(fileContent, Is.EqualTo(expectedScript));
    }
}
