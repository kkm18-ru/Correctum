using RetailCorrector.Attributes;
using RetailCorrector.Data;
using System.Collections.ObjectModel;

namespace RetailCorrector
{
    [Singleton]
    internal partial class Workspace
    {
        public Guid Id { get; }
        public ObservableCollection<ParseInfo> ParserHistory { get; }
        public ObservableCollection<Receipt> Receipts { get; }

        private Workspace()
        {
            Id = Guid.CreateVersion7();
            ParserHistory = [];
            Receipts = [];
        }
    }
}
