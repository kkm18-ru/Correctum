using RetailCorrector.Utils;
using RetailCorrector.UI.WindowTypes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RetailCorrector.UI
{
    internal partial class Parser
    {
        internal class View : DialogWindow
        {
            private readonly Parser _context;
            private readonly Grid _content;

            public static View? Instance { get; private set; } = null;

            public View() : base()
            {
                SizeToContent = SizeToContent.WidthAndHeight;
                DataContext = _context = new();
                Instance = this;

                _context.PropertyChanged += ChildUpdated;

                _content = new Grid() { Margin = new Thickness(5) };

                _content.AddColumn(GridUnitType.Auto);
                _content.AddColumn(GridUnitType.Pixel, 5);
                _content.AddColumn(GridUnitType.Pixel, 80);
                _content.AddColumn(GridUnitType.Pixel, 5);
                _content.AddColumn(GridUnitType.Pixel, 80);

                _content.AddRow(GridUnitType.Auto);
                _content.AddRow(GridUnitType.Pixel, 5);
                _content.AddRow(GridUnitType.Auto);

                _content.AddChild(GenerateComboBox());

                var btnParse = new Button { Content = "Запустить", Command = _context.ParseCommand };
                btnParse.SetBinding(Button.IsEnabledProperty, nameof(_context.IsEnabledParseButton));
                _content.AddChild(btnParse, x: 2);

                var btnCancel = new Button { Content = "Отмена", Command = _context.CancelCommand };
                btnCancel.SetBinding(Button.IsEnabledProperty, nameof(_context.IsEnabledCancel));
                _content.AddChild(btnCancel, x: 4);

                _content.AddChild(_context.Child, y: 2, dx: 5);

                Content = _content;
            }

            protected override void OnClosed(EventArgs e) { Instance = null; }

            private void ChildUpdated(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                if (e.PropertyName is not nameof(_context.Child)) return;
                _content.Children.Remove(_content.Children[^1]);
                _content.AddChild(_context.Child, y: 2, dx: 5);
                _context.Child.SetBinding(Control.IsEnabledProperty,
                    new Binding(nameof(_context.IsEnabledParse))
                    {
                        Mode = BindingMode.OneWay,
                        Source = _context
                    });
            }

            private ComboBox GenerateComboBox()
            {
                var cb = new ComboBox
                {
                    Width = 150,
                    ItemsSource = Parsers,
                    DisplayMemberPath = nameof(KeyValuePair<object, object>.Key),
                    SelectedValuePath = nameof(KeyValuePair<object, object>.Value)
                };

                cb.SetBinding(ComboBox.IsEnabledProperty, nameof(_context.IsEnabledParse));
                cb.SetBinding(ComboBox.SelectedValueProperty, nameof(_context.Generator));

                return cb;
            }
        }
    }
}
