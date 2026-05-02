using System.Text;
using Bogus;
using Correctum.Utils.Integrations.Clients;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Correctum.Tests.ApiClients.Taxcom;

partial class Taxcom
{
    private static string login;
    private static string password;
    private static string integratorId;
    private static string token;
    private static string agreementNumber;
    
    private static void InitValuesAuth(Faker faker)
    {
        login = faker.Internet.Email();
        password = faker.Internet.Password();
        integratorId = faker.Random.Uuid().ToString("D");
        token = faker.Internet.Password(40);
        agreementNumber = faker.Internet.Password(51);
    }
    
    [Test] public void Auth_Success()
    {
        _server.Given(Request.Create().UsingPost().WithBodyAsJson(new RequestBody(login, password))
                .WithHeader("Integrator-ID", integratorId).WithPath("/API/v2/Login"))
            .RespondWith(Response.Create().WithBodyAsJson(new ResponseBody(token), Encoding.UTF8));
        TaxcomClient.IntegratorId = integratorId;
        Assert.DoesNotThrowAsync(async () => await _client.AuthAsync(login, password));
        Assert.That(_client.SessionToken, Is.EqualTo(token));
    }
    
    [Test] public void AuthAgreement_Success()
    {
        _server.Given(Request.Create().UsingPost().WithBodyAsJson(new RequestBody(login, password, agreementNumber))
                .WithHeader("Integrator-ID", integratorId).WithPath("/API/v2/Login"))
            .RespondWith(Response.Create().WithBodyAsJson(new ResponseBody(token), Encoding.UTF8));
        TaxcomClient.IntegratorId = integratorId;
        Assert.DoesNotThrowAsync(async () => await _client.AuthAsync(login, password, agreementNumber));
        Assert.That(_client.SessionToken, Is.EqualTo(token));
    }
}

file readonly record struct RequestBody(string login, string password, string? agreementNumber = null);
file readonly record struct ResponseBody(string sessionToken);