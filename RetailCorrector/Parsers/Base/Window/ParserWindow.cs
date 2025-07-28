using RetailCorrector.Extensions;
using RetailCorrector.WindowTypes;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.Parsers
{
    internal class ParserWindow : DialogWindow
    {
        private readonly Parser _context;
        private readonly Grid _content;

        public static ParserWindow? Instance { get; private set; } = null;

        public ParserWindow() : base()
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            DataContext = _context = new();
            Instance = this;

            _context.PropertyChanged += ChildUpdated;

            _content = new Grid() 
            { 
                Margin = new Thickness(5) 
            };

            _content.AddColumn(GridUnitType.Auto);
            _content.AddColumn(GridUnitType.Pixel, 5);
            _content.AddColumn(GridUnitType.Pixel, 80);
            _content.AddColumn(GridUnitType.Pixel, 5);
            _content.AddColumn(GridUnitType.Pixel, 80);

            _content.AddRow(GridUnitType.Auto);
            _content.AddRow(GridUnitType.Pixel, 5);
            _content.AddRow(GridUnitType.Auto);

            _content.AddChild(GenerateComboBox());

            var btnParse = UIGenerators.Button("Запустить", _context.ParseCommand);
            btnParse.SetBinding(Button.IsEnabledProperty, nameof(_context.IsEnabledParse));
            _content.AddChild(btnParse, x: 2);

            var btnCancel = UIGenerators.Button("Отмена", _context.CancelCommand);
            btnCancel.SetBinding(Button.IsEnabledProperty, nameof(_context.IsEnabledCancel));
            _content.AddChild(btnCancel, x: 4);

            _content.AddChild(_context.Child, y: 2, dx: 5);

            Content = _content;
        }

        protected override void OnClosed(EventArgs e)
        {
            Instance = null;
        }

        private void ChildUpdated(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not nameof(_context.Child)) return;
            _content.Children.Remove(_content.Children[^1]);
            _content.AddChild(_context.Child, y: 2, dx: 5);
            _context.Child.SetBinding(Control.IsHitTestVisibleProperty, nameof(_context.IsEnabledParse));
        }

        private ComboBox GenerateComboBox()
        {
            var cb = new ComboBox
            {
                Width = 150,
                ItemsSource = Parser.Parsers,
                DisplayMemberPath = nameof(KeyValuePair<object, object>.Key),
                SelectedValuePath = nameof(KeyValuePair<object, object>.Value)
            };

            cb.SetBinding(ComboBox.IsEnabledProperty, nameof(_context.IsEnabledParse));
            cb.SetBinding(ComboBox.SelectedValueProperty, nameof(_context.Generator));

            return cb;
        }
    }
}
