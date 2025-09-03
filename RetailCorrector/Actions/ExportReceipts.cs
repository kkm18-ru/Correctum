using RetailCorrector.Attributes;
using System.Diagnostics;
using System.Windows;

namespace RetailCorrector.Actions
{
    [RegisterAction("Экспорт чеков")]
    internal partial class ExportReceipts : ActionBase<ExportReceipts, ExportReceipts.View>
    {
        public override async void Start()
        {
            var path = await Exporter.Export();
            if (MessageBox.Show($"Открыть каталог с сохранёнными чеками?", "Чеки сохранены", 
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                Process.Start("explorer.exe", path);
        }
    }
}
