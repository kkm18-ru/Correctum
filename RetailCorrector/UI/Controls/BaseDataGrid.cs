using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace RetailCorrector.UI.Controls
{
    class BaseDataGrid: DataGrid
    {
        protected BaseDataGrid()
        {
            IsReadOnly = true;
            CanUserDeleteRows = CanUserReorderColumns =
                CanUserResizeRows = CanUserSortColumns =
                CanUserAddRows = AutoGenerateColumns = false;
            EnableColumnVirtualization = EnableRowVirtualization = true;
            FontFamily = (FontFamily)Application.Current.Resources["jbmono"];
            VirtualizingPanel.SetIsVirtualizing(this, true); 
            VirtualizingPanel.SetIsVirtualizingWhenGrouping(this, true);
            VirtualizingPanel.SetVirtualizationMode(this, VirtualizationMode.Recycling);
            ScrollViewer.SetCanContentScroll(this, true); 
            ScrollViewer.SetIsDeferredScrollingEnabled(this, true);
        }

        protected void AddStarColumn(string name, string binding, double min = 0, double stars = 1)
        {
            Columns.Add(new DataGridTextColumn
            {
                Header = name,
                Binding = new Binding(binding),
                Width = new DataGridLength(stars, DataGridLengthUnitType.Star),
                MinWidth = min
            });
        }

        protected void AddColumn(string name, string binding)
        {
            Columns.Add(new DataGridTextColumn
            {
                Header = name,
                Binding = new Binding(binding),
            });
        }

        protected void AddColumn(string name, string binding, double width)
        {
            Columns.Add(new DataGridTextColumn
            {
                Header = name,
                Binding = new Binding(binding),
                Width = new DataGridLength(width),
                MinWidth = width
            });
        }
    }
}
