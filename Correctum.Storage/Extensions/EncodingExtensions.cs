using Correctum.Storage.Encodings;
using System.Text;

namespace Correctum.Storage.Extensions;

internal static class EncodingExtensions
{
    extension(Encoding)
    {
        public static CorrectumStorageEncoding CorrectumStorage => CorrectumStorageEncoding.Instance;
    }
}
