using RetailCorrector.Enums;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace RetailCorrector.Controls
{
    internal class ReceiptPanel : DockPanel, INotifyPropertyChanged
    {
        private readonly ScrollBar scroll;
        private readonly ReceiptGrid grid;

        private int _index;
        public int BeginIndex
        {
            get => _index;
            set
            {
                if (_index == value) return;
                var diff = value - _index;
                for (var i = Math.Abs(diff); i > 0; i--)
                {
                    if (_currMaxCount <= grid.Items.Count)
                    {
                        var index = diff > 0 ? 0 : _currMaxCount - diff;
                        grid.Items.RemoveAt(index);
                    }
                    var item = Workspace.Instance.Receipts[value - i];
                    var display = new
                    {
                        Date = $"{item.Created:yyyy'-'MM'-'dd' 'HH':'mm}",
                        Sign = $"{item.DocSign}",
                        Type = item.Type switch
                        {
                            Operation.Income => "Приход",
                            Operation.Outcome => "Расход",
                            Operation.RefundIncome => "Возврат прихода",
                            Operation.RefundOutcome => "Возврат расхода"
                        },
                        Cash = $"{(item.Payment.Cash * 0.01):C2}",
                        eCash = $"{(item.Payment.ECash * 0.01):C2}",
                        PrePay = $"{(item.Payment.Pre * 0.01):C2}",
                        PostPay = $"{(item.Payment.Post * 0.01):C2}",
                        OtherPay = $"{(item.Payment.Provision * 0.01):C2}",
                        Total = $"{(item.Payment.TotalSum * 0.01):C2}",
                    };
                    if (diff > 0) grid.Items.Add(display);
                    else grid.Items.Insert(0, display);
                }
                _index = value;
                OnPropertyChanged();
            }
        }

        public int MaxIndex
        {
            get
            {
                var count = Workspace.Instance.Receipts.Count;
                var res = count - _currMaxCount;
                if (res >= 0) return res;
                return 0;
            }
        }

        private int _currMaxCount = 0;

        public ReceiptPanel()
        {
            DataContext = this;

            scroll = new ScrollBar
            {
                ViewportSize = 1,
                SmallChange = 1
            };
            grid = new ReceiptGrid();

            scroll.SetBinding(ScrollBar.MaximumProperty, new Binding(nameof(MaxIndex)) { Mode = BindingMode.OneWay});
            scroll.SetBinding(ScrollBar.ValueProperty, new Binding(nameof(BeginIndex)));

            grid.SizeChanged += Receipts_SizeChanged;
            Workspace.Instance.Receipts.CollectionChanged += Receipts_CollectionChanged;

            DockPanel.SetDock(scroll, Dock.Right);
            Children.Add(scroll);
            Children.Add(grid);
        }

        private void Receipts_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(MaxIndex));
            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                if (grid.Items.Count < _currMaxCount)
                {
                    for(var i = 0; i < Workspace.Instance.Receipts.Count; i++)
                    {
                        var item = Workspace.Instance.Receipts[i];
                        var display = new
                        {
                            Date = $"{item.Created:yyyy'-'MM'-'dd' 'HH':'mm}",
                            Sign = $"{item.DocSign}",
                            Type = item.Type switch
                            {
                                Operation.Income => "Приход",
                                Operation.Outcome => "Расход",
                                Operation.RefundIncome => "Возврат прихода",
                                Operation.RefundOutcome => "Возврат расхода"
                            },
                            Cash = $"{(item.Payment.Cash * 0.01):C2}",
                            eCash = $"{(item.Payment.ECash * 0.01):C2}",
                            PrePay = $"{(item.Payment.Pre * 0.01):C2}",
                            PostPay = $"{(item.Payment.Post * 0.01):C2}",
                            OtherPay = $"{(item.Payment.Provision * 0.01):C2}",
                            Total = $"{(item.Payment.TotalSum * 0.01):C2}",
                        };
                        grid.Items.Add(display);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string property = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        private void Receipts_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            _currMaxCount = (int)Math.Floor((e.NewSize.Height - (1278 > e.NewSize.Width ? 43 : 26)) / 19);
            OnPropertyChanged(nameof(MaxIndex));
        }
    }
}
