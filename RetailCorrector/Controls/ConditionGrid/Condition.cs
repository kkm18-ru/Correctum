using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Controls
{
    internal partial class ConditionGrid
    {
        public class Condition(int num, bool pos = false, int opcode = 0, string val = "") : INotifyPropertyChanged
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
                }
            }

            private int _operationId = opcode;
            public int OperationId
            {
                get => _operationId;
                set
                {
                    if (_operationId == value) return;
                    _operationId = value;
                    OnPropertyChanged();
                }
            }

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

            public event PropertyChangedEventHandler? PropertyChanged;
            private void OnPropertyChanged([CallerMemberName] string property = "") =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
