using System.Runtime.CompilerServices;

namespace Correctum
{
    public static partial class Utils
    {
        internal static void SetField<T>(ref T field, T value, Func<T, string?> validate, [CallerMemberName] string propertyName = "")
        {
            if (validate(value) is { Length: > 0 } error)
                throw new ArgumentException(error, propertyName);
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
        }
    }
}
