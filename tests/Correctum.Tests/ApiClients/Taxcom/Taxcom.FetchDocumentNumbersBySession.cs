using System.Text;
using Bogus;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Correctum.Tests.ApiClients.Taxcom;

partial class Taxcom
{
    private static Array documentSummaries;
    private static uint shift;
    
    private static void InitValuesDocumentSummary(Faker faker)
    {
        shift = faker.Random.UInt(1);
        documentSummaries = new DocumentSummary[faker.Random.Int(2, 5)];
        var summaryFaker = new Faker<DocumentSummary>()
            .RuleFor(ds => ds.fnFactoryNumber, (f, _) => f.Random.ULong().ToString("D16"))
            .RuleFor(ds => ds.fpd, (f, _) => f.Random.UInt().ToString())
            .RuleFor(ds => ds.shiftNumber, (f, _) => f.Random.Int(1).ToString())
            .RuleFor(ds => ds.documentType, (_, _) => "3")
            .RuleFor(ds => ds.dateTime, (f, _) => f.Date.Past())
            .RuleFor(ds => ds.fdNumber, (f, _) => f.Random.Int(1))
            .RuleFor(ds => ds.numberInShift, (f, _) => f.Random.Int(1))
            .RuleFor(ds => ds.cashier, (f, _) => f.Random.ULong().ToString("D16"))
            .RuleFor(ds => ds.taxationSystem, (f, _) => f.Random.ULong().ToString("D16"))
            .RuleFor(ds => ds.accountingType, (f, _) => f.Random.ULong().ToString("D16"))
            .RuleFor(ds => ds.sum, (f, _) => f.Random.Int())
            .RuleFor(ds => ds.cash, (f, _) => f.Random.Int())
            .RuleFor(ds => ds.electronic, (f, _) => f.Random.Int())
            .RuleFor(ds => ds.nondsSum, (f, _) => f.Random.Int())
            .RuleFor(ds => ds.nds0Sum, (f, _) => f.Random.Int())
            .RuleFor(ds => ds.nds10, (f, _) => f.Random.Int())
            .RuleFor(ds => ds.nds20, (f, _) => f.Random.Int())
            .RuleFor(ds => ds.ndsC10, (f, _) => f.Random.Int())
            .RuleFor(ds => ds.ndsC20, (f, _) => f.Random.Int())
            .RuleFor(ds => ds.nds5, (f, _) => f.Random.Int())
            .RuleFor(ds => ds.nds7, (f, _) => f.Random.Int())
            .RuleFor(ds => ds.ndsC5, (f, _) => f.Random.Int())
            .RuleFor(ds => ds.ndsC7, (f, _) => f.Random.Int())
            .RuleFor(ds => ds.nds22, (f, _) => f.Random.Int())
            .RuleFor(ds => ds.ndsC22, (f, _) => f.Random.Int())
            .RuleFor(ds => ds.sumPrepaid, (f, _) => f.Random.Int())
            .RuleFor(ds => ds.sumPostpaid, (f, _) => f.Random.Int())
            .RuleFor(ds => ds.sumСounterclaims, (f, _) => f.Random.Int());
        for (var i = 0; i < documentSummaries.Length; i++) 
            documentSummaries.SetValue(summaryFaker.Generate(), i);
    }
    
    [Test] public async Task FetchDocumentNumbersBySession_Success()
    {
        _server.Given(Request.Create().UsingGet().WithHeader("Session-Token", token)
                .WithPath("/API/v2/DocumentList").WithParam("pn", pnRegex).WithParam("ps", psRegex)
                .WithParam("fn", fnNumber).WithParam("shift", shift.ToString()))
            .RespondWith(Response.Create().WithBodyAsJson(new ArrayResponseBody(documentSummaries), Encoding.UTF8));
        _server.Given(Request.Create().UsingGet().WithHeader("Session-Token", token)
                .WithPath("/API/v2/DocumentList").WithParam("pn", "2").WithParam("ps", psRegex)
                .WithParam("fn", fnNumber).WithParam("shift", shift.ToString()))
            .RespondWith(Response.Create()
                .WithBodyAsJson(new ArrayResponseBody(Array.Empty<DocumentSummary>()), Encoding.UTF8));
        _client.SessionToken = token;
        var docIds = await _client.FetchDocumentNumbersBySessionAsync(fnNumber, shift);
        var list = new List<int>(((DocumentSummary[])documentSummaries).Select(ds => ds.fdNumber));
        list.Sort();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(docIds, Has.Count.EqualTo(documentSummaries.Length));
            for (var i = 0; i < documentSummaries.Length; i++)
                Assert.That(docIds[i], Is.EqualTo(list[i]));
        }
    }
}

file class DocumentSummary
{
    public string fnFactoryNumber { get; set; }
    public string shiftNumber { get; set; }
    public string documentType { get; set; }
    public DateTime dateTime { get; set; }
    public int fdNumber { get; set; }
    public int numberInShift { get; set; }
    public string fpd { get; set; }
    public string cashier { get; set; }
    public string? taxationSystem { get; set; }
    public string? accountingType { get; set; }
    public int sum { get; set; }
    public int cash { get; set; }
    public int electronic { get; set; }
    public int nondsSum { get; set; }
    public int nds0Sum { get; set; }
    public int nds10 { get; set; }
    public int nds20 { get; set; }
    public int ndsC10 { get; set; }
    public int ndsC20 { get; set; }
    public int nds5 { get; set; }
    public int nds7 { get; set; }
    public int ndsC5 { get; set; }
    public int ndsC7 { get; set; }
    public int nds22 { get; set; }
    public int ndsC22 { get; set; }
    public int sumPrepaid { get; set; }
    public int sumPostpaid { get; set; }
    public int sumСounterclaims { get; set; }
}