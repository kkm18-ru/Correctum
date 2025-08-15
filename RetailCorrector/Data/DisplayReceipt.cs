namespace RetailCorrector.Data
{
    internal readonly struct DisplayReceipt(Receipt origin)
    {
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
        public string Cash { get; } = $"{(origin.Payment.Cash * 0.01):C2}";
        public string eCash { get; } = $"{(origin.Payment.ECash * 0.01):C2}";
        public string PrePay { get; } = $"{(origin.Payment.Pre * 0.01):C2}";
        public string PostPay { get; } = $"{(origin.Payment.Post * 0.01):C2}";
        public string OtherPay { get; } = $"{(origin.Payment.Provision * 0.01):C2}";
        public string Total { get; } = $"{(origin.TotalSum * 0.01):C2}";

        public override string ToString() => $"Дата: {Date}; ФПД: {Sign}";
    }
}
