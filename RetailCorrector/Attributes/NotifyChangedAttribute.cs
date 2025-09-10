namespace RetailCorrector.Attributes;

[AttributeUsage(AttributeTargets.Property)]
internal class NotifyChangedAttribute(object? defaultValue = null) : Attribute;