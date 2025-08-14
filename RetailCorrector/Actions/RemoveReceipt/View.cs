using RetailCorrector.Extensions;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.Actions.RemoveReceipt
{
    internal class RemoveReceiptView : ActionView<RemoveReceiptView, RemoveReceiptPresenter>
    {
        public RemoveReceiptView()
        {
            (Height, Width) = (255, 400);
            var grid = new Grid();
            Content = grid;

            grid.AddColumn(GridUnitType.Star);
            grid.AddColumn(GridUnitType.Pixel, 5);
            grid.AddColumn(GridUnitType.Star);

            grid.AddChild(Context.Condition, dx: 3);
        }        
    }
}
