using Correctum.Abstractions;
using Correctum.UI.Windows;
using Correctum.Utils;
using Correctum.ViewModels.Exports;
using Correctum.ViewModels.Windows;
using Correctum.Views.Windows;
using Correctum.Windows;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using Microsoft.Xaml.Behaviors.Core;
using System.Collections.Immutable;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Correctum.ViewModels.Controls;

public sealed partial class MainMenuViewModel : BindableBase
{
    private static string ExplorerPath { get; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe");

    public static MenuItem[] Exporters => SearchExporters();
    public static partial ImmutableArray<ImportDescriptor> Imports { get; }
    public static partial ImmutableArray<ActionDescriptor> Actions { get; }

    public ICommand OpenImportCommand { get; } = new RelayCommand<ImportDescriptor>(OpenImport);
    public ICommand OpenActionCommand { get; } = new RelayCommand<ActionDescriptor>(OpenAction);
    public ICommand CreateContextCommand { get; } = new ActionCommand(CreateDbContext);
    public ICommand LoadContextCommand { get; } = new ActionCommand(SetPathContext);
    public ICommand SaveAsContextCommand { get; } = new ActionCommand(ChangePathContext);
    public ICommand OpenExportMasterCommand { get; } = new RelayCommand<Type>(OpenExportMaster);
    public ICommand ShowFptrCommand { get; } = new ActionCommand(() => new FptrWindow().ShowDialog());
    public ICommand ShowPasteCommand { get; } = new ActionCommand(() => new PasteWindow().ShowDialog());
    public ICommand ShowQueryCommand { get; } = new ActionCommand(() => new QueryWindow().ShowDialog());
    public ICommand ShowSettingsCommand { get; } = new ActionCommand(() => new ConfigurationWindow().ShowDialog());
    public ICommand QuitCommand { get; } = new ActionCommand(() => Application.Current.Shutdown());
    public ICommand OpenLinkCommand { get; } = new RelayCommand<string>(OpenLink);
    
    private static void OpenLink(string url) => Process.Start(ExplorerPath, url);

    private static void ChangePathContext()
    {
        var dialog = new SaveFileDialog
        {
            Filter = $"{StorageContext.DisplayNameFileExtension}|*{StorageContext.FileExtension}"
        };
        if (dialog.ShowDialog() ?? false)
        {
            var connBuilder = new SqliteConnectionStringBuilder(StorageContext.ConnectionString);
            SqliteConnection.ClearAllPools();
            File.Copy(connBuilder.DataSource, dialog.FileName);
            StorageContext.SetContextPath(dialog.FileName);
        }
    }

    private static void CreateDbContext()
    {
        var dialog = new SaveFileDialog
        {
            Filter = $"{StorageContext.DisplayNameFileExtension}|*{StorageContext.FileExtension}"
        };
        if(dialog.ShowDialog() ?? false)
            StorageContext.SetContextPath(dialog.FileName);
    }

    private static void SetPathContext()
    {
        var dialog = new OpenFileDialog
        {
            Filter = $"{StorageContext.DisplayNameFileExtension}|*{StorageContext.FileExtension}"
        };
        if (dialog.ShowDialog() ?? false)
            StorageContext.SetContextPath(dialog.FileName);
    }

    private static void OpenImport(ImportDescriptor descriptor) =>
        new ImportWindow(descriptor.DisplayName, descriptor.Function.Invoke()).ShowDialog();

    private static void OpenAction(ActionDescriptor descriptor) =>
        new ActionWindow(descriptor.DisplayName, descriptor.Function.Invoke()).ShowDialog();

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

    public readonly record struct ImportDescriptor(string DisplayName, Func<BaseImport> Function);
    public readonly record struct ActionDescriptor(string DisplayName, Func<IAction> Function);
}