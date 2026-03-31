using RetailCorrector.UI.Windows.Types;
using RetailCorrector.ViewModels.Reports;

namespace RetailCorrector.Views.Windows;

public sealed partial class ReportWindow : DialogWindow
{
    public ReportWindow(BaseReportViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
