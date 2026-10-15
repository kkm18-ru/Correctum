using Correctum.UI.Windows.Types;
using Correctum.Utils;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace Correctum.Windows;

public sealed class QueryWindow : DialogWindow, INotifyPropertyChanged
{
    public QueryWindow()
    {
        QueryCommand = new RelayCommand(Query, CanQuery);
    }

    private async void Query()
    {
        try
        {
            StorageContext.CreateBackup();
            await using var ctx = new StorageContext();
            await using var cmd = ctx.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = CommandText;

            if(cmd.Connection!.State != ConnectionState.Open)
                await cmd.Connection.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            StatusText = $"Количество измененных строк: {reader.RecordsAffected}";
            var table = new DataTable();
            table.Load(reader);
            Result = table;
        }
        catch (Exception e)
        {
            StatusText = e.Message;
        }
        finally
        {
            PropertyChanged?.Invoke(this, new(nameof(StatusText)));
            PropertyChanged?.Invoke(this, new(nameof(Result)));
        }
    }

    private bool CanQuery() => !string.IsNullOrWhiteSpace(CommandText);

    public ICommand QueryCommand { get; }
    public DataTable? Result { get; private set; }
    public string? StatusText { get; private set; }

    public string CommandText
    {
        get => (string)GetValue(CommandTextProperty);
        set => SetValue(CommandTextProperty, value);
    }

    public static readonly DependencyProperty CommandTextProperty =
        DependencyProperty.Register(nameof(CommandText),
            typeof(string), typeof(QueryWindow));

    public event PropertyChangedEventHandler? PropertyChanged;
}
