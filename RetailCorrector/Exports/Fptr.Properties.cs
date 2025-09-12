using RetailCorrector.Attributes;

namespace RetailCorrector.Exports
{
    internal partial class Fptr
    {
        [NotifyChanged(0)] public partial int CorrType { get; set; }
        /// <summary>
        /// Печатать, ли чеки? (0 - Да; 1 - Нет)
        /// </summary>
        [NotifyChanged(1)] public partial int Print { get; set; }
        [NotifyChanged("")] public partial string ActId { get; set; }
    }
}
