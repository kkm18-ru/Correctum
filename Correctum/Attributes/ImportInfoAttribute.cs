namespace Correctum.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ImportInfoAttribute(string displayName) : Attribute
{
    public string DisplayName { get; } = displayName;
}