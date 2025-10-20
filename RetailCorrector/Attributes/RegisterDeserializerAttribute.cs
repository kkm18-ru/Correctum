namespace RetailCorrector.Attributes;

[AttributeUsage(AttributeTargets.Class)]
internal class RegisterDeserializerAttribute : Attribute
{
    public required string DisplayName { get; set; }
}
