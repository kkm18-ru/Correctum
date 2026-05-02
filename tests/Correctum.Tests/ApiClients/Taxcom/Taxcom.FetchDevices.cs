using System.Text;
using Bogus;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Correctum.Tests.ApiClients.Taxcom;

partial class Taxcom
{
    private static Array devices;
    private static object outlet;
    
    private static void InitValuesDevice(Faker faker)
    {
        outlet = new Faker<Outlet>()
            .RuleFor(o => o.id, (f, _) => f.Random.Uuid().ToString("D"))
            .RuleFor(o => o.name, (f, _) => f.Name.JobArea())
            .RuleFor(o => o.address, (f, _) => f.Address.FullAddress())
            .RuleFor(o => o.code, (_, _) => null).Generate();
        
        devices = new Device[faker.Random.Int(2, 5)];
        var devicesFaker = new Faker<Device>()
            .RuleFor(d => d.name, (f, _) => f.Name.JobArea())
            .RuleFor(d => d.kktRegNumber, (f, _) => f.Random.ULong().ToString("D16"))
            .RuleFor(d => d.kktFactoryNumber, (f, _) => f.Random.ULong().ToString("D16"))
            .RuleFor(d => d.fnFactoryNumber, (f, _) => f.Random.ULong().ToString("D16"))
            .RuleFor(d => d.cashdeskState, (f, _) => f.Random.Bool() ? "Active" : "Expired")
            .RuleFor(d => d.problemIndicator, (f, _) => f.Random.Bool() ? "OK" : "Problem");
        for (var i = 0; i < devices.Length; i++) 
            devices.SetValue(devicesFaker.Generate(), i);
    }
    
    [Test] public async Task FetchDevices_Success()
    {
        var outletId = ((Outlet)outlet).id;
        _server.Given(Request.Create().UsingGet().WithHeader("Session-Token", token).WithParam("id", outletId)
                .WithPath("/API/v2/KKTList").WithParam("pn", pnRegex).WithParam("ps", psRegex))
            .RespondWith(Response.Create()
                .WithBodyAsJson(new DeviceArrayResponseBody(devices, (Outlet)outlet), Encoding.UTF8));
        _server.Given(Request.Create().UsingGet().WithHeader("Session-Token", token).WithParam("pn", "2")
                .WithPath("/API/v2/KKTList").WithParam("id", outletId).WithParam("ps", psRegex))
            .RespondWith(Response.Create()
                .WithBodyAsJson(new DeviceArrayResponseBody(Array.Empty<Outlet>(), (Outlet)outlet), Encoding.UTF8));
        _client.SessionToken = token;
        using (Assert.EnterMultipleScope())
        {
            var i = 0;
            await foreach (var regId in _client.FetchDevicesAsync(outletId))
            {
                Assert.That(regId, Is.EqualTo(((Device?)devices.GetValue(i))?.kktRegNumber ?? ""));
                i++;
            }
        }
    }
}

file class Device
{
    public string name { get; set; }
    public string kktRegNumber { get; set; }
    public string kktFactoryNumber { get; set; }
    public string fnFactoryNumber { get; set; }
    public string cashdeskState { get; set; }
    public string problemIndicator { get; set; }
}

file class DeviceArrayResponseBody(Array _records, Outlet _outlet) : ArrayResponseBody(_records)
{
    public Outlet outlet { get; set; } = _outlet;
}

file class Outlet
{
    public string id { get; set; }
    public string name { get; set; }
    public string? code { get; set; }
    public string address { get; set; }
}