using System.ComponentModel;
using System.Runtime.CompilerServices;
using RetailCorrector.Importers;
using RetailCorrector.UI.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using RetailCorrector.Converters;
using RetailCorrector.Utils;

namespace RetailCorrector.UI.Windows
{
    internal sealed class ImportWindow : Types.DialogWindow, INotifyPropertyChanged
    {
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly VisualProgress _progress;

        public int ProgressValue { get; set => SetField(ref field, value); } = 0;
        public int ProgressMaximum { get; set => SetField(ref field, value); } = 0;
        public bool Running
        {
            private get;
            set
            {
                if (!SetField(ref field, value)) return;
                if (!value)
                {
                    _cancellationTokenSource?.Cancel();
                    _cancellationTokenSource?.Dispose();
                }
                else _cancellationTokenSource = new CancellationTokenSource();
            }
        }

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public ImportWindow(ImporterBase importer)
        {
            _progress = new VisualProgress();
            _progress.CurrentValueChanged += (_, value) => ProgressValue = value;
            _progress.MaxValueChanged += (_, value) => ProgressMaximum = value;
            
            SizeToContent = SizeToContent.WidthAndHeight;
            MaxWidth = 361;
            SetBinding(TitleProperty, new Binding(nameof(FormGrid.FormName)) 
            { 
                StringFormat = "Импортер чеков \"{0}\"",
                Source = importer 
            });
            Closing += (_, e) => OnClosing(importer, e);

            var panel = new StackPanel { Margin = new Thickness(5) };
            Content = panel;
            
            importer.SetBinding(IsEnabledProperty,
                new Binding(nameof(Running)) { Source = this, Converter = InvertBooleanConverter.Instance });
            panel.Children.Add(importer);
            
            panel.Children.Add(new Separator { Height = 5, Width = 0});
            
            var controlPanel = new WrapPanel();
            panel.Children.Add(controlPanel);
            
            var progressBar = new TextProgressBar { Width = 125 };
            progressBar.SetBinding(TextProgressBar.ValueProperty,
                new Binding(nameof(ProgressValue)) { Mode = BindingMode.OneWay, Source = this });
            progressBar.SetBinding(TextProgressBar.MaximumProperty,
                new Binding(nameof(ProgressMaximum)) { Mode = BindingMode.OneWay, Source = this });
            controlPanel.Children.Add(progressBar);
            
            controlPanel.Children.Add(new Separator { Width = 5, Height = 0 });
            
            var btnStart = new Button { Content = "Запустить", Width = 100 };
            btnStart.SetBinding(IsEnabledProperty, new Binding(nameof(Running))
                { Mode = BindingMode.OneWay, Source = this, Converter = InvertBooleanConverter.Instance });
            btnStart.Click += async (_, _) =>
            {
                Running = true;
                await ImportAsync(importer, _cancellationTokenSource!.Token);
            };
            controlPanel.Children.Add(btnStart);
            
            controlPanel.Children.Add(new Separator { Width = 5, Height = 0 });
            
            var btnCancel = new Button { Content = "Отмена", Width = 100 };
            btnCancel.SetBinding(IsEnabledProperty, new Binding(nameof(Running)) 
                { Mode = BindingMode.OneWay, Source = this });
            btnCancel.Click += (_, _) => Running = false;
            controlPanel.Children.Add(btnCancel);
        }

        private void OnClosing(ImporterBase importer, CancelEventArgs e)
        {
            Running = false;
            importer.OnClosing(e);
        }

        private async Task ImportAsync(ImporterBase importer, CancellationToken cancellationToken)
        {
            try
            {
                await foreach (var receipt in importer.ImportAsync(_progress, cancellationToken))
                    Workspace.Instance.Receipts.Add(receipt);
                Close();
                ReceiptPanel.UpdateReceiptList();
            }
            catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
            {
                RemoveReceiptsOfCurrentSession();
                Logger.Info(ex, "Выгрузка чеков была отменена пользователем");
                MessageBox.Show("Выгрузка чеков была отменена пользователем", "", 0, MessageBoxImage.Hand);
            }
            catch (Exception ex)
            {
                RemoveReceiptsOfCurrentSession();
                Logger.Error(ex, "Необработаное исключение");
                MessageBox.Show(ex.Message, "Ошибка выгрузки чеков", 0, MessageBoxImage.Error);
            }
            finally
            {
                Running = false;
            }
        }

        private void RemoveReceiptsOfCurrentSession()
        {
            // todo Добавить определение сессии чеков
            for (var i = Workspace.Instance.Receipts.Count - 1; i >= 0; i--)
                Workspace.Instance.Receipts.RemoveAt(i);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
        private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
