using RetailCorrector.Data;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RetailCorrector.Controls
{
    class ReceiptGrid : BaseDataGrid
    {
        public ObservableCollection<DisplayReceipt> Collection { get; } = [];

        public ReceiptGrid() : base()
        {
            DataContext = this;

            AddColumn("Дата", "Date", 121);
            AddColumn("ФПД", "Sign", 93);
            AddColumn("Операция", "Type", 114);
            AddColumn("Налично", "Cash", 158);
            AddColumn("Безналично", "eCash", 158);
            AddColumn("Предоплата", "PrePay", 158);
            AddColumn("Кредит", "PostPay", 158);
            AddColumn("Иная оплата", "OtherPay", 158);
            AddColumn("Итог", "Total", 158);

            var view = (CollectionView)CollectionViewSource.GetDefaultView(Collection);
            view.GroupDescriptions.Add(new PropertyGroupDescription("Parse"));

            SetBinding(BaseDataGrid.ItemsSourceProperty, new Binding(nameof(Collection))
            {
                Mode = BindingMode.OneWay
            });

            var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            textBlockFactory.SetBinding(TextBlock.TextProperty, new Binding("Name"));
            textBlockFactory.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            textBlockFactory.SetValue(TextBlock.FontSizeProperty, 14.0);
            textBlockFactory.SetValue(TextBlock.MarginProperty, new Thickness(3));

            var rowStyle = new Style(typeof(DataGridRow));
            rowStyle.Setters.Add(new Setter(DataGridRow.BackgroundProperty, new Binding("Background")));
            RowStyle = rowStyle;

            GroupStyle.Add(new GroupStyle
            {
                Panel = new ItemsPanelTemplate(
                    new FrameworkElementFactory(typeof(VirtualizingStackPanel))
                ),
                HeaderTemplate = new DataTemplate
                {
                    VisualTree = textBlockFactory
                }
            });
        }
    }
}
