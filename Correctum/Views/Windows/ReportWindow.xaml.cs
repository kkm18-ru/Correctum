using Correctum.UI.Windows.Types;
using Correctum.ViewModels.Windows;
using System.Data;

namespace Correctum.Views.Windows;

public sealed partial class ReportWindow : DialogWindow
{
    public ReportWindow(string reportName, Func<DataTable> function)
    {
        DataContext = new ReportWindowViewModel(reportName, function);
        InitializeComponent();
    }
}
