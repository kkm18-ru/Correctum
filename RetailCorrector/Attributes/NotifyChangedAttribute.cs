namespace RetailCorrector.Attributes;

[AttributeUsage(AttributeTargets.Property)]
internal class NotifyChangedAttribute : Attribute
{
    public object? DefaultValue { get; set; } = default;
}