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
            /*
            Items.Add(new
            {
                Date = "2025-12-30 10:50",
                Sign = "123456789012",
                Type = "Возврат прихода",
                Payment =
                """
                Налично:     1 000 000 000.00 руб.
                Безналично:  1 000 000 000.00 руб.
                Предоплата:  1 000 000 000.00 руб.
                Кредит:      1 000 000 000.00 руб.
                Иная оплата: 1 000 000 000.00 руб.
                """,
                Total = "1 000 000 000.00 руб.",
            });*/
        }
    }
}
