using System.Windows;

namespace RetailCorrector.Actions
{
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
