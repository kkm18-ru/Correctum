namespace RetailCorrector.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ImporterPropertyAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}