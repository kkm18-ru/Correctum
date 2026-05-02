using Correctum.Utils.Integrations.Clients;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Correctum.Tests.ApiClients;

partial class FirstOfdClientTests
{
    private static void InitGetFiscalDrivesAsync()
    {
        var path = $"/api/cp-ofd/kkms/{KktId}/fiscal-drives";

        _server.Given(Request.Create().UsingGet().WithPath(path)
            .WithCookie("PLAY_SESSION", SessionToken))
            .AtPriority(1)
            .RespondWith(Response.Create().WithStatusCode(200)
            .WithHeader("Content-Type", "application/json")
            .WithBody($$"""
            [
                {
                    "producer": "Шифровальное (криптографическое) средство защиты фискальных данных фискальный накопитель «ФН-1.1» исполнение Ав15-2",
                    "number": "{{DriveNum}}",
                    "expireDate": "{{DriveExpire}}",
                    "activationDate": "{{DriveActivation}}",
                    "closeArchiveDate": "{{DriveArchive}}",
                    "memoryUsed": null
                }
            ]
            """));

        _server.Given(Request.Create().UsingGet().WithCookie("PLAY_SESSION", SessionToken)
            .WithPath(new RegexMatcher(@"\/api\/cp-ofd\/kkms\/\d+\/fiscal-drives")))
            .AtPriority(200)
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("[]")
            .WithHeader("Content-Type", "application/json"));

        _server.Given(Request.Create().UsingGet().WithPath(path))
            .AtPriority(300)
            .RespondWith(Response.Create().WithStatusCode(401)
            .WithBodyAsJson(new FirstOfdError("credentials.invalid", "Неверные параметры аутентификации")));
    }

    [Test] public async Task GetFiscalDrivesAsync_Should_ReturnKkts()
    {
        using var client = new FirstOfdClient(_server.Url!);
        await client.AuthAsync(Login, Password);
        var kkts = await client.GetFiscalDrivesAsync(new(KktId, KktRegId));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(kkts, Has.Length.EqualTo(1));
            Assert.That(kkts[0].number, Is.EqualTo(DriveNum));
            Assert.That(kkts[0].activationDate, Is.EqualTo(DateTime.Parse(DriveActivation)));
            Assert.That(kkts[0].expireDate, Is.EqualTo(DateTime.Parse(DriveExpire)));
            Assert.That(kkts[0].closeArchiveDate, Is.EqualTo(DateTime.Parse(DriveArchive)));
        }
    }

    [Test] public async Task GetFiscalDrivesAsync_Should_ReturnEmpty()
    {
        using var client = new FirstOfdClient(_server.Url!);
        await client.AuthAsync(Login, Password);
        var kkts = await client.GetFiscalDrivesAsync(new(KktId+1, KktRegId));
        Assert.That(kkts, Has.Length.EqualTo(0));
    }

    [Test] public void GetFiscalDrivesAsync_Should_ThrowUnauthorized()
    {
        using var client = new FirstOfdClient(_server.Url!);
        var exc = Assert.ThrowsAsync<FirstOfdException>(() => client.GetFiscalDrivesAsync(new(KktId, KktRegId)));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(exc.MessageId, Is.EqualTo("credentials.invalid"));
            Assert.That(exc.LocalizedMessage, Is.EqualTo("Неверные параметры аутентификации"));
            Assert.That(exc.Payload, Is.Null);
        }
    }
}
