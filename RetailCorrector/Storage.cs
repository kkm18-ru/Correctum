using Correctum.Core.Models;
using System.Collections.ObjectModel;

namespace RetailCorrector
{
    internal partial class Storage
    {
        public static Storage Instance { get; private set; }

        public Guid Id { get; }
        public ObservableCollection<Receipt> Receipts { get; }
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private Storage()
        {
            Id = Guid.CreateVersion7();
            Receipts = [];
            Logger.Info("Создано новое рабочее место \"{Id:D}\"", Id);
        }

        public static void Load(string filepath)
        {
            throw new NotImplementedException();
            Instance = new Storage();
        }

        public void Save(string filepath)
        {
            throw new NotImplementedException();
        }
    }
}
