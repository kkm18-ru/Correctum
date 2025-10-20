namespace RetailCorrector.Attributes;

[AttributeUsage(AttributeTargets.Property)]
internal class NotifyChangedAttribute : Attribute
{
    public object? DefaultValue { get; set; } = default;
    public bool CreateEvent{ get; set; } = false;
}