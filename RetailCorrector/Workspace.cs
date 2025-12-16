using RetailCorrector.Data;
using Serilog;
using System.Collections.ObjectModel;

namespace RetailCorrector
{
    internal partial class Workspace
    {
        public static Workspace Instance => field ??= new Workspace();

        public Guid Id { get; }
        public ObservableCollection<ImportSession> ImportHistory { get; }
        public ObservableCollection<Receipt> Receipts { get; }

        private Workspace()
        {
            Id = Guid.CreateVersion7();
            ImportHistory = [];
            Receipts = [];
            Log.Information("Создано новое рабочее место \"{Id:D}\"", Id);
        }
    }
}
