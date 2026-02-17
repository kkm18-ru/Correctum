using System.Text;
using Bogus;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace RetailCorrector.Tests.ApiClients.Taxcom;

partial class Taxcom
{
    private static Array sessions;
    private static DateTime beginDate;
    private static DateTime endDate;
    private static string fnNumber;
    
    private static void InitValuesSession(Faker faker)
    {
        fnNumber = faker.Random.ULong().ToString("D16");
        beginDate = faker.Date.Past();
        endDate = faker.Date.Future();
        
        sessions = new Session[faker.Random.Int(2, 5)];
        var sessionFaker = new Faker<Session>()
            .RuleFor(o => o.fnFactoryNumber, (_, _) => fnNumber)
            .RuleFor(o => o.shiftNumber, (f, _) => f.Random.Int(1))
            .RuleFor(o => o.openDateTime, (f, _) => f.Date.Between(beginDate, endDate))
            .RuleFor(o => o.closeDateTime, (f, s) => f.Date.Between(s.openDateTime, s.openDateTime.AddHours(24)))
            .RuleFor(o => o.receiptCount, (f, _) => f.Random.Int(2));
        for (var i = 0; i < sessions.Length; i++) 
            sessions.SetValue(sessionFaker.Generate(), i);
    }
    
    [Test] public async Task FetchSessionIdsByPeriod_Success()
    {
        _server.Given(Request.Create().UsingGet().WithHeader("Session-Token", token)
                .WithPath("/API/v2/ShiftList").WithParam("pn", pnRegex).WithParam("ps", psRegex)
                .WithParam("fn", fnNumber).WithParam("begin", dtRegex).WithParam("end", dtRegex))
            .RespondWith(Response.Create().WithBodyAsJson(new ArrayResponseBody(sessions), Encoding.UTF8));
        _server.Given(Request.Create().UsingGet().WithHeader("Session-Token", token)
                .WithPath("/API/v2/ShiftList").WithParam("pn", "2").WithParam("ps", psRegex)
                .WithParam("fn", fnNumber).WithParam("begin", dtRegex).WithParam("end", dtRegex))
            .RespondWith(Response.Create()
                .WithBodyAsJson(new ArrayResponseBody(Array.Empty<Session>()), Encoding.UTF8));
        _client.SessionToken = token;
        var sessionIds = await _client.FetchSessionIdsByPeriodAsync(fnNumber, beginDate, endDate);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sessionIds, Has.Count.EqualTo(sessions.Length));
            for (var i = 0; i < sessions.Length; i++)
                Assert.That(sessionIds[i], Is.EqualTo(((Session?)sessions.GetValue(i))?.shiftNumber ?? -1));
        }
    }
}

file class Session
{
    public string fnFactoryNumber { get; set; }
    public int shiftNumber { get; set; }
    public DateTime openDateTime { get; set; }
    public DateTime closeDateTime { get; set; }
    public int receiptCount { get; set; }
}