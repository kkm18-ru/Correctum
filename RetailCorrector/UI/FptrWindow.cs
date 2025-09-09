using Atol.Drivers10.Fptr;
using RetailCorrector.UI.WindowTypes;
using RetailCorrector.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace RetailCorrector.UI
{
    internal class FptrWindow : DialogWindow
    {
        public int ProgressValue
        {
            get => _progressValue;
            set
            {
                if (_progressValue == value) return;                
                Dispatcher.Invoke(() =>_barProgress.Value = _progressValue = value);
                Dispatcher.Invoke(() =>_txtProgress.Text = $"{_barProgress.Value} / {_barProgress.Maximum}");
            }
        }

        private int _progressValue;
        private readonly IFptr _driver;
        private readonly TextBlock _txtProgress;
        private readonly ProgressBar _barProgress;
        private readonly Button _btnStart;
        private readonly Button _btnCancel;
        private readonly TextBox _inputAct;
        private readonly ComboBox _cbCorrType;
        private readonly ComboBox _cbPrint;
        private CancellationTokenSource? cts;

        public FptrWindow()
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            _driver = new Fptr();
            var grid = new Grid
            {
                Margin = new Thickness(5)
            };

            grid.AddColumn(GridUnitType.Pixel, 93);
            grid.AddColumn(GridUnitType.Pixel, 5);
            grid.AddColumn(GridUnitType.Pixel, 125);

            grid.AddRow(GridUnitType.Pixel, 22);
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Pixel, 22);
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Pixel, 22);
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Pixel, 22);
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Pixel, 22);
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Pixel, 22);

            grid.AddChild(CreateConfigurationButton(), dx: 3);
            
            grid.AddChild(CreateLabel("Печатать ли чек"), y: 2);
            _cbPrint = new ComboBox
            {
                ItemsSource = new[] { "Да", "Нет" },
                SelectedIndex = 0
            };
            grid.AddChild(_cbPrint, 2, 2);
            
            grid.AddChild(CreateLabel("Вид коррекции"), y: 4);
            _cbCorrType = new ComboBox
            {
                ItemsSource = new[] { "Самостоятельная", "По предписанию" },
                SelectedIndex = 0
            };
            grid.AddChild(_cbCorrType, 2, 4);

            var lbl = CreateLabel("Номер акта");
            lbl.ToolTip = new ToolTip
            {
                Content = """
                          Данный параметр нужно использовать только в двух случаях:
                          1) Коррекция выполняется по требованию ФНС
                          2) Коррекция выполняется на кассе с ФФД 1.05, так как чек ранее не был сформирован 
                          """
            }; 
            grid.AddChild(lbl, y: 6);
            _inputAct = new TextBox();
            grid.AddChild(_inputAct, 2, 6);

            _barProgress = CreateProgress();
            grid.AddChild(_barProgress, 0, 8, 3);
            _txtProgress = CreateProgressLabel();
            grid.AddChild(_txtProgress, 0, 8, 3);

            _btnStart = CreateStart();
            grid.AddChild(_btnStart, y: 10);
            _btnCancel = CreateCancel();
            grid.AddChild(_btnCancel, 2, 10);

            Content = grid;
        }

        private Button CreateConfigurationButton()
        {
            var btn = new Button
            {
                Content = "  Настройка драйвера  "
            };
            btn.Click += (_, _) => _driver.showProperties(0, 
                new WindowInteropHelper(MainWindow.Instance).EnsureHandle());
            return btn;
        }

        private static ProgressBar CreateProgress() => new()
        {
            Value = 0, 
            Maximum = Math.Max(Workspace.Instance.Receipts.Count, 1)
        };

        private TextBlock CreateProgressLabel() => new()
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Text = $"0 / {_barProgress.Maximum}"
        };

        private Button CreateStart()
        {
            var btn = new Button
            {
                Content = "Запуск"
            };
            btn.Click += (_, _) =>
            {
                _btnCancel.IsEnabled = true;
                _btnStart.IsEnabled = false;
                cts = new CancellationTokenSource();
                new Thread(() => ProcessingReceipts(cts.Token)) { IsBackground = true }.Start();
            };
            return btn;
        }

        private Button CreateCancel()
        {
            var btn = new Button
            {
                Content = "Отмена",
                IsEnabled = false
            };
            btn.Click += (_, _) =>
            {
                _btnCancel.IsEnabled = false;
                _btnStart.IsEnabled = true;
                cts?.Cancel();
                cts?.Dispose();
            };
            return btn;
        }
        
        private static TextBlock CreateLabel(string text) => new()
        {
            VerticalAlignment = VerticalAlignment.Center,
            Text = $"{text}: "
        };

        private void ProcessingReceipts(CancellationToken token)
        {
            try
            {
                _driver.open();
                var @ref = Workspace.Instance.Receipts;
                if (FptrPresenter.GetFFD(_driver) == 120 && 
                    @ref.SelectMany(r => r.Items).Any(p => p.MeasureUnit is null))
                {
                    MessageBox.Show("На ФФД 1.2 единицы измерения обязаны присутствовать!");
                    return;
                }

                    ProgressValue = 0;
                while(ProgressValue < @ref.Count)
                {
                    if (FptrPresenter.GetSessionState(_driver) is null) goto fail;
                    if (FptrPresenter.GetSessionState(_driver) == Constants.LIBFPTR_SS_EXPIRED)
                    {
                        _driver.setParam(Constants.LIBFPTR_PARAM_REPORT_TYPE, Constants.LIBFPTR_RT_CLOSE_SHIFT);
                        if (0 != _driver.report()) goto fail;
                    }
                    if (FptrPresenter.GetSessionState(_driver) == Constants.LIBFPTR_SS_CLOSED &&
                        0 != _driver.openShift()) goto fail;
                    
                    var receipt = @ref[ProgressValue];
                    token.ThrowIfCancellationRequested();
                    FptrPresenter.OpenReceiptArgs openArgs = new();
                    Dispatcher.Invoke(() =>
                    {
                        openArgs.Act = string.IsNullOrWhiteSpace(_inputAct.Text) ? " " : _inputAct.Text;
                        openArgs.Print = _cbPrint.SelectedIndex > 0;
                        openArgs.CorrType = _cbCorrType.SelectedIndex;
                    });
                    token.ThrowIfCancellationRequested();
                    if (!FptrPresenter.OpenReceipt(_driver, receipt, openArgs)) goto fail;
                    token.ThrowIfCancellationRequested();
                    if (receipt.Items.Any(pos => !FptrPresenter.AddPosition(_driver, pos, token))) goto fail;
                    token.ThrowIfCancellationRequested();
                    if (!FptrPresenter.TotalReceipt(_driver, receipt.TotalSum)) goto fail;
                    token.ThrowIfCancellationRequested();
                    if (!FptrPresenter.Payment(_driver, receipt.Payment)) goto fail;
                    token.ThrowIfCancellationRequested();
                    if (0 != _driver.closeReceipt()) goto fail;
                    ProgressValue++;
                    continue;
                    
                    fail:
                        FptrPresenter.CancelReceipt(_driver);
                }
            }
            catch (OperationCanceledException)
            {
                _driver.cancelReceipt();
                _driver.close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _driver.cancelReceipt();
            _driver.close();
            _driver.destroy();
        }
    }
}
