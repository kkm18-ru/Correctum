using Correctum.Utils.Integrations.Clients;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Correctum.Tests.ApiClients;

partial class FirstOfdClientTests
{
    private static void InitAuthAsync()
    {
        const string path = "/api/cp-core/user/login";

        // Выдаёт валидный токен
        _server.Given(Request.Create().WithPath(path).UsingPost()
            .WithBodyAsJson(new FirstOfdAuthRequest(Login, Password)))
            .AtPriority(1)
            .RespondWith(Response.Create().WithStatusCode(200)
            .WithHeader("Set-Cookie", $"PLAY_SESSION={SessionToken}; Path=/; Domain=localhost")
            .WithBodyAsJson(new { authToken = SessionToken }));

        // Выдаёт ошибку (некорректные данные)
        _server.Given(Request.Create().WithPath(path).UsingPost()
            .WithBody(new WildcardMatcher("*\"login\":\"*\"*"))
            .WithBody(new WildcardMatcher("*\"password\":\"*\"*")))
            .AtPriority(100)
            .RespondWith(Response.Create().WithStatusCode(400)
            .WithBodyAsJson(new FirstOfdError("user.wrong.credentials", "Неверный логин или пароль")));

        // Выдаёт ошибку (логин пустой)
        _server.Given(Request.Create().WithPath(path).UsingPost()
            .WithBody(new WildcardMatcher("*\"login\":\"\"*"))
            .WithBody(new WildcardMatcher("*\"password\":\"*\"*")))
            .AtPriority(100)
            .RespondWith(Response.Create().WithStatusCode(400)
            .WithBodyAsJson(new FirstOfdError("method.arguments.error", "Проверьте правильность данных",
                new([new("login", "", "NotBlank", "не должно быть пустым")]))));

        // Выдаёт ошибку (пароль пустой)
        _server.Given(Request.Create().WithPath(path).UsingPost()
            .WithBody(new WildcardMatcher("*\"login\":\"*\"*"))
            .WithBody(new WildcardMatcher("*\"password\":\"\"*")))
            .AtPriority(100)
            .RespondWith(Response.Create().WithStatusCode(400)
            .WithBodyAsJson(new FirstOfdError("method.arguments.error", "Проверьте правильность данных", 
                new([new("password", "", "NotBlank", "не должно быть пустым")]))));

        // Выдаёт ошибку (логин и пароль пустые)
        _server.Given(Request.Create().WithPath(path).UsingPost()
            .WithBody(new WildcardMatcher("*\"login\":\"\"*"))
            .WithBody(new WildcardMatcher("*\"password\":\"\"*")))
            .AtPriority(100)
            .RespondWith(Response.Create().WithStatusCode(400)
            .WithBodyAsJson(new FirstOfdError("method.arguments.error", "Проверьте правильность данных", 
                new([new("login", "", "NotBlank", "не должно быть пустым"),
                    new("password", "", "NotBlank", "не должно быть пустым")]))));
    }

    [Test]
    public void AuthAsync_Should_DoesNotThrows()
    {
        using var client = new FirstOfdClient(_server.Url!);
        Assert.DoesNotThrowAsync(() => client.AuthAsync(Login, Password));
    }

    [Test]
    public void AuthAsync_Should_ThrowInvalidData()
    {
        using var client = new FirstOfdClient(_server.Url!);
        var exc = Assert.ThrowsAsync<FirstOfdException>(() => client.AuthAsync(Password, Password));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(exc.MessageId, Is.EqualTo("user.wrong.credentials"));
            Assert.That(exc.LocalizedMessage, Is.EqualTo("Неверный логин или пароль"));
            Assert.That(exc.Payload, Is.Null);
        }
    }

    [Test]
    public void AuthAsync_Should_ThrowEmptyLogin()
    {
        using var client = new FirstOfdClient(_server.Url!);
        var exc = Assert.ThrowsAsync<FirstOfdException>(() => client.AuthAsync("", Password));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(exc.MessageId, Is.EqualTo("method.arguments.error"));
            Assert.That(exc.LocalizedMessage, Is.EqualTo("Проверьте правильность данных"));
            Assert.That(exc.Payload, Is.Not.Null);
            Assert.That(exc.Payload?.fieldErrors, Has.Length.EqualTo(1));
            Assert.That(exc.Payload?.fieldErrors[0].field, Is.EqualTo("login"));
            Assert.That(exc.Payload?.fieldErrors[0].value, Is.EqualTo(""));
            Assert.That(exc.Payload?.fieldErrors[0].message, Is.EqualTo("NotBlank"));
            Assert.That(exc.Payload?.fieldErrors[0].localizedMessage, Is.EqualTo("не должно быть пустым"));
        }
    }

    [Test]
    public void AuthAsync_Should_ThrowEmptyPassword()
    {
        using var client = new FirstOfdClient(_server.Url!);
        var exc = Assert.ThrowsAsync<FirstOfdException>(() => client.AuthAsync(Login, ""));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(exc.MessageId, Is.EqualTo("method.arguments.error"));
            Assert.That(exc.LocalizedMessage, Is.EqualTo("Проверьте правильность данных"));
            Assert.That(exc.Payload, Is.Not.Null);
            Assert.That(exc.Payload?.fieldErrors, Has.Length.EqualTo(1));
            Assert.That(exc.Payload?.fieldErrors[0].field, Is.EqualTo("password"));
            Assert.That(exc.Payload?.fieldErrors[0].value, Is.EqualTo(""));
            Assert.That(exc.Payload?.fieldErrors[0].message, Is.EqualTo("NotBlank"));
            Assert.That(exc.Payload?.fieldErrors[0].localizedMessage, Is.EqualTo("не должно быть пустым"));
        }
    }

    [Test]
    public void AuthAsync_Should_ThrowEmptyLoginAndPassword()
    {
        using var client = new FirstOfdClient(_server.Url!);
        var exc = Assert.ThrowsAsync<FirstOfdException>(() => client.AuthAsync("", ""));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(exc.MessageId, Is.EqualTo("method.arguments.error"));
            Assert.That(exc.LocalizedMessage, Is.EqualTo("Проверьте правильность данных"));
            Assert.That(exc.Payload, Is.Not.Null);
            Assert.That(exc.Payload?.fieldErrors, Has.Length.EqualTo(2));
            Assert.That(exc.Payload?.fieldErrors[0].field, Is.EqualTo("login"));
            Assert.That(exc.Payload?.fieldErrors[0].value, Is.EqualTo(""));
            Assert.That(exc.Payload?.fieldErrors[0].message, Is.EqualTo("NotBlank"));
            Assert.That(exc.Payload?.fieldErrors[0].localizedMessage, Is.EqualTo("не должно быть пустым"));
            Assert.That(exc.Payload?.fieldErrors[1].field, Is.EqualTo("password"));
            Assert.That(exc.Payload?.fieldErrors[1].value, Is.EqualTo(""));
            Assert.That(exc.Payload?.fieldErrors[1].message, Is.EqualTo("NotBlank"));
            Assert.That(exc.Payload?.fieldErrors[1].localizedMessage, Is.EqualTo("не должно быть пустым"));
        }
    }
}
