using System.Globalization;
using System.Text;
using Bogus;
using Newtonsoft.Json.Linq;
using Correctum.Core.Models;
using Correctum.Core.Enums;
using Correctum.Core.ValueObjects;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Correctum.Tests.ApiClients.Taxcom;

partial class Taxcom
{
    private static object documentInfo = null!;
    private static uint documentNumber;
    
    private static void InitValuesDocumentInfo(Faker faker)
    {
        documentNumber = faker.Random.UInt(1);
        documentInfo = new Faker<DocumentInfo>()
            .RuleFor(o => o.reportDate, (f, _) => f.Date.Past())
            .RuleFor(o => o.documentFormatVersion, (_, _) => "1.2")
            .RuleFor(o => o.documentType, (_, _) => "3")
            .RuleFor(o => o.documentFormatDate, (_, _) => "21.03.2017")
            .RuleFor(o => o.document, GenerateTags)
            .Generate();
    }

    private static JObject GenerateTags(Faker faker, object info)
    {
        var positions = new JArray();
        for (var i = 0; i < faker.Random.Int(2, 5); i++)
        {
            positions.Add(new JObject
            {
                { "1030", new JValue(faker.Commerce.ProductName()) },
                { "1079", new JValue(faker.Random.Int(1)) },
                { "1043", new JValue(faker.Random.Int(1)) },
                { "1023", new JValue(faker.Random.Double(1,5).ToString("F3", CultureInfo.InvariantCulture)+"000") },
                { "1199", new JValue((int)faker.Random.Enum<NDSRate>()) },
                { "2108", new JValue((int)faker.Random.Enum<UnitOfMeasure>()) },
                { "1214", new JValue((int)faker.Random.Enum<CalculationMethod>()) },
                { "1212", new JValue((int)faker.Random.Enum<PositionType>()) }
            });
        }
        return new JObject
        {
            { "1008", new JValue(faker.Internet.Email()) },                                                 // телефон или электронный адрес покупателя
            { "1009", new JValue(faker.Address.FullAddress()) },                                            // адрес расчетов
            { "1012", new JValue(faker.Date.Past().ToString("s")) },                                                      // дата, время
            { "1018", new JValue(faker.Random.ULong(10000_00000, 99999_99999).ToString("D10")) },    // ИНН пользователя
            { "1020", new JValue(faker.Random.UInt()) },                                                    // сумма расчета, указанного в чеке (БСО)
            { "1021", new JValue(faker.Name.LastName()) },                                                  // кассир
            { "1031", new JValue(faker.Random.UInt()) },                                                    // сумма по чеку (БСО) наличными
            { "1037", new JValue(faker.Random.ULong().ToString("D16")) },                            // регистрационный номер ККТ
            { "1038", new JValue(faker.Random.UInt()) },                                                    // номер смены
            { "1040", new JValue(faker.Random.UInt()) },                                                    // номер ФД
            { "1041", new JValue(faker.Random.ULong().ToString("D16")) },                            // номер ФН
            { "1042", new JValue(faker.Random.UInt()) },                                                    // номер чека за смену
            { "1048", new JValue(faker.Name.JobTitle()) },                                                  // наименование пользователя
            { "1054", new JValue((int)faker.Random.Enum<ReceiptType>()) },                                    // признак расчета
            { "1055", new JValue(faker.Random.Int(1,63)) },                                                 // применяемая система налогообложения
            { "1059", new JArray(positions) },                                                              // предмет расчета
            { "1060", new JValue("www.nalog.gov.ru") },                                                     // адрес сайта ФНС
            { "1077", new JValue(FiscalHelper.ToStringSign(faker.Random.UInt())) },                         // ФПД
            { "1081", new JValue(faker.Random.UInt()) },                                                    // сумма по чеку (БСО) безналичными
            { "1102", new JValue(faker.Random.UInt()) },                                                    // сумма НДС чека по ставке 20%
            { "1103", new JValue(faker.Random.UInt()) },                                                    // сумма НДС чека по ставке 10%
            { "1104", new JValue(faker.Random.UInt()) },                                                    // сумма расчета по чеку с НДС по ставке 0%
            { "1105", new JValue(faker.Random.UInt()) },                                                    // сумма расчета по чеку без НДС
            { "1106", new JValue(faker.Random.UInt()) },                                                    // сумма НДС чека по расч. ставке 20/120
            { "1107", new JValue(faker.Random.UInt()) },                                                    // сумма НДС чека по расч. ставке 10/110
            { "1187", new JValue(faker.Name.JobTitle()) },                                                  // место расчетов
            { "1209", new JValue(4) },                                                                      // номер версии ФФД
            { "1215", new JValue(faker.Random.UInt()) },                                                    // сумма по чеку (БСО) предоплатой (зачетом аванса и (или) предыдущих платежей)
            { "1216", new JValue(faker.Random.UInt()) },                                                    // сумма постоплатой (кредит)
            { "1217", new JValue(faker.Random.UInt()) },                                                    // сумма встречным предоставлением
        };
    }
    
    [Test] public async Task FetchReceipt_Success()
    {
        _server.Given(Request.Create().UsingGet().WithHeader("Session-Token", token).WithParam("fn", fnNumber)
                .WithPath("/API/v2/DocumentInfo").WithParam("fd", documentNumber.ToString()))
            .RespondWith(Response.Create().WithBodyAsJson(documentInfo, Encoding.UTF8));
        _client.SessionToken = token;
        var doc = await _client.FetchReceiptAsync(fnNumber, documentNumber);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(doc, Is.Not.Null);
            var receipt = (Receipt)doc;
            var document = ((DocumentInfo)documentInfo).document;
            Assert.That(receipt.RegId, Is.EqualTo(document["1037"]?.Value<string>()));
            Assert.That(receipt.StorageId, Is.EqualTo(document["1041"]?.Value<string>()));
            Assert.That(receipt.DocInSession, Is.EqualTo(document["1042"]?.Value<uint>()));
            Assert.That(receipt.SessionId, Is.EqualTo(document["1038"]?.Value<uint>()));
            Assert.That(receipt.DocNumber, Is.EqualTo(document["1040"]?.Value<uint>()));
            Assert.That(receipt.DocSign, Is.EqualTo(FiscalSign.Parse(document["1077"]?.Value<string>())));
            Assert.That(receipt.Created?.ToString("s"), Is.EqualTo(document["1012"]?.Value<string>()));
            Assert.That((int?)receipt.Type, Is.EqualTo(document["1054"]?.Value<int>()));
            Assert.That((ulong?)receipt.CashSum, Is.EqualTo(document["1031"]?.Value<ulong>()));
            Assert.That((ulong?)receipt.NonCashSum, Is.EqualTo(document["1081"]?.Value<ulong>()));
            Assert.That((ulong?)receipt.AdvanceSum, Is.EqualTo(document["1215"]?.Value<ulong>()));
            Assert.That((ulong?)receipt.CreditSum, Is.EqualTo(document["1216"]?.Value<ulong>()));
            Assert.That((ulong?)receipt.ProvisionSum, Is.EqualTo(document["1217"]?.Value<ulong>()));
            Assert.That((ulong?)receipt.TotalSum, Is.EqualTo(document["1020"]?.Value<ulong>()));
            Assert.That(receipt.BuyerContact, Is.EqualTo(document["1008"]?.Value<string>()));
            Assert.That((int?)receipt.TaxationSystem, Is.EqualTo(document["1055"]?.Value<int>()));
            Assert.That(receipt.Items, Has.Length.EqualTo(((JArray?)document["1059"])?.Count));
            for (var i = 0; i < receipt.Items.Length; i++)
            {
                var position = receipt.Items[i];
                var posObject = document["1059"]?[i]!;
                Assert.That(position.Name, Is.EqualTo(posObject["1030"]?.Value<string>()));
                Assert.That((ulong?)position.Price, Is.EqualTo(posObject["1079"]?.Value<ulong>()));
                Assert.That(position.Quantity?.ToString("F6", CultureInfo.InvariantCulture), 
                    Is.EqualTo(posObject["1023"]?.Value<string>()));
                Assert.That((ulong?)position.Cost, Is.EqualTo(posObject["1043"]?.Value<ulong>()));
                Assert.That((int?)position.Unit, Is.EqualTo(posObject["2108"]?.Value<int>()));
                Assert.That((int?)position.CalcMethod, Is.EqualTo(posObject["1214"]?.Value<int>()));
                Assert.That((int?)position.Type, Is.EqualTo(posObject["1212"]?.Value<int>()));
                Assert.That((int?)position.TaxRate, Is.EqualTo(posObject["1199"]?.Value<int>()));
            }
        }
    }
}

file class DocumentInfo
{
    public DateTime reportDate { get; set; }
    public string documentFormatDate { get; set; }
    public string documentType { get; set; }
    public string documentFormatVersion { get; set; }
    public JObject document { get; set; }
}

file class _Postion
{
    public string Name { get; set; }
    public string Quantity { get; set; }
    public int Price { get; set; }
    public int Cost { get; set; }
    public int Unit { get; set; }
    public int Type { get; set; }
    public int Payment { get; set; }
    public int Tax { get; set; }
}