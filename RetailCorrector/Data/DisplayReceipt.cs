using System.Windows.Media;

namespace RetailCorrector.Data
{
    internal readonly struct DisplayReceipt(Receipt origin)
    {
        public string Id { get; } = origin.Id.ToString("D");
        public string Parse { get; } = Workspace.Instance.ParserHistory
            .FirstOrDefault(p=>p.Id == origin.ParseId).ToString();
        public string Date { get; } = $"{origin.Created:yyyy'-'MM'-'dd' 'HH':'mm}";
        public string Sign { get; } = $"{origin.DocSign}";
        public string Type { get; } = origin.Type switch
        {
            Enums.Operation.Income => "Приход",
            Enums.Operation.Outcome => "Расход",
            Enums.Operation.RefundIncome => "Возврат прихода",
            Enums.Operation.RefundOutcome => "Возврат расхода",
            _ => "???"
        }; 
        public string Cash { get; } = $"{(origin.Payment.Cash * 0.01):F2}";
        public string eCash { get; } = $"{(origin.Payment.ECash * 0.01):F2}";
        public string PrePay { get; } = $"{(origin.Payment.Pre * 0.01):F2}";
        public string PostPay { get; } = $"{(origin.Payment.Post * 0.01):F2}";
        public string OtherPay { get; } = $"{(origin.Payment.Provision * 0.01):F2}";
        public string Total { get; } = $"{(origin.TotalSum * 0.01):F2}";
        public string TaxationSystem { get; } = origin.TaxationSystem switch
        {
            Enums.TaxationSystem.OSN => "ОСН",
            Enums.TaxationSystem.USNIncome => "УСН доход",
            Enums.TaxationSystem.USNIncomeExpenditure => "УСН доход-расход",
            Enums.TaxationSystem.ENVD => "ЕНВД",
            Enums.TaxationSystem.ESN => "ЕСН",
            Enums.TaxationSystem.Patent => "Патент",
            _ => "???"
        };

        public Brush Background { get; } = origin.IsReadOnly switch
        {
            true => Brushes.LightGray,
            false => Brushes.White
        };

        public override string ToString() => $"ID: {Id}; Дата: {Date}; ФПД: {Sign}";
    }
}
