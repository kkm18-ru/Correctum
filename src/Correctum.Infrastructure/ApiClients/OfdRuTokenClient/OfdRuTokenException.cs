using System.Text;

namespace Correctum.ApiClients.OfdRuTokenClient;

public sealed class OfdRuTokenException : Exception
{
    internal OfdRuTokenException(IEnumerable<string> errors)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Запрос выполнился со следующими ошибками:");
        foreach (var error in errors)
            builder.AppendLine($" - {error}");
    }
}
