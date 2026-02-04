using System.Collections.ObjectModel;
using RetailCorrector.Data;

namespace RetailCorrector.UI.Controls;

internal sealed partial class PositionGrid
{
    public static PositionGrid Instance { get; } = new();
    public ObservableCollection<DisplayPostion> Collection { get; } = [];
    public PositionGrid()
    {
        InitializeComponent();
    }
}