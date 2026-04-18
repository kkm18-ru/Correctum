using Correctum.Core.Models;
using System.Collections.ObjectModel;

namespace RetailCorrector
{
    internal partial class Storage
    {
        public static Storage Instance => field ??= new Storage();

        public Guid Id { get; }
        public ObservableCollection<Receipt> Receipts { get; }
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private Storage()
        {
            Id = Guid.CreateVersion7();
            Receipts = [];
            Logger.Info("Создано новое рабочее место \"{Id:D}\"", Id);
        }
    }
}
