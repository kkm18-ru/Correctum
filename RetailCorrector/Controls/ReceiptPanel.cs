using RetailCorrector.Data;
using RetailCorrector.Parsers;
using RetailCorrector.Utils;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace RetailCorrector.Controls
{
    internal class ReceiptPanel : DockPanel, INotifyPropertyChanged
    {
        private int _maxCount = 200;
        public int MaxCountReceipts
        {
            get => _maxCount;
            set
            {
                if (value == _maxCount) return;
                _maxCount = value;
                OnPropertyChanged();
                UpdateReceiptList();
            }
        }

        public int _index = 1;
        public int IndexPage
        {
            get => _index;
            set
            {
                if (value == _index || value < 1 || value > CachedCountPages) return;
                _index = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEnabledPrevButtons));
                OnPropertyChanged(nameof(IsEnabledNextButtons));
                Refresh();
            }
        }

        private int _cacheCountPages = 0;
        public int CachedCountPages
        {
            get => _cacheCountPages;
            set
            {
                if (_cacheCountPages == value) return;
                _cacheCountPages = value;
                OnPropertyChanged();
                if (IndexPage > value) IndexPage = value;
                else OnPropertyChanged(nameof(IsEnabledNextButtons));
            }
        }

        public bool IsEnabledPrevButtons => IndexPage > 1;
        public bool IsEnabledNextButtons => IndexPage < CachedCountPages;

        private ReceiptGrid grid;

        private static ReceiptPanel instance;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string property = "") =>
            PropertyChanged?.Invoke(this, new(property));

        public ReceiptPanel()
        {
            DataContext = instance = this;

            var toolPanel = new WrapPanel();
            DockPanel.SetDock(toolPanel, Dock.Top);
            Children.Add(toolPanel);

            grid = new ReceiptGrid();
            Children.Add(grid);

            grid.SelectedCellsChanged += Grid_SelectedCellsChanged;

            var btnRefresh = new Button
            {
                Content = "🗘",
                RenderTransform = new RotateTransform(90, 11, 11),
                Height = 22,
                Width = 22,
                Margin = new(5),
                Command = new RelayCommand(instance.Refresh)
            };
            toolPanel.Children.Add(btnRefresh);

            var btnToBeginning = new Button
            {
                Content = "<<",
                Height = 22,
                Width = 22,
                Margin = new(5),
                Command = new RelayCommand(_ => instance.IndexPage = 1)
            };
            btnToBeginning.SetBinding(Button.IsEnabledProperty, new Binding(nameof(IsEnabledPrevButtons)) { Mode = BindingMode.OneWay });
            toolPanel.Children.Add(btnToBeginning);

            var btnToPrev = new Button
            {
                Content = "<",
                Height = 22,
                Width = 22,
                Margin = new(0,5,5,5),
                Command = new RelayCommand(_ => instance.IndexPage--)
            };
            btnToPrev.SetBinding(Button.IsEnabledProperty, new Binding(nameof(IsEnabledPrevButtons)) { Mode = BindingMode.OneWay });
            toolPanel.Children.Add(btnToPrev);

            var ib = new IntegerUpDown
            {
                Height = 22,
                Width = 55,
                Margin = new(0, 5, 5, 5),
                ShowButtonSpinner = false
            };
            ib.SetBinding(IntegerUpDown.ValueProperty, nameof(IndexPage));
            toolPanel.Children.Add(ib);

            var btnToNext = new Button
            {
                Content = ">",
                Height = 22,
                Width = 22,
                Margin = new(0, 5, 5, 5),
                Command = new RelayCommand(_ => instance.IndexPage++)
            };
            btnToNext.SetBinding(Button.IsEnabledProperty, new Binding(nameof(IsEnabledNextButtons)) { Mode = BindingMode.OneWay });
            toolPanel.Children.Add(btnToNext);

            var btnToEnd = new Button
            {
                Content = ">>",
                Height = 22,
                Width = 22,
                Margin = new(0, 5, 5, 5),
                Command = new RelayCommand(_ => instance.IndexPage = CachedCountPages)
            };
            btnToEnd.SetBinding(Button.IsEnabledProperty, new Binding(nameof(IsEnabledNextButtons)) { Mode = BindingMode.OneWay });
            toolPanel.Children.Add(btnToEnd);

            var cb = new ComboBox
            {
                Height = 22,
                Width = 88,
                Margin = new(5),
                ItemsSource = (KeyValuePair<int, string>[])
                [
                    new(50, "50 чеков"),
                    new(100, "100 чеков"),
                    new(200, "200 чеков"),
                    new(500, "500 чеков"),
                    new(1000, "1000 чеков"),
                    new(2500, "2500 чеков"),
                    new(5000, "5000 чеков"),
                    new(10000, "10000 чеков"),
                ],
                DisplayMemberPath = "Value",
                SelectedValuePath = "Key"
            };
            cb.SetBinding(ComboBox.SelectedValueProperty, nameof(MaxCountReceipts));
            toolPanel.Children.Add(cb);

            var btnParser = new Button
            {
                Content = "  Парсер  ",
                Height = 22,
                Margin = new(5, 5, 5, 5),
                Command = new RelayCommand(_ => Parser.Show())
            };
            toolPanel.Children.Add(btnParser);
        }

        private void Grid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var @ref = Workspace.Instance.Receipts;
            var columnCount = ((DataGrid)sender).Columns.Count;

            var addedCount = e.AddedCells.Count / columnCount;
            for(var i = 0; i < addedCount; i++)
            {
                var item = (DisplayReceipt)e.AddedCells[i * columnCount].Item;
                var row = grid.Items.IndexOf(item);
                foreach (var p in @ref[row + _maxCount * (_index - 1)].Items)
                    PositionGrid.Instance.Items.Add(new DisplayPostion(p, item));
            }

            var removedCount = e.RemovedCells.Count / columnCount;
            for (var i = 0; i < removedCount; i++)
            {
                var item = (DisplayReceipt)e.RemovedCells[i * columnCount].Item;
                var row = grid.Items.IndexOf(item);
                var ps = PositionGrid.Instance.Items.Cast<DisplayPostion>().Where(p => p.Receipt == item.ToString()).ToArray();
                for (var j = 0; j < ps.Length; j++) PositionGrid.Instance.Items.Remove(ps[j]);
            }

            PositionGrid.Instance.Items.Refresh();
        }

        private void Refresh(object? _ = null)
        {
            PositionGrid.Instance.Items.Clear();
            PositionGrid.Instance.Items.Refresh();
            grid.Items.Clear();
            if (CachedCountPages == 0) return;
            var page = Workspace.Instance.Receipts.Chunk(MaxCountReceipts).ToArray()[_index - 1];
            foreach (var item in page)
                grid.Items.Add(new DisplayReceipt(item));
            grid.Items.Refresh();
        }

        public static void UpdateReceiptList()
        {
            instance.CachedCountPages = Workspace.Instance.Receipts.Chunk(instance.MaxCountReceipts).Count();
            instance.Refresh();
        }
    }
}
