using System.Collections.ObjectModel;
using RetailCorrector.Data;

namespace RetailCorrector.UI.Controls;

internal sealed partial class ReceiptGrid
{
    public ObservableCollection<DisplayReceipt> Collection { get; } = [];
    public ReceiptGrid()
    {
        InitializeComponent();
    }
}