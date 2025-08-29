using RetailCorrector.Attributes;
using RetailCorrector.Data;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RetailCorrector.UI.Controls
{
    [Singleton]
    internal partial class PositionGrid : BaseDataGrid
    {
        public ObservableCollection<DisplayPostion> Collection { get; } = [];

        private PositionGrid() : base()
        {
            DataContext = this;

            AddStarColumn("Наименование", "Name", 250);
            AddColumn("Тип", "Type", 172);
            AddColumn("Метод оплаты", "PayMethod", 186);
            AddColumn("Ставка НДС", "TaxRate", 81);
            AddColumn("Цена", "Price", 157);
            AddColumn("Количество", "Quantity", 136);
            AddColumn("Стоимость", "Cost", 157);

            var view = (CollectionView)CollectionViewSource.GetDefaultView(Collection);
            view.GroupDescriptions.Add(new PropertyGroupDescription("Receipt"));

            SetBinding(BaseDataGrid.ItemsSourceProperty, new Binding(nameof(Collection)) 
            { 
                Mode = BindingMode.OneWay
            });

            var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            textBlockFactory.SetBinding(TextBlock.TextProperty, new Binding("Name"));
            textBlockFactory.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            textBlockFactory.SetValue(TextBlock.FontSizeProperty, 14.0);
            textBlockFactory.SetValue(TextBlock.MarginProperty, new Thickness(3));

            GroupStyle.Add(new GroupStyle
            {
                HeaderTemplate = new DataTemplate
                {
                    VisualTree = textBlockFactory
                }
            });
        }
    }
}
