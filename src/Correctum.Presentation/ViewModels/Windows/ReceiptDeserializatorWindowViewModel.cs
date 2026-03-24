using CommunityToolkit.Mvvm.ComponentModel;
using Correctum.ReceiptDeserializator;

namespace Correctum.ViewModels.Windows;

public partial class ReceiptDeserializatorWindowViewModel : ObservableObject
{
    [ObservableProperty] private string m_text = string.Empty;
    [ObservableProperty] private IReceiptDeserializator? m_receiptDeserializator;

    public event Action? Closed;

    public void Submit()
    {
        if (ReceiptDeserializator is null) return;
        try
        {
            var receipt = ReceiptDeserializator.Deserialize(Text);
            Closed?.Invoke();
        }
        catch
        {
            //MessageBox.Show("Не удалось спарсить чек. Подробная запись в журнале.");
        }
    }
}
