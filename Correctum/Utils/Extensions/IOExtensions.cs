using System.IO;
using System.Text;

namespace Correctum.Utils.Extensions;

internal static class IOExtensions
{
    extension(Directory)
    {
        public static void CreateDirectoryIfNotExists(string? path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }

    extension(File)
    {
        public static string ReadLine(string filepath)
        {
            using var reader = new StreamReader(filepath, Encoding.ASCII, true);
            return reader.ReadLine() ?? "";
        }
    }
}
