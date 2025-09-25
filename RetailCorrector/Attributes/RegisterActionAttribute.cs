namespace RetailCorrector.Attributes;

[AttributeUsage(AttributeTargets.Class)]
internal class RegisterActionAttribute : Attribute
{
    public required string DisplayName { get; set; }
}
