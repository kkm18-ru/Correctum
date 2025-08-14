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
            btn.Click += (_, _) => Conditions.Add(new(Conditions.Count + 1));
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
            var btn = new Button { Content = path };
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

            #region Сгенерировано ChatGPT5
            // Создаём шаблон кнопки
            var template = new ControlTemplate(typeof(Button));

            // Корневой элемент шаблона — Border
            var borderFactory = new FrameworkElementFactory(typeof(Border)) { Name = "border" };
            borderFactory.SetValue(Border.BorderBrushProperty, Brushes.White);

            // Внутри Border — ContentPresenter (для текста/контента кнопки)
            var contentPresenterFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenterFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenterFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            borderFactory.AppendChild(contentPresenterFactory);
            template.VisualTree = borderFactory;

            // Триггер для состояния IsEnabled = false
            var trigger = new Trigger
            {
                Property = UIElement.IsEnabledProperty,
                Value = false
            };
            trigger.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.White, "border"));
            template.Triggers.Add(trigger);

            // Триггер для активного состояния
            trigger = new Trigger
            {
                Property = UIElement.IsEnabledProperty,
                Value = true
            };
            trigger.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.White, "border"));
            template.Triggers.Add(trigger);

            btn.Template = template;
            #endregion
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
                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
                SelectionMode = DataGridSelectionMode.Single,
                SelectionUnit = DataGridSelectionUnit.FullRow
            };
            data.SetBinding(DataGrid.SelectedIndexProperty, nameof(ConditionIndex));
            this.AddChild(data, y: 2, dx: 3);

            var column = new DataGridTemplateColumn { CellEditingTemplate = new(), CellTemplate = new(), IsReadOnly = true };
            column.CellTemplate.VisualTree = new(typeof(TextBlock));
            column.CellTemplate.VisualTree.SetBinding(TextBlock.TextProperty, new Binding("Number") { Mode = BindingMode.OneWay });
            column.CellTemplate.VisualTree.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Right);
            data.Columns.Add(column);

            column = new DataGridTemplateColumn { CellEditingTemplate = new(), CellTemplate = new(), Width = new(80) };
            column.CellTemplate.VisualTree = new(typeof(TextBlock));
            column.CellTemplate.VisualTree.SetBinding(TextBlock.TextProperty, new Binding("TypeName") { Mode = BindingMode.OneWay });
            column.CellTemplate.VisualTree.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            column.CellEditingTemplate.VisualTree = new(typeof(ComboBox));
            column.CellEditingTemplate.VisualTree.SetBinding(ComboBox.SelectedValueProperty, new Binding("IsPosition") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            column.CellEditingTemplate.VisualTree.SetValue(ComboBox.ItemsSourceProperty, Condition.Types);
            column.CellEditingTemplate.VisualTree.SetValue(ComboBox.DisplayMemberPathProperty, "Value");
            column.CellEditingTemplate.VisualTree.SetValue(ComboBox.SelectedValuePathProperty, "Key");
            data.Columns.Add(column);

            column = new DataGridTemplateColumn { CellEditingTemplate = new(), CellTemplate = new(), Width = new(115) };
            column.CellTemplate.VisualTree = new(typeof(TextBlock));
            column.CellTemplate.VisualTree.SetBinding(TextBlock.TextProperty, new Binding("PartText") { Mode = BindingMode.OneWay });
            column.CellTemplate.VisualTree.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            column.CellEditingTemplate.VisualTree = new(typeof(ComboBox));
            column.CellEditingTemplate.VisualTree.SetBinding(ComboBox.ItemsSourceProperty, new Binding("Parts") { Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            column.CellEditingTemplate.VisualTree.SetBinding(ComboBox.SelectedValueProperty, new Binding("PartId") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            column.CellEditingTemplate.VisualTree.SetValue(ComboBox.DisplayMemberPathProperty, "Name");
            column.CellEditingTemplate.VisualTree.SetValue(ComboBox.SelectedValuePathProperty, "Id");
            data.Columns.Add(column);

            column = new DataGridTemplateColumn { CellEditingTemplate = new(), CellTemplate = new(), Width = new(101) };
            column.CellTemplate.VisualTree = new(typeof(TextBlock));
            column.CellTemplate.VisualTree.SetBinding(TextBlock.TextProperty, new Binding("OperationText") { Mode = BindingMode.OneWay });
            column.CellTemplate.VisualTree.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            column.CellEditingTemplate.VisualTree = new(typeof(ComboBox));
            column.CellEditingTemplate.VisualTree.SetBinding(ComboBox.ItemsSourceProperty, new Binding("Operations") { Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            column.CellEditingTemplate.VisualTree.SetBinding(ComboBox.SelectedValueProperty, new Binding("OperationId") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            column.CellEditingTemplate.VisualTree.SetValue(ComboBox.DisplayMemberPathProperty, "Name");
            column.CellEditingTemplate.VisualTree.SetValue(ComboBox.SelectedValuePathProperty, "Id");
            column.CellEditingTemplate.VisualTree.SetValue(ComboBox.FontFamilyProperty, new FontFamily("FiraCode"));
            data.Columns.Add(column);

            column = new DataGridTemplateColumn { CellEditingTemplate = new(), CellTemplate = new(), Width = new(1, DataGridLengthUnitType.Star) };
            column.CellTemplate.VisualTree = new(typeof(TextBlock));
            column.CellTemplate.VisualTree.SetBinding(TextBlock.TextProperty, new Binding("Value"));
            column.CellTemplate.VisualTree.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            column.CellEditingTemplate.VisualTree = new(typeof(TextBox));
            column.CellEditingTemplate.VisualTree.SetBinding(TextBox.TextProperty, new Binding("Value") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged});
            data.Columns.Add(column);
        }
    }
}
