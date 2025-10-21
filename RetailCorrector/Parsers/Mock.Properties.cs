using Kraig.Attributes;

namespace RetailCorrector.Parsers
{
    internal partial class Mock
    {
        [NotifyChanged] private int _minCountDocs = 1;
        [NotifyChanged] private int _maxCountDocs = 1000;
    }
}
