using Correctum.UI.Windows.Types;
using Correctum.ViewModels.Reports;

namespace Correctum.Views.Windows;

public sealed partial class ReportWindow : DialogWindow
{
    public ReportWindow(BaseReportViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
