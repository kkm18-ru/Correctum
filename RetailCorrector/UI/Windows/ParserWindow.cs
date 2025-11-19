using RetailCorrector.Parsers;
using RetailCorrector.UI.Controls;
using RetailCorrector.Utils.Extensions;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.UI.Windows
{
    internal class ParserWindow : Types.DialogWindow
    {
        private CancellationTokenSource? cts;
        private readonly Progress<long> progress;
        private readonly IParser parser;

        private readonly Button btnStart;
        private readonly Button btnCancel;
        private readonly TextBlock txtProgress;
        private readonly ProgressBar barProgress;

        public ParserWindow(UserControl view) : this((IParser)view.DataContext) =>
            ((StackPanel)Content).Children.Insert(0, view);

        public ParserWindow(IParser parser)
        {
            #region Основные параметры
            this.parser = parser;
            progress = new(OnChangedProgress);
            SizeToContent = SizeToContent.WidthAndHeight;
            var stack = new StackPanel { Margin = new(5, 5, 5, 5) };
            Content = stack;
            MaxWidth = 361;
            #endregion

            #region Служебная панель
            var grid = new Grid();
            grid.AddColumn(GridUnitType.Pixel, 125);
            grid.AddColumn(GridUnitType.Pixel, 5);
            grid.AddColumn(GridUnitType.Pixel, 100);
            grid.AddColumn(GridUnitType.Pixel, 5);
            grid.AddColumn(GridUnitType.Pixel, 100);
            stack.Children.Add(grid);
            #endregion

            #region Отображение прогресса
            barProgress = new ProgressBar();
            grid.AddChild(barProgress);

            #region Текст прогресса
            txtProgress = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Text = "0 / 0"
            };
            grid.AddChild(txtProgress);
            #endregion
            #endregion

            #region Кнопка запуска
            btnStart = new Button
            {
                Content = "  Запустить  "
            };
            btnStart.Click += StartParse;
            grid.AddChild(btnStart, 2);
            #endregion

            #region Кнопка отмены
            btnCancel = new Button
            {
                Content = "  Отмена  ",
                IsEnabled = false
            };
            btnCancel.Click += CancelParse;
            grid.AddChild(btnCancel, 4);
            #endregion
        }

        private void OnChangedProgress(long value)
        {
            if (value < 0)
            {
                barProgress.Value = 0;
                barProgress.Maximum = -value;
            }
            else barProgress.Value = value;
            txtProgress.Text = $"{barProgress.Value} / {barProgress.Maximum}";
        }

        private async void StartParse(object? s, RoutedEventArgs e)
        {
            btnCancel.IsEnabled = true;
            btnStart.IsEnabled = false;
            cts = new CancellationTokenSource();
            Workspace.Instance.ParserHistory.Add(parser.GenerateInfo());
            try
            {
                await foreach (var receipt in parser.Parse(progress, cts.Token))
                    Workspace.Instance.Receipts.Add(receipt);
                btnCancel.IsEnabled = false;
                btnStart.IsEnabled = true;
                cts.Dispose();
                Close();
                ReceiptPanel.UpdateReceiptList();
            }
            catch (Exception ex)
            {
                var lastParse = Workspace.Instance.ParserHistory[^1];
                for (var i = Workspace.Instance.Receipts.Count - 1; i >= 0; i--)
                {
                    if (Workspace.Instance.Receipts[i].ParseId == lastParse.Id)
                        Workspace.Instance.Receipts.RemoveAt(i);
                }
                Workspace.Instance.ParserHistory.Remove(lastParse);
                if (ex is (OperationCanceledException or TaskCanceledException))
                    MessageBox.Show("Выгрузка чеков была отменена пользователем", "", 0, MessageBoxImage.Hand);
                else 
                {
                    MessageBox.Show(ex.Message, "Ошибка выгрузки чеков", 0, MessageBoxImage.Error);
                    CancelParse(null, null!);
                }
            }
        }

        private void CancelParse(object? s, RoutedEventArgs e)
        {
            btnCancel.IsEnabled = false;
            btnStart.IsEnabled = true;
            cts?.Cancel();
            cts?.Dispose();
        }
    }
}
