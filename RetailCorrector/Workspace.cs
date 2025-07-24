using RetailCorrector.Data;
using System.Collections.ObjectModel;

namespace RetailCorrector
{
    internal class Workspace
    {
        public ObservableCollection<Receipt> Receipts { get; }
        public uint TaskId { get; set; }

        public static Workspace Instance { get; private set; }

        private Workspace()
        {
            Receipts = [];
        }

        public static void New() => Instance = new();
    }
}
