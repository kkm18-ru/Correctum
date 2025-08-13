using RetailCorrector.Extensions;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.Actions.RemoveReceipt
{
    internal class RemoveReceiptView : ActionView<RemoveReceiptView, RemoveReceiptPresenter>
    {
        public RemoveReceiptView()
        {
            (Height, Width) = (255, 230);
            var grid = new Grid();
            Content = grid;

            grid.AddColumn(GridUnitType.Pixel, 109);
            grid.AddColumn(GridUnitType.Pixel, 5);
            grid.AddColumn(GridUnitType.Pixel, 115);

            grid.AddRow(GridUnitType.Pixel, 22);
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Pixel, 238);

            grid.AddChild(UIGenerators.Text("Тип фильтра:"));

            var vb = new ComboBox { ItemsSource = new string[] { "Белый список", "Черный список" } };
            vb.SetBinding(ComboBox.SelectedIndexProperty, nameof(Context.FilterType));
            grid.AddChild(vb, 2);

            grid.AddChild(Context.Condition, y: 2, dx: 3);
        }        
    }
}
