using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Data
{
    public class Condition(int num, bool pos = false, int part = -1, int opcode = -1, string val = "") : INotifyPropertyChanged
    {
        public int Number
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                OnPropertyChanged();
            }
        } = num;

        public bool IsPosition
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                OnPropertyChanged();
                PartId = -1;
                Parts.Clear();
                foreach (var part in value ? PositionParts : ReceiptParts)
                    Parts.Add(part);
            }
        } = pos;
        public string TypeName => Types[IsPosition];

        public int PartId
        {
            get;
            set
            {
                if (field == value) return;
                if (field == -1 || value == -1 || Parts[field].Type != Parts[value].Type)
                {
                    OperationId = -1;
                    Operations.Clear();
                    if (value != -1)
                    {
                        var arr = Parts[value].Type switch
                        {
                            PartType.Text => TextOperations,
                            PartType.Integer => IntegerOperations,
                            PartType.Double => DoubleOperations,
                            _ => []
                        };
                        foreach (var part in arr)
                            Operations.Add(part);
                    }
                }
                field = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PartText));
            }
        } = part;
        public string PartText => PartId == -1 ? "" : Parts[PartId].Name;

        public int OperationId
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(OperationText));
            }
        } = opcode;
        public string OperationText => OperationId == -1 ? "" : Operations[OperationId].Name;

        public string Value
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                OnPropertyChanged();
            }
        } = val;

        public static Dictionary<bool, string> Types = new()
        {
            { false, "Чек" },
            { true, "Позиция" }
        };

        private readonly static PartInfo[] ReceiptParts = [
            new(0, "Налично", PartType.Double),
            new(1, "Безналично", PartType.Double),
            new(2, "Аванс", PartType.Double),
            new(3, "Кредит", PartType.Double),
            new(4, "ВП", PartType.Double),
            new(5, "Итог", PartType.Double),
        ];
        private readonly static PartInfo[] PositionParts = [
            new(0, "Наименование", PartType.Text),
            new(1, "ЕИ", PartType.Integer),
            new(2, "Тип", PartType.Integer),
            new(3, "Метод оплаты", PartType.Integer),
            new(4, "Ставка НДС", PartType.Integer),
            new(5, "Цена", PartType.Double),
            new(6, "Количество", PartType.Double),
            new(7, "Стоимость", PartType.Double),
        ];

        private readonly static OperationInfo[] TextOperations = [
            new(0, "содержит", (t, a) => ((string)t).Contains((string)a)),
            new(1, "не содержит", (t, a) => !((string)t).Contains((string)a)),
        ];
        private readonly static OperationInfo[] IntegerOperations = [
            new(0, "=", (t, a) => ((int)t) == ((int)a)),
            new(1, "≠", (t, a) => ((int)t) != ((int)a)),
        ];
        private readonly static OperationInfo[] DoubleOperations = [
            new(0, "<", (t, a) => ((double)t) < ((double)a)),
            new(1, "≤", (t, a) => ((double)t) <= ((double)a)),
            new(2, "≠", (t, a) => ((double)t) != ((double)a)),
            new(3, "=", (t, a) => ((double)t) == ((double)a)),
            new(4, "≥", (t, a) => ((double)t) >= ((double)a)),
            new(5, ">", (t, a) => ((double)t) > ((double)a)),
        ];

        public ObservableCollection<PartInfo> Parts { get; } = new(ReceiptParts);
        public ObservableCollection<OperationInfo> Operations { get; } = [];

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string property = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        public readonly struct PartInfo(int id, string name, PartType type)
        {
            public int Id { get; } = id;
            public string Name { get; } = name;
            public PartType Type { get; } = type;
        }

        public readonly struct OperationInfo(int id, string name, Func<object, object, bool> action)
        {
            public int Id { get; } = id;
            public string Name { get; } = name;
            public Func<object, object, bool> Action { get; } = action;
        }

        public enum PartType { Text, Integer, Double }
    }
    }
