using System.Reflection;

namespace Correctum.Storage.Helpers;

public static class ReflectionHelper
{
    private static readonly NullabilityInfoContext _nullabilityContext = new();

    public static bool IsNullable(PropertyInfo property)
    {
        var type = property.PropertyType;

        // 1. Value types (int vs int?)
        if (type.IsValueType)
            return Nullable.GetUnderlyingType(type) != null;

        // 2. Reference types (string vs string?)
        var nullability = _nullabilityContext.Create(property);
        return nullability.WriteState == NullabilityState.Nullable;
    }
}
