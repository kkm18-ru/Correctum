using Kraig.Attributes;

namespace RetailCorrector.Parsers
{
    internal partial class Sqlite
    {
        [NotifyChanged] private string _path = "";
    }
}
