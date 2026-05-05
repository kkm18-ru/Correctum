using Correctum.Core.Models;
using Correctum.UI.Controls;
using Correctum.Utils;
using System.Collections.Immutable;
using System.Windows;
using System.Windows.Input;

namespace Correctum.ViewModels.Windows;

public sealed partial class PasteWindowViewModel : BaseViewModel
{
    public static partial ImmutableArray<DeserializerDescriptor> Deserializers { get; }
    public DeserializerDescriptor? CurrentDeserializer { get; set => SetField(ref field, value); }
    public string Text { get; set => SetField(ref field, value); }
    public ICommand PasteCommand { get; }
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public PasteWindowViewModel() : this(null) { }
    public PasteWindowViewModel(string? text)
    {
        Text = text ?? string.Empty;
        PasteCommand = new RelayCommand(Paste, () => CurrentDeserializer is not null);
    }

    private void Paste()
    {
        try
        {
            var deserializer = (DeserializerDescriptor)CurrentDeserializer!;
            var receipt = deserializer.Deserialize.Invoke(Text);
            Workspace.Instance.Receipts.Add(receipt);
            ReceiptPanel.UpdateReceiptList();
        }
        catch(Exception e)
        {
            Logger.Error(e, "Не удалось вставить чек.");
            MessageBox.Show("Не удалось вставить чек. Подробная запись в журнале.",
                "", 0, MessageBoxImage.Error);

        }
        // Close();
    }

    public readonly record struct DeserializerDescriptor(
        string DisplayName, 
        Func<ReadOnlySpan<char>, Receipt> Deserialize);
}
