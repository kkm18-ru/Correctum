using RetailCorrector.Data;
using Serilog;
using System.Collections.ObjectModel;

namespace RetailCorrector
{
    internal partial class Workspace
    {
        private static Workspace _instance = null!;
        public static Workspace Instance => _instance ??= new Workspace();

        public Guid Id { get; }
        public ObservableCollection<ParseInfo> ParserHistory { get; }
        public ObservableCollection<Receipt> Receipts { get; }

        private Workspace()
        {
            Id = Guid.CreateVersion7();
            ParserHistory = [];
            Receipts = [];
            Log.Information("Создано новое рабочее место \"{Id:D}\"", Id);
        }
    }
}
