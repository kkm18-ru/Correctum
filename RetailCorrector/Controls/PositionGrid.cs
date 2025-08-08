namespace RetailCorrector.Controls
{
    internal class PositionGrid : BaseDataGrid
    {
        public static PositionGrid Instance { get; set; }

        public PositionGrid() : base()
        {
            Instance = this;

            AddStarColumn("Наименование", "Name", 250);
            AddColumn("Тип", "Type", 172);
            AddColumn("Метод оплаты", "PayMethod", 186);
            AddColumn("Ставка НДС", "TaxRate", 81);
            AddColumn("Цена", "Price", 157);
            AddColumn("Количество", "Quantity", 136);
            AddColumn("Стоимость", "Cost", 157);
        }
    }
}
