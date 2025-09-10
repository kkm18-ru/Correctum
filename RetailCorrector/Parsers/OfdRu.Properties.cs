using RetailCorrector.Attributes;

namespace RetailCorrector.Parsers
{
    internal partial class OfdRu 
    {
        [NotifyChanged] public partial DateTime BeginDate { get; set; }
        [NotifyChanged] public partial DateTime EndDate { get; set; }
        [NotifyChanged] public partial string Vatin { get; set; }
        [NotifyChanged] public partial string DeviceId { get; set; }
        [NotifyChanged] public partial string Token { get; set; }
    }
}
