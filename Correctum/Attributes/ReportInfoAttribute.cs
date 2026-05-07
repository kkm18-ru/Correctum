namespace Correctum.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ReportInfoAttribute(string displayName) : Attribute
{
    public string DisplayName { get; } = displayName;
}