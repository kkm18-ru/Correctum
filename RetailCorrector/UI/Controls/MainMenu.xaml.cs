using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using RetailCorrector.UI.Windows;
using RetailCorrector.Utils;
using RetailCorrector.ViewModels.Reports;
using RetailCorrector.Views.Windows;

namespace RetailCorrector.UI.Controls;

public partial class MainMenu : Menu
{
    private string ExplorerPath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Windows), 
        "explorer.exe");

    public MainMenu()
    {
        InitializeComponent();
    }

    private void ShowPosNameReport(object s, RoutedEventArgs e) => new ReportWindow(new PositionNamesReportViewModel()).ShowDialog();
    private void ShowFptrWindow(object s, RoutedEventArgs e) => new FptrWindow().ShowDialog();
    private void ShowConfigurationWindow(object s, RoutedEventArgs e) => new ConfigurationWindow().ShowDialog();
    private void Quit(object s, RoutedEventArgs e) => Application.Current.Shutdown();

    private void OpenDocs(object s, RoutedEventArgs e) => Process.Start(ExplorerPath, Constants.DocsUrl);
    private void OpenRoadmap(object s, RoutedEventArgs e) => Process.Start(ExplorerPath, Constants.RoadmapUrl);
    private void OpenChangelog(object s, RoutedEventArgs e) => Process.Start(ExplorerPath, Constants.ChangelogUrl);
}