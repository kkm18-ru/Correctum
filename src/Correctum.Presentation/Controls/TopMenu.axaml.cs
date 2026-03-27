using Avalonia;
using Avalonia.Controls;
using Correctum.ViewModels.Importers;
using Correctum.Views.Windows;

namespace Correctum.Controls;

public partial class TopMenu : Menu
{
    public TopMenu()
    {
        InitializeComponent();
    }

    private void ShowLocalImporter(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }

    private void ShowOfdRuTokenImporter(object? sender, Avalonia.Interactivity.RoutedEventArgs e) =>
        new ImportWindow(new OfdRuTokenImporterViewModel()).ShowDialog(MainWindow.Instance);

    private void ShowOfdRuImporter(object? sender, Avalonia.Interactivity.RoutedEventArgs e) =>
        new ImportWindow(new OfdRuImporterViewModel()).ShowDialog(MainWindow.Instance);

    private void ShowSabyOfdImporter(object? sender, Avalonia.Interactivity.RoutedEventArgs e) =>
        new ImportWindow(new SabyImporterViewModel()).ShowDialog(MainWindow.Instance);

    private void ShowPlatformOfdImporter(object? sender, Avalonia.Interactivity.RoutedEventArgs e) =>
        new ImportWindow(new PlatformImporterViewModel()).ShowDialog(MainWindow.Instance);

    private void ShowFirstOfdImporter(object? sender, Avalonia.Interactivity.RoutedEventArgs e) =>
        new ImportWindow(new FirstOfdImporterViewModel()).ShowDialog(MainWindow.Instance);

    private void ShowTaxcomKassImporter(object? sender, Avalonia.Interactivity.RoutedEventArgs e) =>
        new ImportWindow(new TaxcomImporterViewModel()).ShowDialog(MainWindow.Instance);


}