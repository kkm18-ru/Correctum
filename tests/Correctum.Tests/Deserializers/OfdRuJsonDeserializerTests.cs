using Correctum.Core.Models;
using Correctum.Deserializers;
using Correctum.Utils.Extensions;
using System.Text.Json;
using static Correctum.Tests.Utils.BogusHelper;

namespace Correctum.Tests.Deserializers;

public class OfdRuJsonDeserializerTests : 
    BaseDeserializerTests<OfdRuJsonDeserializer>
{
    protected override string GenerateText(Receipt receipt) =>
        JsonSerializer.Serialize(new Ticket(receipt));

    private readonly struct Ticket(Receipt doc)
    {
        public int Version { get; } = 3;
        public string DocumentFormat { get; } = "1.2";
        public Document Document { get; } = new(doc);
        public int Tag { get; } = 3;
        public string UserInn { get; } = SharedFaker.Random.UInt().ToString("D10");
        public string KktRegNumber { get; } = doc.RegId!;
        public string FnNumber { get; } = doc.StorageId!;
        public uint DocNumber { get; } = doc.DocNumber!.Value;
        public string DocDateTime { get; } = doc.Created!.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
        public string DocFiscalSign { get; } = FiscalHelper.ToStringSign(uint.Parse(doc.DocSign!.Value.ToString()));
        public string DecimalFiscalSign { get; } = doc.DocSign!.Value.ToString();
        public string CDateUtc { get; } = SharedFaker.Date.Past().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
    }

    private readonly struct Document(Receipt doc)
    {
        public int Tag { get; } = 3;
        public string UserInn { get; } = SharedFaker.Random.UInt().ToString("D10");
        public uint Number { get; } = doc.DocInSession!.Value;
        public string DateTime { get; } = doc.Created!.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
        public uint ShiftNumber { get; } = doc.SessionId!.Value;
        public int OperationType { get; } = (int)doc.Type!.Value;
        public int TaxationType { get; } = (int)doc.TaxationSystem!.Value;
        public string Operator { get; } = SharedFaker.Name.FullName();
        public string KKT_RegNumber { get; } = doc.RegId!;
        public string FN_FactoryNumber { get; } = doc.StorageId!;
        public Item[] Items { get; } = [.. doc.Items!.Select(i => new Item(i))];
        public string RetailPlaceAddress { get; } = SharedFaker.Address.FullAddress();
        public ulong Amount_Total { get; } = (ulong)doc.TotalSum!.Value;
        public ulong Amount_Cash { get; } = (ulong)doc.Payment!.Value.CashSum!.Value;
        public ulong Amount_ECash { get; } = (ulong)doc.Payment!.Value.NonCashSum!.Value;
        public uint Document_Number { get; } = doc.DocNumber!.Value;
        public string FiscalSign { get; } = FiscalHelper.ToStringSign(uint.Parse(doc.DocSign!.Value.ToString()));
        public string DecimalFiscalSign { get; } = doc.DocSign!.Value.ToString();
        public int Format_Version { get; } = 4;
        public ulong Amount_Advance { get; } = (ulong)doc.Payment!.Value.AdvanceSum!.Value;
        public ulong Amount_Loan { get; } = (ulong)doc.Payment!.Value.CreditSum!.Value;
        public ulong Amount_Granting { get; } = (ulong)doc.Payment!.Value.ProvisionSum!.Value;
        public string Calculation_Place { get; } = SharedFaker.Commerce.Department();
    }

    private readonly struct Item(Position pos)
    {
        public string Name { get; } = pos.Name!;
        public ulong Price { get; } = (ulong)pos.Price!.Value;
        public decimal Quantity { get; } = pos.Quantity!.Value * 0.001M;
        public ulong Total { get; } = (ulong)pos.Cost!.Value;
        public int SubjectType { get; } = (int)pos.Type!.Value;
        public int CalculationMethod { get; } = (int)pos.CalcMethod!.Value;
        public int NDS_Rate { get; } = pos.TaxRate!.Value.ToFns();
        public int ProductUnitOfMeasure { get; } = (int)pos.Unit!.Value;
    }
}
