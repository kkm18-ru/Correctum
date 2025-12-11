using System.IO;
using System.Text;

namespace RetailCorrector.Utils.Extensions;

public static class StreamExtensions
{
    extension(File)
    {
        public static string ReadLine(string filepath)
        {
            using var reader = new StreamReader(filepath, Encoding.ASCII, true);
            return reader.ReadLine() ?? "";
        }
    }
}