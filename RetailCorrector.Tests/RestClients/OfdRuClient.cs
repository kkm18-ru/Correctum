using WireMock.Server;
using Clients = RetailCorrector.Utils.Integrations.Clients;

namespace RetailCorrector.Tests.RestClients;

[TestFixture]
public class OfdRuClient
{
    private static WireMockServer _server;
    private static Clients.OfdRuClient _client;

    [OneTimeSetUp]
    public static void SetUp()
    {
        var port = Random.Shared.Next(10000, 30000);
        _server = WireMockServer.StartWithAdminInterface(port);
        _client = new Clients.OfdRuClient(_server.Url!);
    }

    [OneTimeTearDown]
    public static void TearDown()
    {
        _client.Dispose();
        _server.Dispose();
    }
}