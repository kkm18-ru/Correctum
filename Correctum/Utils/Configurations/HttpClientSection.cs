namespace Correctum.Utils.Configurations
{
    internal sealed class HttpClientSection
    {
        public int RequestDelay { get; set; } = 1000;
        public byte TriesCount { get; set; } = 3;
        public short PageSize { get; set; } = 100;
    }
}
