using Kraig.Attributes;

namespace RetailCorrector.Parsers
{
    internal partial class OfdRu 
    {
        [NotifyChanged] private DateTime _beginDate;
        [NotifyChanged] private DateTime _endDate;
        [NotifyChanged] private string _vatin;
        [NotifyChanged] private string _deviceId;
        [NotifyChanged(GenerateEvent = true)] private string _token;
    }
}
