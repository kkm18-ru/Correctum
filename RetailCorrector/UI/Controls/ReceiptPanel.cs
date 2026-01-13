using RetailCorrector.Data;
using Serilog;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace RetailCorrector.UI.Controls
{
    internal partial class ReceiptPanel : DockPanel, INotifyPropertyChanged
    {
        public int MaxCountReceipts
        {
            get;
            set
            {
                if (value == field) return;
                field = value;
                OnPropertyChanged();
                UpdateReceiptList();
            }
        } = 200;

        public int IndexPage
        {
            get;
            set
            {
                if (value == field || value < 1 || value > CachedCountPages) return;
                field = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEnabledPrevButtons));
                OnPropertyChanged(nameof(IsEnabledNextButtons));
                Refresh();
            }
        } = 1;

        public int CachedCountPages
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                OnPropertyChanged();
                if (IndexPage > value) IndexPage = value;
                else OnPropertyChanged(nameof(IsEnabledNextButtons));
            }
        } = 0;

        public bool IsEnabledPrevButtons => IndexPage > 1;
        public bool IsEnabledNextButtons => IndexPage < CachedCountPages;

        private readonly ReceiptGrid grid;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string property = "") =>
            PropertyChanged?.Invoke(this, new(property));

        public static ReceiptPanel Instance => field ??= new ReceiptPanel();

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
                ],
                DisplayMemberPath = "Value",
                SelectedValuePath = "Key"
            };
            cb.SetBinding(ComboBox.SelectedValueProperty, nameof(MaxCountReceipts));
            toolPanel.Children.Add(cb);

            var btnImporter = new ImporterMenuButton
            {
                Height = 22,
                Margin = new(5),
            };
            toolPanel.Children.Add(btnImporter);

            var btnActions = new ActionMenuButton
            {
                Height = 22,
                Margin = new(0, 5, 5, 5),
            };
            toolPanel.Children.Add(btnActions);
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
                foreach (var p in @ref[row + MaxCountReceipts * (IndexPage - 1)].Items)
                    PositionGrid.Instance.Collection.Add(new DisplayPostion(p, item));
            }

            var removedCount = e.RemovedCells.Count / columnCount;
            for (var i = 0; i < removedCount; i++)
            {
                if (e.RemovedCells[i * columnCount].Item is not DisplayReceipt item) continue;
                var ps = PositionGrid.Instance.Collection.Where(p => p.Receipt == item.ToString()).ToArray();
                for (var j = 0; j < ps.Length; j++)
                    PositionGrid.Instance.Collection.Remove(ps[j]);
            }
        }

        private void Refresh()
        {
            PositionGrid.Instance.Collection.Clear();
            grid.Collection.Clear();
            if (CachedCountPages == 0) return;
            if (Workspace.Instance.Receipts.Count == 0) return;
            var page = Workspace.Instance.Receipts.Skip(MaxCountReceipts * (IndexPage - 1)).Take(MaxCountReceipts);
            foreach (var item in page)
                grid.Collection.Add(new DisplayReceipt(item));
            ((ListCollectionView)CollectionViewSource.GetDefaultView(grid.Collection)).Refresh();
        }

        public static void UpdateReceiptList()
        {
            Instance.CachedCountPages = Workspace.Instance.Receipts.Chunk(Instance.MaxCountReceipts).Count();
            Instance.Refresh(); 
            Log.Information("Список чеков обновлен");
        }
    }
}
