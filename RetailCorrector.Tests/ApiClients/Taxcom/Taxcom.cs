using Bogus;
using WireMock.Server;
using RetailCorrector.Utils.Integrations.Clients;

namespace RetailCorrector.Tests.ApiClients.Taxcom;

[TestFixture]
internal sealed partial class Taxcom
{
    private static TaxcomClient _client;
    private static WireMockServer _server;

    [OneTimeSetUp] public static void SetUp()
    {
        var faker = new Faker("ru");
        InitValuesAuth(faker);
        
        _server = WireMockServer.Start();
        _client = new TaxcomClient(_server.Urls[0]);
    }

    [OneTimeTearDown] public static void TearDown()
    {
        _server.Dispose();
    }
}
