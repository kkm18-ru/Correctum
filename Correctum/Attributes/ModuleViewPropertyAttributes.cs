#pragma warning disable IDE0130, CS9113
namespace Correctum.Attributes.ModuleViewProperty;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class AuthorizationPropertiesAttribute
    (params string[] propertyNames) : Attribute;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class SecretBoxAttribute(
    string displayName, 
    string propertyName
) : Attribute;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class TextBoxAttribute(
    string displayName, 
    string propertyName
) : Attribute;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class DateRangeAttribute(
    string displayName, 
    string propertyName
) : Attribute;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class CheckComboBoxAttribute(
    string displayName, 
    string itemsSourcePropertyName,
    string selectedItemsPropertyName
) : Attribute;
