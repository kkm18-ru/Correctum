using RetailCorrector.Utils.Integrations.Clients;
using System.Globalization;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace RetailCorrector.Tests.ApiClients;

partial class FirstOfdClientTests
{
    private static void InitGetKktsAsync()
    {
        var path = $"/api/cp-ofd/retail-places/filter-retail-place/{RetailPlaceId}";

        _server.Given(Request.Create().UsingPost().WithBody("{}").WithPath(path)
            .WithCookie("PLAY_SESSION", SessionToken))
            .AtPriority(1)
            .RespondWith(Response.Create().WithStatusCode(200)
            .WithHeader("Content-Type", "application/json")
            .WithBody($$"""
            {
              "id": {{RetailPlaceId}},
              "title": "{{_faker.Address.FullAddress()}}",
              "address": "{{_faker.Address.FullAddress()}}",
              "addressObj": {
                "fullAddress": "{{_faker.Address.FullAddress()}}",
                "localityId": "{{Guid.CreateVersion7()}}",
                "regionCode": "{{_faker.Address.CountryCode()}}",
                "postalCode": "{{_faker.Address.ZipCode()}}",
                "area": "{{_faker.Address.State()}}",
                "city": "{{_faker.Address.City()}}",
                "settlement": null,
                "street": "{{_faker.Address.StreetAddress()}}",
                "house": "{{_faker.Address.BuildingNumber()}}",
                "housing": null,
                "apartment": null,
                "latitude": {{_faker.Address.Latitude().ToString(CultureInfo.InvariantCulture)}},
                "longitude": {{_faker.Address.Longitude().ToString(CultureInfo.InvariantCulture)}},
                "precision": "NUMBER",
                "fiasLevel": "HOUSE",
                "unparsedPart": null,
                "strict": false
              },
              "latitude": {{_faker.Address.Latitude().ToString(CultureInfo.InvariantCulture)}},
              "longitude": {{_faker.Address.Longitude().ToString(CultureInfo.InvariantCulture)}},
              "kkms": [
                {
                  "id": {{KktId}},
                  "orgId": {{_faker.Random.ULong(1, 999999)}},
                  "retailPlaceId": {{RetailPlaceId}},
                  "internalName": "{{_faker.Name.JobTitle()}}",
                  "model": null,
                  "serialNumber": "{{_faker.Random.ULong(100000000000000, 999999999999999):D15}}",
                  "onlineStatus": 0,
                  "status": null,
                  "createdAt": null,
                  "declineReason": "",
                  "fnsKkmId": "{{KktRegId}}",
                  "regStatus": null,
                  "regStatusChangedAt": null,
                  "year": null,
                  "fiscalDrive": null,
                  "lastShift": null,
                  "address": null,
                  "rpTitle": null,
                  "billingStatus": 1,
                  "timezone": "{{_faker.Date.TimeZoneString()}}",
                  "virtualKKm": null,
                  "params": {
                    "checkStatus": [
                      {
                        "value": 3,
                        "description": "ККТ зарегистрирована "
                      }
                    ],
                    "cloudKkmId": null
                  },
                  "flcError": null,
                  "markingError": null,
                  "billingInfo": [],
                  "online": false
                }
              ],
              "kkmsOnlineCount": 0,
              "kkmsStats": {
                "id": {{KktId}},
                "kkmsCount": 1,
                "kkmsOnlineCount": 0,
                "lastShiftSells": 0,
                "lastShiftReturns": 0
              },
              "kkmGroupId": null,
              "isKkmsLoaded": true
            }
            """));

        _server.Given(Request.Create().UsingPost().WithBody("{}")
            .WithCookie("PLAY_SESSION", SessionToken)
            .WithPath(new RegexMatcher(@"\/api\/cp-ofd\/retail-places\/filter-retail-place\/\d+")))
            .AtPriority(200)
            .RespondWith(Response.Create().WithStatusCode(403)
            .WithBodyAsJson(new FirstOfdError("kkms.retail.place.forbidden", "Нет доступа к торговой точке")));

        _server.Given(Request.Create().UsingPost().WithBody("{}").WithPath(path))
            .AtPriority(300)
            .RespondWith(Response.Create().WithStatusCode(401)
            .WithBodyAsJson(new FirstOfdError("credentials.invalid", "Неверные параметры аутентификации")));
    }

    [Test] public async Task GetKktsAsync_Should_ReturnKkts()
    {
        using var client = new FirstOfdClient(_server.Url!);
        await client.AuthAsync(Login, Password);
        var kkts = await client.GetKktsAsync(RetailPlaceId);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(kkts, Has.Length.EqualTo(1));
            Assert.That(kkts[0].id, Is.EqualTo(KktId));
            Assert.That(kkts[0].fnsKkmId, Is.EqualTo(KktRegId));
        }
    }

    [Test] public async Task GetKktsAsync_Should_ThrowForbidden()
    {
        using var client = new FirstOfdClient(_server.Url!);
        await client.AuthAsync(Login, Password);
        var exc = Assert.ThrowsAsync<FirstOfdException>(() => client.GetKktsAsync(RetailPlaceId+1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(exc.MessageId, Is.EqualTo("kkms.retail.place.forbidden"));
            Assert.That(exc.LocalizedMessage, Is.EqualTo("Нет доступа к торговой точке"));
            Assert.That(exc.Payload, Is.Null);
        }

    }

    [Test] public void GetKktsAsync_Should_ThrowUnauthorized()
    {
        using var client = new FirstOfdClient(_server.Url!);
        var exc = Assert.ThrowsAsync<FirstOfdException>(() => client.GetKktsAsync(RetailPlaceId));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(exc.MessageId, Is.EqualTo("credentials.invalid"));
            Assert.That(exc.LocalizedMessage, Is.EqualTo("Неверные параметры аутентификации"));
            Assert.That(exc.Payload, Is.Null);
        }
    }
}
