using RetailCorrector.Attributes;

namespace RetailCorrector.Parsers
{
    internal partial class OfdRu 
    {
        private static byte COUNT_TRIES => Configuration.OfdRu.CountTries;
        private static int REQUEST_DELAY => Configuration.OfdRu.RequestDelay;

        [NotifyChanged] public partial DateTime BeginDate { get; set; }
        [NotifyChanged] public partial DateTime EndDate { get; set; }
        [NotifyChanged] public partial string Vatin { get; set; }
        [NotifyChanged] public partial string DeviceId { get; set; }
        [NotifyChanged] public partial string Token { get; set; }
    }
}
