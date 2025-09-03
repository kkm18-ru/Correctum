namespace RetailCorrector.Attributes;

[AttributeUsage(AttributeTargets.Class)]
internal class RegisterParserAttribute(string displayName) : Attribute;
