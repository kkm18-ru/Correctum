using Bogus;
using WireMock.Server;

namespace Correctum.Tests.ApiClients;

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
    const string DriveNum = "2345678901234561";
    const string DriveActivation = "2021-01-04T07:10:13";
    const string DriveArchive = "2022-02-05T08:11:14";
    const string DriveExpire = "2023-03-06T09:12:15";

    [OneTimeSetUp] public static void Setup()
    {
        _faker = new Faker("ru");
        _server = WireMockServer.Start();
        InitAuthAsync();
        InitGetRetailPlaceIdsAsync();
        InitGetKktsAsync();
        InitGetFiscalDrivesAsync();
    }

    [OneTimeTearDown] public static void Teardown()
    {
        _server.Dispose();
    }
}
