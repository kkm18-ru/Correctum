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
        public string Cash { get; } = $"{(decimal)origin.Payment.Cash:F2}";
        public string eCash { get; } = $"{(decimal)origin.Payment.ECash:F2}";
        public string PrePay { get; } = $"{(decimal)origin.Payment.Pre:F2}";
        public string PostPay { get; } = $"{(decimal)origin.Payment.Post:F2}";
        public string OtherPay { get; } = $"{(decimal)origin.Payment.Provision:F2}";
        public string Total { get; } = $"{(decimal)origin.TotalSum:F2}";
        public string TaxationSystem { get; } = EnumLocales.TaxationSystems.GetLocale(origin.TaxationSystem);

        public Brush Background { get; } = origin.IsReadOnly switch
        {
            true => Brushes.LightGray,
            false => Brushes.White
        };

        public override string ToString() => $"Чек №{DocNumb} (ККТ: {RegId}; ФН: {StorageId})";
    }
}
