using Bogus;
using WireMock.Server;
using RetailCorrector.Utils.Integrations.Clients;
using WireMock.Matchers;

namespace RetailCorrector.Tests.ApiClients.Taxcom;

[TestFixture]
internal sealed partial class Taxcom
{
    private static TaxcomClient _client;
    private static WireMockServer _server;

    private static RegexMatcher pnRegex = new(@"^(-\d+|0|1)$");
    private static RegexMatcher psRegex = new(@"^([1-9]\d{0,2}|1000)$");
    private static RegexMatcher dtRegex = new(@"^(\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2})$");

    [OneTimeSetUp] public static void SetUp()
    {
        var faker = new Faker("ru");
        InitValuesAuth(faker);
        InitValuesOutlet(faker);
        InitValuesDevice(faker);
        InitValuesDrive(faker);
        InitValuesSession(faker);
        InitValuesDocumentSummary(faker);
        InitValuesDocumentInfo(faker);
        
        _server = WireMockServer.Start();
        _client = new TaxcomClient(_server.Urls[0]);
    }

    [OneTimeTearDown] public static void TearDown()
    {
        _server.Dispose();
    }
}

internal class ArrayResponseBody(Array _records)
{
    public DateTime reportDate { get; set; } = DateTime.Now;
    public Counts counts { get; set; } = new(_records.Length, _records.Length, _records.Length);
    public Array records { get; set; } = _records;
}
internal readonly record struct Counts(int recordCount, int recordFilteredCount, int recordInResponceCount);