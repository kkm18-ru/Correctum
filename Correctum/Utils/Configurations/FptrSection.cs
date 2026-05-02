namespace Correctum.Utils.Configurations
{
    internal sealed class FptrSection
    {
        public int CountReceiptsBeforeDelay { get; set; } = 1000;
        public int Delay { get; set; } = 15;
    }
}
