namespace RetailCorrector.Attributes;

[AttributeUsage(AttributeTargets.Class)]
internal class RegisterParserAttribute : Attribute
{
    public required string DisplayName { get; set; }
}
