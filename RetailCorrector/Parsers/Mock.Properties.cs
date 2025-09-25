using RetailCorrector.Attributes;

namespace RetailCorrector.Parsers
{
    internal partial class Mock
    {
        [NotifyChanged(DefaultValue = 1)] 
        public partial int MinCountDocs { get; set; }
        [NotifyChanged(DefaultValue = 1000)] 
        public partial int MaxCountDocs { get; set; }
    }
}
