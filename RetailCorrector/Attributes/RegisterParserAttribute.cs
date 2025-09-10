namespace RetailCorrector.Attributes;

[AttributeUsage(AttributeTargets.Class)]
internal class RegisterParserAttribute(string displayName, bool onlyDebug = false) : Attribute;
