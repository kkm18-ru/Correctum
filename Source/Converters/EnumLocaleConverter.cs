using Correctum.Resources.Locales;
using System.Globalization;
using System.Windows.Data;

namespace Correctum.Converters;

public sealed class EnumLocaleConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var defaultLocale = parameter as string;
        var type = value.GetType();
        if (!type.IsEnum) return defaultLocale;
        var name = $"{type.FullName}.{value:G}";
        var locale = EnumLocales.ResourceManager.GetString(name);
        return locale ?? defaultLocale;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
