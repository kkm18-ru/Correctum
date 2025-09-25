namespace RetailCorrector.Attributes;

[AttributeUsage(AttributeTargets.Class)]
internal class RegisterExportAttribute : Attribute
{
    public required string DisplayName { get; set; }
}
