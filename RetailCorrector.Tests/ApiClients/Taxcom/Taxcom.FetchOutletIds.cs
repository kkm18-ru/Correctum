using System.Text;
using Bogus;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace RetailCorrector.Tests.ApiClients.Taxcom;

partial class Taxcom
{
    private static Array outlets;
    
    private static void InitValuesOutlet(Faker faker)
    {
        outlets = new Outlet[faker.Random.Int(2, 5)];
        var outletFaker = new Faker<Outlet>()
            .RuleFor(o => o.id, (f, _) => f.Random.Uuid().ToString("D"))
            .RuleFor(o => o.name, (f, _) => f.Name.JobArea())
            .RuleFor(o => o.code, (_, _) => null)
            .RuleFor(o => o.address, (f, _) => f.Address.FullAddress())
            .RuleFor(o => o.problemIndicator, (f, _) => f.Random.Bool(0.65f) ? "OK" : "Problem")
            .RuleFor(o => o.department, (_, _) => null);
        for (var i = 0; i < outlets.Length; i++) 
            outlets.SetValue(outletFaker.Generate(), i);
    }
    
    [Test] public async Task FetchOutletIds_Success()
    {
        _server.Given(Request.Create().UsingGet().WithHeader("Session-Token", token)
                .WithPath("/API/v2/OutletList").WithParam("pn", pnRegex).WithParam("ps", psRegex))
            .RespondWith(Response.Create().WithBodyAsJson(new ArrayResponseBody(outlets), Encoding.UTF8));
        _server.Given(Request.Create().UsingGet().WithHeader("Session-Token", token)
                .WithPath("/API/v2/OutletList").WithParam("pn", "2").WithParam("ps", psRegex))
            .RespondWith(Response.Create()
                .WithBodyAsJson(new ArrayResponseBody(Array.Empty<Outlet>()), Encoding.UTF8));
        _client.SessionToken = token;
        using (Assert.EnterMultipleScope())
        {
            var i = 0;
            await foreach (var id in _client.FetchOutletIdsAsync())
            {
                Assert.That(id, Is.EqualTo(((Outlet?)outlets.GetValue(i))?.id ?? ""));
                i++;
            }
        }
    }
}

file class Outlet
{
    public string id { get; set; }
    public string name { get; set; }
    public string? code { get; set; }
    public string address { get; set; }
    public string problemIndicator { get; set; }
    public string? department { get; set; }
}