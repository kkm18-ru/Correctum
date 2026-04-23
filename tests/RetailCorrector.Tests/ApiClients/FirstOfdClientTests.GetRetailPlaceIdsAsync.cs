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
                    "title": "{{_faker.Company.CompanyName()}}",
                    "address": "{{_faker.Address.FullAddress()}}",
                    "addressObj": {
                      "fullAddress": "{{_faker.Address.FullAddress()}}",
                      "localityId": "{{Guid.CreateVersion7()}}",
                      "regionCode": "{{_faker.Address.CountryCode()}}",
                      "postalCode": "{{_faker.Address.ZipCode()}}",
                      "area": "{{_faker.Address.State()}}",
                      "city": "{{_faker.Address.City()}}",
                      "settlement": null,
                      "street": "{{_faker.Address.StreetName()}}",
                      "house": "{{_faker.Address.BuildingNumber()}}",
                      "housing": null,
                      "apartment": null,
                      "latitude": {{_faker.Address.Latitude()}},
                      "longitude": {{_faker.Address.Longitude()}},
                      "precision": "NUMBER",
                      "fiasLevel": "EMPTY",
                      "unparsedPart": null,
                      "strict": false
                    },
                    "longitude": {{_faker.Address.Longitude()}},
                    "latitude": {{_faker.Address.Latitude()}},
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
