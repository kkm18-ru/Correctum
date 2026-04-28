using System.IO;

namespace RetailCorrector.Utils.Configurations
{
    internal sealed class HttpClientSection
    {
        public int RequestDelay { get; set; } = 1000;
        public byte TriesCount { get; set; } = 3;
        public short PageSize { get; set; } = 100;
        public string HeadlessBrowserPath { get; set; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), 
                "Microsoft", "Edge", "Application", "msedge.exe");
    }
}
