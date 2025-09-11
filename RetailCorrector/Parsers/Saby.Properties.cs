using RetailCorrector.Attributes;

namespace RetailCorrector.Parsers;

internal partial class Saby
{
    [NotifyChanged] public partial string Login { get; set; }
    [NotifyChanged] public partial string Password { get; set; }
    [NotifyChanged] public partial string INN { get; set; }
    [NotifyChanged] public partial string Device { get; set; }
    [NotifyChanged] public partial string Storage { get; set; }
    [NotifyChanged] public partial DateTime FromDate { get; set; }
    [NotifyChanged] public partial DateTime ToDate { get; set; }
}