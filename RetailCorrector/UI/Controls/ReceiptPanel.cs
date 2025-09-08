using RetailCorrector.Data;
using RetailCorrector.Utils;
using RetailCorrector.Attributes;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace RetailCorrector.UI.Controls
{
    [Singleton]
    internal partial class ReceiptPanel : DockPanel, INotifyPropertyChanged
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

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string property = "") =>
            PropertyChanged?.Invoke(this, new(property));

        private ReceiptPanel()
        {
            DataContext = this;

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
            };
            btnRefresh.Click += (_, _) => Refresh();
            toolPanel.Children.Add(btnRefresh);

            var btnToBeginning = new Button
            {
                Content = "<<",
                Height = 22,
                Width = 22,
                Margin = new(5),
            };
            btnToBeginning.Click += (_, _) => IndexPage = 1;
            btnToBeginning.SetBinding(Button.IsEnabledProperty, new Binding(nameof(IsEnabledPrevButtons)) { Mode = BindingMode.OneWay });
            toolPanel.Children.Add(btnToBeginning);

            var btnToPrev = new Button
            {
                Content = "<",
                Height = 22,
                Width = 22,
                Margin = new(0,5,5,5),
            };
            btnToPrev.Click += (_, _) => IndexPage--;
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
            };
            btnToNext.Click += (_, _) => IndexPage++;
            btnToNext.SetBinding(Button.IsEnabledProperty, new Binding(nameof(IsEnabledNextButtons)) { Mode = BindingMode.OneWay });
            toolPanel.Children.Add(btnToNext);

            var btnToEnd = new Button
            {
                Content = ">>",
                Height = 22,
                Width = 22,
                Margin = new(0, 5, 5, 5),
            };
            btnToEnd.Click += (_, _) => IndexPage = CachedCountPages;
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

            var btnParser = new ParserMenuButton
            {
                Height = 22,
                Margin = new(5),
            };
            toolPanel.Children.Add(btnParser);

            var btnActions = new ActionMenuButton
            {
                Height = 22,
                Margin = new(0, 5, 5, 5),
            };
            toolPanel.Children.Add(btnActions);

            var btnFptr = new FptrButton
            {
                Height = 22,
                Margin = new(0, 5, 5, 5),
            };
            toolPanel.Children.Add(btnFptr);
        }

        private void Grid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var @ref = Workspace.Instance.Receipts;
            var columnCount = ((DataGrid)sender).Columns.Count;

            var addedCount = e.AddedCells.Count / columnCount;
            for(var i = 0; i < addedCount; i++)
            {
                var item = (DisplayReceipt)e.AddedCells[i * columnCount].Item;
                var row = grid.Collection.IndexOf(item);
                foreach (var p in @ref[row + _maxCount * (_index - 1)].Items)
                    PositionGrid.Instance.Collection.Add(new DisplayPostion(p, item));
            }

            var removedCount = e.RemovedCells.Count / columnCount;
            for (var i = 0; i < removedCount; i++)
            {
                if (e.RemovedCells[i * columnCount].Item is not DisplayReceipt item) continue;
                var row = grid.Collection.IndexOf(item);
                var ps = PositionGrid.Instance.Collection.Where(p => p.Receipt == item.ToString()).ToArray();
                for (var j = 0; j < ps.Length; j++)
                    PositionGrid.Instance.Collection.Remove(ps[j]);
            }
        }

        private void Refresh(object? _ = null)
        {
            PositionGrid.Instance.Collection.Clear();
            grid.Collection.Clear();
            if (CachedCountPages == 0) return;
            if (Workspace.Instance.Receipts.Count == 0) return;
            var page = Workspace.Instance.Receipts.Skip(MaxCountReceipts * (_index - 1)).Take(MaxCountReceipts);
            foreach (var item in page)
                grid.Collection.Add(new DisplayReceipt(item));
            ((ListCollectionView)CollectionViewSource.GetDefaultView(grid.Collection)).Refresh();
        }

        public static void UpdateReceiptList()
        {
            Instance.CachedCountPages = Workspace.Instance.Receipts.Chunk(Instance.MaxCountReceipts).Count();
            Instance.Refresh();
        }
    }
}
