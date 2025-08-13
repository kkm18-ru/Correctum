using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Controls
{
    internal partial class ConditionGrid
    {
        public class Condition(int num, int src, int opcode, string val) : INotifyPropertyChanged
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

            private int _sourceId = src;
            public int SourceId
            {
                get => _sourceId;
                set
                {
                    if (_sourceId == value) return;
                    _sourceId = value;
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
