using System.Text.RegularExpressions;

namespace Correctum.Exceptions.Saby;

public partial class SabyAuthException : Exception
{
    public string Title { get; init; }
    public string Description { get; init; }
    
    public SabyAuthException(string errorMessage) : base(errorMessage)
    {
        var match = MessageRegex().Match(errorMessage.Replace("\\\"", "\""));
        Title = match.Groups["title"].Value;
        Description = match.Groups["desc"].Value;
    }

    [GeneratedRegex(@"(?<title>.+) \[0\] \(ОПИСАНИЕ: (?<desc>.+)\)",0,250)]
    private static partial Regex MessageRegex();
}