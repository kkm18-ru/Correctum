using RetailCorrector.Utils.Integrations.Clients;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace RetailCorrector.Tests.ApiClients;

partial class FirstOfdClientTests
{
    private static void InitGetRetailPlaceIdsAsync()
    {
        const string path = "/api/cp-ofd/retail-places/without-group";

        _server.Given(Request.Create().WithPath(path).UsingGet()
            .WithCookie("PLAY_SESSION", SessionToken))
            .AtPriority(1)
            .RespondWith(Response.Create().WithStatusCode(200)
            .WithHeader("Content-Type", "application/json")
            .WithBody($$"""
                [
                  {
                    "id": {{RetailPlaceId}},
                    "title": "",
                    "address": "",
                    "addressObj": {
                      "fullAddress": "",
                      "localityId": "00000000-0000-0000-0000-000000000000",
                      "regionCode": "00",
                      "postalCode": "000000",
                      "area": "",
                      "city": null,
                      "settlement": "",
                      "street": "",
                      "house": "",
                      "housing": null,
                      "apartment": null,
                      "latitude": 0,
                      "longitude": 0,
                      "precision": "NUMBER",
                      "fiasLevel": "EMPTY",
                      "unparsedPart": null,
                      "strict": false
                    },
                    "longitude": 0,
                    "latitude": 0,
                    "kkms": [],
                    "kkmsOnlineCount": null,
                    "kkmsStats": null,
                    "kkmGroupId": null,
                    "isKkmsLoaded": null
                  }
                ]
                """));

        _server.Given(Request.Create().WithPath(path).UsingGet())
            .AtPriority(100)
            .RespondWith(Response.Create().WithStatusCode(401)
            .WithBodyAsJson(new FirstOfdError("credentials.invalid", "Неверные параметры аутентификации", null)));
    }

    [Test] public async Task GetRetailPlaceIdsAsync_Should_ReturnIds()
    {
        using var client = new FirstOfdClient(_server.Url!);
        await client.AuthAsync(Login, Password);
        var ids = await client.GetRetailPlaceIdsAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ids, Has.Length.EqualTo(1));
            Assert.That(ids[0], Is.EqualTo(RetailPlaceId));
        }
    }

    [Test] public void GetRetailPlaceIdsAsync_Should_ThrowUnauthorized()
    {
        using var client = new FirstOfdClient(_server.Url!);
        var exc = Assert.ThrowsAsync<FirstOfdException>(() => client.GetRetailPlaceIdsAsync());
        using (Assert.EnterMultipleScope())
        {
            Assert.That(exc.MessageId, Is.EqualTo("credentials.invalid"));
            Assert.That(exc.LocalizedMessage, Is.EqualTo("Неверные параметры аутентификации"));
        }
    }
}
