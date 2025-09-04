using RetailCorrector.Parsers;
using RetailCorrector.UI.WindowTypes;
using RetailCorrector.Utils;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.UI
{
    internal class ParserWindow : DialogWindow
    {
        private CancellationTokenSource cts;

        public ParserWindow(UserControl view) :
            this((IParser)view.DataContext)
        {
            ((StackPanel)Content).Children.Insert(0, view);
        }

        public ParserWindow(IParser parser)
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            var stack = new StackPanel { Margin = new(5, 5, 5, 5) };
            var panel = DrawPanel(parser);
            stack.Children.Add(panel);
            Content = stack;
            MaxWidth = 361;
        }
        
        private Grid DrawPanel(IParser parser)
        {
            var grid = new Grid();

            grid.AddColumn(GridUnitType.Pixel, 125);
            grid.AddColumn(GridUnitType.Pixel, 5);
            grid.AddColumn(GridUnitType.Pixel, 100);
            grid.AddColumn(GridUnitType.Pixel, 5);
            grid.AddColumn(GridUnitType.Pixel, 100);

            var pBar = new ProgressBar();
            grid.AddChild(pBar);
            var lbl = new TextBlock 
            { 
                HorizontalAlignment = HorizontalAlignment.Center, 
                VerticalAlignment = VerticalAlignment.Center,
                Text = "0 / 0"
            };
            grid.AddChild(lbl);

            var progress = new Progress<long>(value =>
            {
                if (value < 0)
                {
                    pBar.Value = 0;
                    pBar.Maximum = -value;
                }
                else pBar.Value = value;
                lbl.Text = $"{pBar.Value} / {pBar.Maximum}";
            });

            var btn = new Button
            {
                Content = "  Запустить  "
            };
            btn.Click += async (_, _) =>
            {
                cts = new CancellationTokenSource();
                await foreach (var p in parser.Parse(progress, cts.Token)) ;
                Close();
            };
            grid.AddChild(btn,2);

            btn = new Button
            {
                Content = "  Отмена  "
            };
            btn.Click += (_, _) =>
            {
                cts?.Cancel();
                cts?.Dispose();
            };
            grid.AddChild(btn,4);

            return grid;
        }
    }
}
