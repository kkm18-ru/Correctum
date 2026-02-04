using System.Windows.Media;
using RetailCorrector.Utils;

namespace RetailCorrector.Data
{
    internal readonly struct DisplayReceipt(Receipt origin)
    {
        public string Id { get; } = origin.Id.ToString("D");
        public string RegId { get; } = origin.RegId;
        public string StorageId { get; } = origin.StorageId;
        public string DocNumb { get; } = origin.DocNumber.ToString();
        public string SessionId { get; } = origin.SessionId.ToString();
        public string DocInSession { get; } = origin.DocInSession.ToString();
        public string Date { get; } = $"{origin.Created:yyyy'-'MM'-'dd' 'HH':'mm}";
        public string Sign { get; } = $"{origin.DocSign}";
        public string Type { get; } = EnumLocales.ReceiptTypes.GetLocale(origin.Type); 
        public string Cash { get; } = $"{(origin.Payment.Cash * 0.01):F2}";
        public string eCash { get; } = $"{(origin.Payment.ECash * 0.01):F2}";
        public string PrePay { get; } = $"{(origin.Payment.Pre * 0.01):F2}";
        public string PostPay { get; } = $"{(origin.Payment.Post * 0.01):F2}";
        public string OtherPay { get; } = $"{(origin.Payment.Provision * 0.01):F2}";
        public string Total { get; } = $"{(origin.TotalSum * 0.01):F2}";
        public string TaxationSystem { get; } = EnumLocales.TaxationSystems.GetLocale(origin.TaxationSystem);

        public Brush Background { get; } = origin.IsReadOnly switch
        {
            true => Brushes.LightGray,
            false => Brushes.White
        };

        public override string ToString() => $"Чек №{DocNumb} (ККТ: {RegId}; ФН: {StorageId})";
    }
}
