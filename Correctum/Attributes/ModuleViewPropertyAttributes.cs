#pragma warning disable IDE0130, CS9113
namespace Correctum.Attributes.ModuleViewProperty;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class AuthorizationPropertiesAttribute
    (params string[] propertyNames) : Attribute;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class TextBoxAttribute(
    string displayName, 
    string propertyName,
    string converterResourceKey = ""
) : Attribute;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class DateRangeAttribute(
    string displayName, 
    string propertyName
) : Attribute;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class CheckBoxAttribute(
    string displayName, 
    string propertyName
) : Attribute;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class CheckComboBoxAttribute(
    string displayName,
    string selectedItemsPropertyName, 
    string itemsSourcePropertyName
) : Attribute;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ComboBoxAttribute(
    string displayName,
    string selectedPropertyName, 
    string itemsSourcePropertyName
) : Attribute;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class LocalizedComboBoxAttribute(
    string displayName,
    string selectedPropertyName, 
    string localizedItemsPropertyName
) : Attribute;
