#pragma warning disable CA1822
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using Correctum.ViewModels.Importers;
using Correctum.Views.Windows;
using Ursa.Controls;

namespace Correctum.ViewModels;

public class MenuViewModel : ObservableObject
{

    public async void ShowLocalImporter()
    {
        try
        {
            //MvvmDialogs.FrameworkDialogs.OpenFile.OpenFileDialogSettings
            
            /*var filters = new FilePickerFileType[]
            {
                new("Отчет ОФД-Я"){ Patterns = ["*.xlsx", "*.xls"] },
            };

            var topLevel = TopLevel.GetTopLevel(MainWindow.Instance);
            var dialogOptions = new FilePickerOpenOptions
            {
                FileTypeFilter = filters,
                AllowMultiple = false
            };
            var dialog = await topLevel!.StorageProvider.OpenFilePickerAsync(dialogOptions);
            await MessageBox.ShowAsync(dialogOptions.SuggestedFileType?.Name ?? "null");*/
            //todo
        }
        catch (Exception ex) {
            await MessageBox.ShowAsync(ex.ToString(), icon: MessageBoxIcon.Error);
        }
    }

    public void ShowOfdRuTokenImporter() =>
        new ImportWindow(new OfdRuTokenImporterViewModel()).ShowDialog(MainWindow.Instance);

    public void ShowOfdRuImporter() =>
        new ImportWindow(new OfdRuImporterViewModel()).ShowDialog(MainWindow.Instance);

    public void ShowSabyOfdImporter() =>
        new ImportWindow(new SabyImporterViewModel()).ShowDialog(MainWindow.Instance);

    public void ShowPlatformOfdImporter() =>
        new ImportWindow(new PlatformImporterViewModel()).ShowDialog(MainWindow.Instance);

    public void ShowFirstOfdImporter() =>
        new ImportWindow(new FirstOfdImporterViewModel()).ShowDialog(MainWindow.Instance);

    public void ShowTaxcomKassImporter() =>
        new ImportWindow(new TaxcomImporterViewModel()).ShowDialog(MainWindow.Instance);

    public void ShowFptrWindow() =>
        new FptrWindow().ShowDialog(MainWindow.Instance);
}
