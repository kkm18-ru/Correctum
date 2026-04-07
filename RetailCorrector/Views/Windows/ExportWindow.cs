using RetailCorrector.UI.Windows.Types;
using RetailCorrector.ViewModels.Exports;
using System.Windows.Controls;
using System.Windows.Data;

namespace RetailCorrector.Views.Windows;

public class ExportWindow : DialogWindow
{
    public ExportWindow(BaseExportViewModel vm)
    {
        DataContext = vm;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
        SetBinding(TitleProperty, new Binding(nameof(BaseExportViewModel.ExportName))
        {
            Mode = BindingMode.OneTime,
            StringFormat = "Экспортер: {0}"
        });

        var grid = new Grid { Margin = new(5), Width = 350 };

        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new(1, System.Windows.GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new(5) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new(1, System.Windows.GridUnitType.Star) });

        grid.RowDefinitions.Add(new RowDefinition { Height = new(1, 0) });

        var btn = new Button { Content = "Экспорт" };
        btn.SetBinding(Button.CommandProperty, new Binding(nameof(BaseExportViewModel.ExportCommand))
        {
            Mode = BindingMode.OneTime,
        });
        grid.AddChild(btn);

        var progress = new ProgressBar();
        progress.SetBinding(ProgressBar.IsIndeterminateProperty, new Binding(nameof(BaseExportViewModel.IsBusy))
        {
            Mode = BindingMode.OneWay
        });
        grid.AddChild(progress, x: 2);

        Content = grid;
    }
}
