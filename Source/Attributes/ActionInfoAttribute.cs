namespace Correctum.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ActionInfoAttribute(string displayName) : Attribute
{
    public string DisplayName { get; } = displayName;
}