using RetailCorrector.Attributes;
using System.Windows;

namespace RetailCorrector.Actions
{
    [RegisterAction("Экспорт чеков")]
    internal partial class ExportReceipts : ActionBase<ExportReceipts, ExportReceipts.View>
    {
        public override async void Start()
        {
            await Exporter.Export(Path);
            MessageBox.Show("Чеки сохранены");
        }
    }
}
