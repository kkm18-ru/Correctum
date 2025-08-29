using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Data
{
    public class Condition(int num, bool pos = false, int part = -1, int opcode = -1, string val = "") : INotifyPropertyChanged
    {
        private int _number = num;
        public int Number
        {
            get => _number;
            set
            {
                if (_number == value) return;
                _number = value;
                OnPropertyChanged();
            }
        }

        private bool _position = pos;
        public bool IsPosition
        {
            get => _position;
            set
            {
                if (_position == value) return;
                _position = value;
                OnPropertyChanged();
                PartId = -1;
                Parts.Clear();
                foreach (var part in value ? PositionParts : ReceiptParts)
                    Parts.Add(part);
            }
        }
        public string TypeName => Types[IsPosition];

        private int _partId = part;
        public int PartId
        {
            get => _partId;
            set
            {
                if (_partId == value) return;
                if (_partId == -1 || value == -1 || Parts[_partId].Type != Parts[value].Type)
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
                _partId = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PartText));
            }
        }
        public string PartText => PartId == -1 ? "" : Parts[PartId].Name;

        private int _operationId = opcode;
        public int OperationId
        {
            get => _operationId;
            set
            {
                if (_operationId == value) return;
                _operationId = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(OperationText));
            }
        }
        public string OperationText => OperationId == -1 ? "" : Operations[OperationId].Name;

        private string _value = val;
        public string Value
        {
            get => _value;
            set
            {
                if (_value == value) return;
                _value = value;
                OnPropertyChanged();
            }
        }

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
