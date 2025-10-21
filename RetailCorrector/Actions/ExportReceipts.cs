using System.Windows;
using Kraig.Attributes;

namespace RetailCorrector.Actions
{
    [Singleton]
    internal partial class ExportReceipts : ActionBase<ExportReceipts, ExportReceipts.View>
    {
        public override string Name => "Экспорт чеков";

        public override async void Start()
        {
            await Exporter.Export(Path);
            MessageBox.Show("Чеки сохранены");
        }
    }
}
