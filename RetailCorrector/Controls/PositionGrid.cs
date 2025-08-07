namespace RetailCorrector.Controls
{
    internal class PositionGrid : BaseDataGrid
    {
        public PositionGrid() : base()
        {
            AddStarColumn("Наименование", "Name", 250);
            AddColumn("Тип", "Type", 172);
            AddColumn("Метод оплаты", "PayMethod", 186);
            AddColumn("Ставка НДС", "TaxRate", 81);
            AddColumn("Цена", "Price", 157);
            AddColumn("Количество", "Quantity", 136);
            AddColumn("Стоимость", "Cost", 157);

            Items.Add(new
            {
                Name = "Тест",
                Type = "ВНЕРЕАЛИЗАЦИОННЫЙ ДОХОД",
                PayMethod = "ЧАСТИЧНЫЙ РАСЧЕТ И КРЕДИТ",
                TaxRate = "10/110",
                Price = "1 000 000 000.00 руб.",
                Quantity = "100 000.000 кв. см",
                Cost = "1 000 000 000.00 руб.",
            });
        }
    }
}
