using Correctum.Core.ValueObjects;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RetailCorrector.Converters;

public sealed class InnConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is Inn inn ? inn.ToString() : "";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string s) return new();
        try
        {
            return Inn.Parse(s);
        }
        catch(Exception e)
        {
            MessageBox.Show(e.Message, "Некорректный ИНН", 0, MessageBoxImage.Error);
            return new();
        }
    }
}
