using RetailCorrector.Attributes;
using RetailCorrector.Data;
using System.Collections.ObjectModel;

namespace RetailCorrector
{
    [Singleton]
    internal partial class Workspace
    {
        public ObservableCollection<ParseInfo> ParserHistory { get; }
        public ObservableCollection<Receipt> Receipts { get; }

        private Workspace()
        {
            ParserHistory = [];
            Receipts = [];
        }
    }
}
