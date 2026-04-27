namespace Correctum.Storage.Extensions;

public static class ExceptionExtensions
{
    extension(IOException)
    {
        public static void ThrowIfFileNotFound(string filepath)
        {
            if (!File.Exists(filepath)) 
                throw new FileNotFoundException(null, filepath);
        }
    }
}
