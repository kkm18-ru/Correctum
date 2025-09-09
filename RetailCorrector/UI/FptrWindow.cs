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
                if (_progressValue != value) return;
                _barProgress.Value = _progressValue = value;                
                _txtProgress.Text = $"{_barProgress.Value} / {_barProgress.Maximum}";
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
                ItemsSource = new[] { "Нет", "Да" },
                SelectedIndex = 1
            };
            grid.AddChild(_cbPrint, 2, 2);
            
            grid.AddChild(CreateLabel("Вид коррекции"), y: 4);
            _cbCorrType = new ComboBox
            {
                ItemsSource = new[] { "Самостоятельная", "По предписанию" },
                SelectedIndex = 0
            };
            grid.AddChild(_cbCorrType, 2, 4);
            
            grid.AddChild(CreateLabel("Номер акта"), y: 6);// todo Добавить подсказку
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
                Dispatcher.Invoke(() => ProgressValue = 0);
                var @ref = Workspace.Instance.Receipts;
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
                    var openArgs = new FptrPresenter.OpenReceiptArgs
                    {
                        Act = string.IsNullOrWhiteSpace(_inputAct.Text) ? " " : _inputAct.Text,
                        Print = _cbPrint.SelectedIndex > 0,
                        CorrType = _cbCorrType.SelectedIndex
                    };
                    if (!FptrPresenter.OpenReceipt(_driver, receipt, openArgs)) goto fail;
                    if (receipt.Items.Any(pos => !FptrPresenter.AddPosition(_driver, pos))) goto fail;
                    if (!FptrPresenter.TotalReceipt(_driver, receipt.TotalSum)) goto fail;
                    if (!FptrPresenter.Payment(_driver, receipt.Payment)) goto fail;
                    if (0 != _driver.closeReceipt()) goto fail;
                    Dispatcher.Invoke(() => ProgressValue++);
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
