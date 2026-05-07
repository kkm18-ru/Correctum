namespace Correctum.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class DeserializerInfoAttribute(string displayName) : Attribute
{
    public string DisplayName { get; } = displayName;
}