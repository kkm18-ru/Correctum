using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using RetailCorrector.UI.Windows;
using RetailCorrector.Utils;

namespace RetailCorrector.UI.Controls;

public partial class MainMenu : Menu
{
    public MainMenu()
    {
        InitializeComponent();
    }

    private void ShowFptrWindow(object s, RoutedEventArgs e) => new FptrWindow().ShowDialog();
    private void ShowSettingsWindow(object s, RoutedEventArgs e) => new SettingsWindow().ShowDialog();
    private void Quit(object s, RoutedEventArgs e) => Application.Current.Shutdown();

    private void OpenDocs(object s, RoutedEventArgs e) => Process.Start("explorer", Constants.DocsUrl);
    private void OpenRoadmap(object s, RoutedEventArgs e) => Process.Start("explorer", Constants.RoadmapUrl);
    private void OpenChangelog(object s, RoutedEventArgs e) => Process.Start("explorer", Constants.ChangelogUrl);
}