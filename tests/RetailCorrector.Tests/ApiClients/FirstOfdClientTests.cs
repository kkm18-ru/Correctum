using WireMock.Server;

namespace RetailCorrector.Tests.ApiClients;

public sealed partial class FirstOfdClientTests
{
    private static WireMockServer _server;

    const string SessionToken = "ffffffff-ffff-ffff-ffff-ffffffffffff";

    const string Login = "admin";
    const string Password = "passwd";

    [OneTimeSetUp] public static void Setup()
    {
        _server = WireMockServer.Start();
        InitAuthAsync();
        InitGetRetailPlaceIdsAsync();
    }

    [OneTimeTearDown] public static void Teardown()
    {
        _server.Dispose();
    }
}
