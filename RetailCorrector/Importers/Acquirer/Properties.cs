using System.Collections.ObjectModel;
using System.ComponentModel;
using RetailCorrector.Enums;

namespace RetailCorrector.Importers;

partial class Acquirer : INotifyPropertyChanged
{
    public string FilePath
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, FilePathEventArg);
        }
    } = "";

    public char Separator
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, SeparatorEventArg);
        }
    } = ';';

    public bool HasHeaders
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, HasHeadersEventArg);
        }
    } = true;
    public int OperationReceipt
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, OperationReceiptEventArg);
        }
    } = -1;
    public int DateReceipt
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, DateReceiptEventArg);
        }
    } = -1;
    public int TotalReceipt
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, TotalReceiptEventArg);
        }
    } = -1;
    public string DateFormat
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, DateFormatEventArg);
        }
    } = "yyyy-MM-dd HH:mm:ss";
    public string ItemName
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, ItemNameEventArg);
        }
    } = "";
    public string IncomePattern
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, IncomePatternEventArg);
        }
    } = "Поступление";
    public string RefundPattern
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, RefundPatternEventArg);
        }
    } = "Возврат";

    public TaxRate ItemVat
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, ItemVatEventArg);
        }
    } = TaxRate.Without;

    public ObservableCollection<string> Headers { get; } = [];

    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private PropertyChangedEventArgs FilePathEventArg = new(nameof(FilePath));
    private PropertyChangedEventArgs SeparatorEventArg = new(nameof(Separator));
    private PropertyChangedEventArgs HasHeadersEventArg = new(nameof(HasHeaders));
    private PropertyChangedEventArgs OperationReceiptEventArg = new(nameof(OperationReceipt));
    private PropertyChangedEventArgs DateReceiptEventArg = new(nameof(DateReceipt));
    private PropertyChangedEventArgs TotalReceiptEventArg = new(nameof(TotalReceipt));
    private PropertyChangedEventArgs DateFormatEventArg = new(nameof(DateFormat));
    private PropertyChangedEventArgs ItemNameEventArg = new(nameof(ItemName));
    private PropertyChangedEventArgs ItemVatEventArg = new(nameof(ItemVat));
    private PropertyChangedEventArgs IncomePatternEventArg = new(nameof(IncomePattern));
    private PropertyChangedEventArgs RefundPatternEventArg = new(nameof(RefundPattern));
}