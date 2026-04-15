using System.Windows.Media;
using RetailCorrector.Utils;
using Correctum.Core.Models;

namespace RetailCorrector.Data
{
    internal readonly struct DisplayReceipt(Receipt origin)
    {
        public string Id { get; } = origin.Id.ToString("D");
        public string RegId { get; } = origin.RegId ?? "";
        public string StorageId { get; } = origin.StorageId ?? "";
        public string DocNumb { get; } = origin.DocNumber.ToString() ?? "";
        public string SessionId { get; } = origin.SessionId.ToString() ?? "";
        public string DocInSession { get; } = origin.DocInSession.ToString() ?? "";
        public string Date { get; } = $"{origin.Created:yyyy'-'MM'-'dd' 'HH':'mm}";
        public string Sign { get; } = $"{origin.DocSign}";
        public string Type { get; } = EnumLocales.ReceiptTypes.GetLocale(origin.Type); 
        public string Cash { get; } = origin.Payment.CashSum.ToString();
        public string eCash { get; } = origin.Payment.NonCashSum.ToString();
        public string PrePay { get; } = origin.Payment.AdvanceSum.ToString();
        public string PostPay { get; } = origin.Payment.CreditSum.ToString();
        public string OtherPay { get; } = origin.Payment.ProvisionSum.ToString();
        public string Total { get; } = origin.TotalSum.ToString();
        public string TaxationSystem { get; } = EnumLocales.TaxationSystems.GetLocale(origin.TaxationSystem);

        public override string ToString() => $"Чек №{DocNumb} (ККТ: {RegId}; ФН: {StorageId})";
    }
}
