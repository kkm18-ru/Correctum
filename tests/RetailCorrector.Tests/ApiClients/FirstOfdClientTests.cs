using WireMock.Server;

namespace RetailCorrector.Tests.ApiClients;

public sealed partial class FirstOfdClientTests
{
    private static WireMockServer _server;

    [OneTimeSetUp] public static void Setup()
    {
        _server = WireMockServer.Start();
    }

    [OneTimeTearDown] public static void Teardown()
    {
        _server.Dispose();
    }
}
