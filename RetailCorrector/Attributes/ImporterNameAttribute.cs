namespace RetailCorrector.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ImporterNameAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}