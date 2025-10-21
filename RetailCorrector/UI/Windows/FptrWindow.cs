using RetailCorrector.Data;
using RetailCorrector.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Kraig.Attributes;

namespace RetailCorrector.UI.Windows
{
    internal partial class FptrWindow : Types.DialogWindow
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
        private nint _driver;
        private readonly nint _library;
        private readonly TextBlock _txtProgress;
        private readonly ProgressBar _barProgress;
        private readonly Button _btnStart;
        private readonly Button _btnCancel;
        private readonly TextBox _inputAct;
        private readonly ComboBox _cbCorrType;
        private readonly ComboBox _cbPrint;

        private readonly CheckBox _chUseOperator;
        private readonly TextBox _tbOperatorName;
        private readonly TextBox _tbOperatorInn;
        
        private CancellationTokenSource? cts;

        [NotifyChanged] private bool _useOperator;
        [NotifyChanged] private string _operatorName = "";
        [NotifyChanged] private string _operatorInn = "";

        public FptrWindow()
        {
            DataContext = this;
            SizeToContent = SizeToContent.WidthAndHeight;
            _library = FptrPresenter.LoadLibrary(FptrPresenter.GetLocationDriver());
            if (FptrNative.libfptr_create(out _driver) != 0)
                throw new Exception("Не удалось создать экземпляр драйвера");
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

            _chUseOperator = new CheckBox
            {
                Content = "Авторизировать оператора?"
            };
            _chUseOperator.SetBinding(CheckBox.IsCheckedProperty, nameof(UseOperator));
            grid.AddChild(_chUseOperator, 0, 8, 3);
            
            grid.AddChild(CreateLabel("ФИО оператора"), y: 10);

            _tbOperatorName = new TextBox();
            _tbOperatorName.SetBinding(TextBox.IsEnabledProperty, nameof(UseOperator));
            _tbOperatorName.SetBinding(TextBox.TextProperty, nameof(OperatorName));
            grid.AddChild(_tbOperatorName, 2, 10);
            
            grid.AddChild(CreateLabel("ИНН оператора"), y: 12);

            _tbOperatorInn = new TextBox();
            _tbOperatorInn.SetBinding(TextBox.IsEnabledProperty, nameof(UseOperator));
            _tbOperatorInn.SetBinding(TextBox.TextProperty, nameof(OperatorInn));
            grid.AddChild(_tbOperatorInn, 2, 12);

            _barProgress = CreateProgress();
            grid.AddChild(_barProgress, 0, 14, 3);
            _txtProgress = CreateProgressLabel();
            grid.AddChild(_txtProgress, 0, 14, 3);

            _btnStart = CreateStart();
            grid.AddChild(_btnStart, y: 16);
            _btnCancel = CreateCancel();
            grid.AddChild(_btnCancel, 2, 16);

            Content = grid;
        }

        private Button CreateConfigurationButton()
        {
            var btn = new Button
            {
                Content = "  Настройка драйвера  "
            };
            btn.Click += (_, _) => FptrNative.libfptr_show_properties(_driver, 0, 
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
            btn.Click += Cancel;
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
                FptrNative.libfptr_open(_driver);
                if (FptrNative.libfptr_device_reboot(_driver) != 0)
                {
                    var code = FptrNative.libfptr_error_code(_driver);
                    MessageBox.Show("Не удалось перезагрузить ККТ", $"Произошла ошибка (Код: {code})", 
                        MessageBoxButton.OK, MessageBoxImage.Hand);
                    Cancel(null, null!);
                    token.ThrowIfCancellationRequested();
                }
                if (UseOperator && !FptrPresenter.LoginOperator(_driver, OperatorName, OperatorInn))
                {
                    var code = FptrNative.libfptr_error_code(_driver);
                    MessageBox.Show("Не удалось зарегистрировать кассира", $"Произошла ошибка (Код: {code})", 
                        MessageBoxButton.OK, MessageBoxImage.Hand);
                    Cancel(null, null!);
                    token.ThrowIfCancellationRequested();
                }
                var @ref = Workspace.Instance.Receipts;
                if (MissingUnitFor120(@ref))
                {
                    MessageBox.Show("На ФФД 1.2 единицы измерения обязаны присутствовать!");
                    return;
                }

                ProgressValue = 0;
                while(ProgressValue < @ref.Count)
                {
                    token.ThrowIfCancellationRequested();
                    if (ProcessingSession() && ProcessingReceipt(@ref[ProgressValue], token))
                        ProgressValue++;
                    else FptrPresenter.CancelReceipt(_driver);
                }
                Cancel(null, null!);
            }
            catch (OperationCanceledException)
            {
                FptrNative.libfptr_cancel_receipt(_driver); 
                FptrNative.libfptr_close(_driver);
            }
        }

        private void Cancel(object? s, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() => _btnCancel.IsEnabled = false);
            Dispatcher.Invoke(() => _btnStart.IsEnabled = true);
            cts?.Cancel();
            cts?.Dispose();
        }

        private bool ProcessingReceipt(Receipt receipt, CancellationToken token)
        {
            FptrPresenter.OpenReceiptArgs openArgs = new();
            Dispatcher.Invoke(() =>
            {
                openArgs.Act = string.IsNullOrWhiteSpace(_inputAct.Text) ? " " : _inputAct.Text;
                openArgs.Print = _cbPrint.SelectedIndex > 0;
                openArgs.CorrType = _cbCorrType.SelectedIndex;
            });
            return FptrPresenter.OpenReceipt(_driver, receipt, openArgs, token) &&
                receipt.Items.All(pos => FptrPresenter.AddPosition(_driver, pos, token)) &&
                FptrPresenter.TotalReceipt(_driver, receipt.TotalSum, token) &&
                FptrPresenter.Payment(_driver, receipt.Payment, token) &&
                0 == FptrNative.libfptr_close_receipt(_driver);
        }

        private bool ProcessingSession()
        {
            if (FptrPresenter.GetSessionState(_driver) is null) return false;
            if (FptrPresenter.GetSessionState(_driver) == 2)
            {
                FptrNative.libfptr_set_param_int(_driver, 65546, 0);
                if (0 != FptrNative.libfptr_report(_driver)) return false;
            }

            if (FptrPresenter.GetSessionState(_driver) != 0) return true;
            return 0 == FptrNative.libfptr_open_shift(_driver);
        }

        private bool MissingUnitFor120(IEnumerable<Receipt> @ref) =>
            FptrPresenter.GetFFD(_driver) == 120 && @ref.SelectMany(r => r.Items).Any(p => p.MeasureUnit is null);

        protected override void OnClosed(EventArgs e)
        {
            if (_driver != 0)
            {
                FptrNative.libfptr_cancel_receipt(_driver);
                FptrNative.libfptr_close(_driver);
                FptrNative.libfptr_destroy(out _driver);
            }
            if(_library != 0)
                FptrNative.FreeLibrary(_library);
        }
    }
}
