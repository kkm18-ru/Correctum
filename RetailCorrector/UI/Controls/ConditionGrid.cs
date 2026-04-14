using RetailCorrector.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace RetailCorrector.UI.Controls
{
    internal partial class ConditionGrid : INotifyPropertyChanged
    {
        public ObservableCollection<Condition> Conditions { get; } = [];

        public int ConditionIndex
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEnabledRemove));
                OnPropertyChanged(nameof(ForegroundRemove));
            }
        } = -1;
        public bool IsEnabledRemove => ConditionIndex != -1;
        public Brush ForegroundRemove => IsEnabledRemove ? Brushes.Red : Brushes.LightGray;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string property = "") =>
            PropertyChanged?.Invoke(this, new(property));

        public int Do(Action<Receipt> action)
        {
            if (Conditions.Any(c => c.OperationId == -1 || c.Value == ""))
            {
                System.Windows.MessageBox.Show("Найдено некорректное условие!");
                return -1;
            }
            var count = 0;
            foreach (var item in Workspace.Instance.Receipts)
            {
                if (!Check(item)) continue;
                action.Invoke(item);
                count++;
            }
            return count;
        }
        
        public int DoByIndex(Action<int> action)
        {
            if (Conditions.Any(c => c.OperationId == -1 || c.Value == ""))
            {
                System.Windows.MessageBox.Show("Найдено некорректное условие!");
                return -1;
            }
            var count = 0;
            var @ref = Workspace.Instance.Receipts;
            for (var i = @ref.Count - 1; i >= 0; i--)
            {
                var item = @ref[i];
                if (!Check(item)) continue;
                action.Invoke(i);
                count++;
            }
            return count;
        }

        private bool Check(Receipt item)
        {
            foreach (var cond in Conditions)
            {
                var o = cond.Operations[cond.OperationId];
                var p = cond.Parts[cond.PartId];

                object val = p.Type switch
                {
                    Condition.PartType.Text => cond.Value,
                    Condition.PartType.Integer => int.Parse(cond.Value),
                    Condition.PartType.Double => double.Parse(cond.Value),
                };
                if (cond.IsPosition)
                {
                    foreach(var pos in item.Items)
                    {
                        object src = cond.PartId switch
                        {
                            0 => pos.Name,
                            1 => ((int?)pos.Unit) ?? -1,
                            2 => pos.Type,
                            3 => pos.PayMethod,
                            4 => pos.TaxRate,
                            5 => (double)pos.Price,
                            6 => pos.Quantity * 0.001,
                            7 => (double)pos.Cost,
                        };
                        if (!o.Action.Invoke(src, val))
                            return false;
                    }
                }
                else
                {
                    object src = cond.PartId switch
                    {
                        0 => (double)item.Payment.Cash,
                        1 => (double)item.Payment.ECash,
                        2 => (double)item.Payment.Pre,
                        3 => (double)item.Payment.Post,
                        4 => (double)item.Payment.Provision,
                        5 => (double)item.TotalSum,
                    };
                    if(!o.Action.Invoke(src, val))
                        return false;
                }
            }
            return true;
        }
    }
}
