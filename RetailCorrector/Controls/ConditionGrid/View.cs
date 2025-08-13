using RetailCorrector.Extensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RetailCorrector.Controls
{
    internal partial class ConditionGrid : Grid
    {
        public ConditionGrid()
        {
            DataContext = this;

            InitColumns();
            InitRows();

            AddPlusButton();
            AddMinusButton();
            AddOrderText();
            AddDataGrid();
        }

        private void InitColumns()
        {
            this.AddColumn(GridUnitType.Pixel, 22);
            this.AddColumn(GridUnitType.Pixel, 22);
            this.AddColumn(GridUnitType.Star);
        }

        private void InitRows()
        {
            this.AddRow(GridUnitType.Pixel, 22);
            this.AddRow(GridUnitType.Pixel, 5);
            this.AddRow(GridUnitType.Pixel, 22);
            this.AddRow(GridUnitType.Star);
        }

        private void AddPlusButton()
        {
            var btn = new Button
            {
                Background = Brushes.White,
                BorderBrush = Brushes.White,
                Content = new Path()
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Fill = Brushes.Green,
                    Data = Geometry.Parse("m0 5l5 0l0-5l5 0l0 5l5 0l0 5l-5 0l0 5l-5 0l0-5l-5 0l0-5")
                }
            };
            btn.Click += (_, _) => Conditions.Add(new(Conditions.Count + 1, 0, 0, ""));
            this.AddChild(btn);
        }

        private void AddMinusButton()
        {
            var path = new Path()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Data = Geometry.Parse("m0 0l15 0l0 5l-15 0l0-5")
            };
            var btn = new Button
            {
                Background = Brushes.White,
                BorderBrush = Brushes.White,
                Content = path
            };
            btn.Click += (_, _) =>
            {
                Conditions.RemoveAt(ConditionIndex);
                var index = 0;
                foreach (var cond in Conditions)
                    cond.Number = ++index;
            };
            btn.SetBinding(Button.IsEnabledProperty, new Binding(nameof(IsEnabledRemove)) { Mode = BindingMode.OneWay });
            path.SetBinding(Path.FillProperty, new Binding(nameof(ForegroundRemove)) { Mode = BindingMode.OneWay });
            this.AddChild(btn, 1);
        }

        private void AddOrderText()
        {
            var tb = new TextBox();
            tb.SetBinding(TextBox.TextProperty, nameof(OrderConditions));
            this.AddChild(tb, y: 2, dx: 3);
        }

        private void AddDataGrid()
        {
            var data = new DataGrid
            {
                HeadersVisibility = DataGridHeadersVisibility.None,
                ItemsSource = Conditions,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                CanUserReorderColumns = false,
                CanUserResizeColumns = false,
                CanUserSortColumns = false,
                CanUserResizeRows = false,
                AutoGenerateColumns = false,
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden
            };
            data.SetBinding(DataGrid.SelectedIndexProperty, nameof(ConditionIndex));
            data.Columns.Add(new DataGridTextColumn { Binding = new Binding("Number") });
            string[] types = ["Чек", "Позиция"];
            data.Columns.Add(new DataGridComboBoxColumn
            {
                SelectedValueBinding = new Binding("SourceId"),
                ItemsSource = types
            });
            data.Columns.Add(new DataGridComboBoxColumn { SelectedValueBinding = new Binding("OperationId") });
            data.Columns.Add(new DataGridTemplateColumn { Width = new(1, DataGridLengthUnitType.Star) });
            this.AddChild(data, y: 3, dx: 3);
        }
    }
}
