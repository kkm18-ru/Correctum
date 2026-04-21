using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace RetailCorrector.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для ReceiptGrid.xaml
    /// </summary>
    public partial class ReceiptGrid : DockPanel
    {
        public ReceiptGrid()
        {
            InitializeComponent();
        }

        private void OnSelectionChangedGrid(object? s, SelectionChangedEventArgs e)
        {
            var sender = s as DataGrid;
            sender.RowDetailsVisibilityMode = sender.SelectedItems.Count == 1
                ? DataGridRowDetailsVisibilityMode.VisibleWhenSelected
                : DataGridRowDetailsVisibilityMode.Collapsed;
        }
    }
}
