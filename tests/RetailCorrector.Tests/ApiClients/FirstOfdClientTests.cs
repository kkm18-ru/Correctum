using Bogus;
using WireMock.Server;

namespace RetailCorrector.Tests.ApiClients;

public sealed partial class FirstOfdClientTests
{
    private static WireMockServer _server;
    private static Faker _faker;

    const string SessionToken = "ffffffff-ffff-ffff-ffff-ffffffffffff";

    const string Login = "admin";
    const string Password = "passwd";

    const int RetailPlaceId = 1;
    const int KktId = 2;
    const string KktRegId = "1234567890123456";

    [OneTimeSetUp] public static void Setup()
    {
        _faker = new Faker("ru");
        _server = WireMockServer.Start();
        InitAuthAsync();
        InitGetRetailPlaceIdsAsync();
        InitGetKktsAsync();
    }

    [OneTimeTearDown] public static void Teardown()
    {
        _server.Dispose();
    }
}
