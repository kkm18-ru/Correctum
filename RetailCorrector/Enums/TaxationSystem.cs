namespace RetailCorrector.Enums
{
    public enum TaxationSystem
    {
        OSN = 1 << 0,
        USNIncome = 1 << 1,
        USNIncomeExpenditure = 1 << 2,
        [Obsolete("Утратил силу с 1 сентября 2025")]
        ENVD = 1 << 3,
        ESHN = 1 << 4,
        Patent = 1 << 5,
    }
}
