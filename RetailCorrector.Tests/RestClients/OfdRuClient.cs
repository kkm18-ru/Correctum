using System.Diagnostics.CodeAnalysis;
using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Clients = RetailCorrector.Utils.Integrations.Clients;

namespace RetailCorrector.Tests.RestClients;

[TestFixture]
public class OfdRuClient
{
    private static WireMockServer _server;
    private static Clients.OfdRuClient _client;
    
    private static readonly IStringMatcher TokenMatcher = new RegexMatcher(TokenPattern);
    
    [StringSyntax("Regex")] private const string TokenPattern = "^[0-9a-f]{32}$";
    private const string TokenParamKey = "AuthToken";
    private const string GroupListPath = "/api/integration/v2/kktgroup/list";
    

    [OneTimeSetUp]
    public static void SetUp()
    {
        var port = Random.Shared.Next(10000, 30000);
        _server = WireMockServer.StartWithAdminInterface(port);
        _client = new Clients.OfdRuClient(_server.Url!);
        AddRequests();
    }

    private static void AddRequests()
    {
        _server.Given(Request.Create().WithPath(GroupListPath).UsingGet()
                .WithParam(TokenParamKey, TokenMatcher))
            .RespondWith(Response.Create().WithHeader("Content-Type", "application/json")
                .WithBody("""
                          {
                            "Status":"Success",
                            "Data":[
                                {
                                    "AgreementId":"dc5cf05e-261b-41ed-a35d-5daa8279b2d6",
                                    "Groups":[
                                        {
                                            "Id":"334082e7-e77e-4218-ad8f-42a299a86b07",
                                            "Name":"Все кассы",
                                            "Path":"/Все кассы/",
                                            "Level":1
                                        }
                                    ]
                                }
                            ],
                            "Elapsed":"00:00:00.0133278"
                          }
                          """));
        _server.Given(Request.Create().WithPath(GroupListPath).UsingGet())
            .RespondWith(Response.Create().WithHeader("Content-Type", "application/json; charset=utf-8")
                .WithBody("""{"Message":"Forbidden"}""").WithStatusCode(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task SuccessAuthTest()
    {
        await _client.SetAuthTokenAsync(Guid.NewGuid().ToString("N"));
        Assert.Pass();
    }

    [Test]
    public void FailedAuthTest()
    {
        var ex = Assert.ThrowsAsync<HttpRequestException>(
            async () => await _client.SetAuthTokenAsync($"{Guid.NewGuid():N}123")
        );

        Assert.That(ex.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public void EmptyTokenTest()
    {
        var ex = Assert.ThrowsAsync<ArgumentException>(
            async () => await _client.SetAuthTokenAsync(string.Empty)
        );

        Assert.That(ex, Is.Not.Null);
    }

    [OneTimeTearDown]
    public static void TearDown()
    {
        _client.Dispose();
        _server.Dispose();
    }
}