using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;
using Correctum.UI.Windows;
using Correctum.Utils;
using Correctum.ViewModels.Exports;
using Correctum.Views.Windows;

namespace Correctum.ViewModels.Controls;

public sealed class MainMenuViewModel : BaseViewModel
{
    private static string ExplorerPath { get; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe");

    public static MenuItem[] Exporters => SearchExporters();
    public static MenuItem[] Reports => [.. ModuleRegistry.Reports.Select(ModuleDescriptorHelper.ToMenuItem)];

    public ICommand OpenExportMasterCommand { get; } = new RelayCommand<Type>(OpenExportMaster);
    public ICommand ShowFptrCommand { get; } = new ActionCommand(() => new FptrWindow().ShowDialog());
    public ICommand ShowSettingsCommand { get; } = new ActionCommand(() => new ConfigurationWindow().ShowDialog());
    public ICommand QuitCommand { get; } = new ActionCommand(() => Application.Current.Shutdown());
    public ICommand OpenLinkCommand { get; } = new RelayCommand<string>(OpenLink);
    
    private static void OpenLink(string url) => Process.Start(ExplorerPath, url);

    private static void OpenExportMaster(Type type)
    {
        var exporter = (BaseExportViewModel)Activator.CreateInstance(type)!;
        new ExportWindow(exporter).ShowDialog();
    }

    private static MenuItem[] SearchExporters() =>
        [.. typeof(BaseExportViewModel).Assembly.GetTypes()
            .Where(type => type.IsSubclassOf(typeof(FileExportViewModel))) // захадркодено
            .Select(type =>
            {
                var report = (BaseExportViewModel)Activator.CreateInstance(type)!;
                var item = new MenuItem
                {
                    Header = report.ExportName,
                    CommandParameter = type,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                item.SetBinding(MenuItem.CommandProperty, nameof(OpenExportMasterCommand));
                return item;
            })];
}