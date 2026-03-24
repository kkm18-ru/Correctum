using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Correctum.Enums;

namespace Correctum.ApiClients.KktOnlineClient;

[method: JsonConstructor]
internal readonly partial struct KktOnlineResponse(int status, string? checkResult)
{
    public int Status { get; } = status;
    public string? Details { get; } = checkResult;

    public KktFnStatusCode GetStatusCode() =>
        Status == 0 ? KktFnStatusCode.Error :
        Details is null ? KktFnStatusCode.NeedRegister :
        NoCreatedRegex().IsMatch(Details) ? KktFnStatusCode.NoCreated :
        Details.StartsWith("Экземпляр с данным заводским номером уже используется.") ? KktFnStatusCode.Ok :
        Details.StartsWith("Экземпляр ФН с данным заводским номером закрыт.") ? KktFnStatusCode.Archived :
        KktFnStatusCode.Unknown;
    
    [GeneratedRegex(@"^Заводской номер \w+ отсутствует в реестре произведенных экземпляров \w+$")]
    private static partial Regex NoCreatedRegex();
}