using RetailCorrector.Utils;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.Actions
{
    internal partial class RemoveReceipt
    {
        internal class View : ActionView<View, RemoveReceipt>
        {
            public View()
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
}
