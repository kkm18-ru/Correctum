using System.IO;

namespace RetailCorrector.Utils.Helpers
{
    internal static class IOHelper
    {
        public static void CreateDirectoryIfNotExists(string? path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            if (!Directory.Exists(path)) 
                Directory.CreateDirectory(path);
        }
    }
}
