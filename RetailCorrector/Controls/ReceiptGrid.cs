using System.Windows.Controls;

namespace RetailCorrector.Controls
{
    class ReceiptGrid : BaseDataGrid
    {
        public ReceiptGrid() : base()
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;

            AddColumn("Дата", "Date", 121);
            AddColumn("ФПД", "Sign", 93);
            AddColumn("Операция", "Type", 114);
            AddColumn("Налично", "Cash", 158);
            AddColumn("Безналично", "eCash", 158);
            AddColumn("Предоплата", "PrePay", 158);
            AddColumn("Кредит", "PostPay", 158);
            AddColumn("Иная оплата", "OtherPay", 158);
            AddColumn("Итог", "Total", 158);
        }
    }
}
