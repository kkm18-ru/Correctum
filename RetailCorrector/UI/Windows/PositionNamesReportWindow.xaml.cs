using Microsoft.Win32;
using RetailCorrector.Enums;
using RetailCorrector.UI.Windows.Types;
using RetailCorrector.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace RetailCorrector.UI.Windows;

public sealed partial class PositionNamesReportWindow : DialogWindow, INotifyPropertyChanged
{
    public string ReportName => "Наименования позиций";
    public bool IsBusy { get; set => SetField(ref field, value); } = false;
    public bool CanExport { get; set => SetField(ref field, value); } = false;
    public ObservableCollection<ReportItem> Items { get; } = [];

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public PositionNamesReportWindow()
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        if(EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public async void Refresh(object? sender, RoutedEventArgs args)
    {
        try
        {
            IsBusy = true;
            CanExport = false;

            var result = await Task.Run(() =>
                Workspace.Instance.Receipts
                    .SelectMany(r => r.Items.Select(i => new { i.Name, i.Type, r, }))
                    .GroupBy(x => (x.Name, x.Type))
                    .Select(n => new ReportItem(n.Key.Name, n.Key.Type, n
                        .Select(x => x.r)
                        .Distinct()
                        .Count()))
                    .OrderBy(i => i.Name)
                    .ToList());
            
            Items.Clear();
            foreach (var item in result)
                Items.Add(item);

            Logger.Info("Отчет \"{ReportName}\" успешно составлен.", ReportName);
            CanExport = Items.Count > 0;
            IsBusy = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не удалось составить отчет.", "", 0, MessageBoxImage.Error);
            Logger.Error(ex, "Не удалось составить отчет \"{ReportName}\"", ReportName);
        }
    }

    public void Export(object? sender, RoutedEventArgs args)
    {
        IsBusy = true;
        var saveDialog = new SaveFileDialog
        {
            Filter = "CSV-таблица (*.csv)|*.csv",
            FileName = "report.csv",
            OverwritePrompt = true,
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            AddToRecent = true,
            
        };
        if (saveDialog.ShowDialog() ?? false)
        {
            var lines = Items.Select(i => $"\"{i.Name}\";\"{i.TypeName}\";{i.CountReceipts:F2}");
            File.WriteAllText(saveDialog.FileName, string.Join("\r\n", ["\"Наименование\";\"Тип\";\"Количество чеков\"", .. lines]), Encoding.UTF8);
            MessageBox.Show("Отчет успешно экспортирован!");
            Logger.Info("Отчет \"{ReportName}\" успешно экспортирован.", ReportName);
        }
        IsBusy = false;
    }

    public readonly struct ReportItem(string name, PositionType type, int numReceipts)
    {
        public string Name { get; } = name;
        public string TypeName { get; } = EnumLocales.PositionTypes.GetLocale(type);
        public int CountReceipts { get; } = numReceipts;
    }
}
