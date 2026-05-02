using System.Text;
using Bogus;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Correctum.Tests.ApiClients.Taxcom;

partial class Taxcom
{
    private static Array drives;
    private static string regId;
    
    private static void InitValuesDrive(Faker faker)
    {
        regId = faker.Random.ULong().ToString("D16");
        drives = new Drive[faker.Random.Int(2, 5)];
        var driveFaker = new Faker<Drive>()
            .RuleFor(d => d.fn, (f, _) => f.Random.ULong().ToString("D16"))
            .RuleFor(d => d.statusFn, (f, _) => f.Random.Bool() ? "Active" : "NotActive");
        for (var i = 0; i < drives.Length; i++) 
            drives.SetValue(driveFaker.Generate(), i);
    }
    
    [Test] public async Task FetchDriveIds_Success()
    {
        _server.Given(Request.Create().UsingGet().WithHeader("Session-Token", token)
                .WithPath("/API/v2/FnHistory").WithParam("kktRegNumber", regId))
            .RespondWith(Response.Create().WithBodyAsJson(new ArrayResponseBody(drives), Encoding.UTF8));
        _client.SessionToken = token;
        using (Assert.EnterMultipleScope())
        {
            var i = 0;
            await foreach (var id in _client.FetchDriveIdsAsync(regId))
            {
                Assert.That(id, Is.EqualTo(((Drive?)drives.GetValue(i))?.fn ?? ""));
                i++;
            }
        }
    }
}

file class Drive
{
    public string fn { get; set; }
    public string statusFn { get; set; }
}