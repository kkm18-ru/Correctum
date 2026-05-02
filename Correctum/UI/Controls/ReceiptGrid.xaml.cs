using System.Collections.ObjectModel;
using Correctum.Data;

namespace Correctum.UI.Controls;

internal sealed partial class ReceiptGrid
{
    public ObservableCollection<DisplayReceipt> Collection { get; } = [];
    public ReceiptGrid()
    {
        InitializeComponent();
    }
}