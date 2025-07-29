using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace System.Windows
{
    internal static class UIGenerators
    {
        public static TextBlock Text(string text) => new()
        {
            Text = text,
            VerticalAlignment = VerticalAlignment.Center,
        };

        public static DatePicker DatePicker(string propertyName, DateTime min, string maxPropName)
        {
            var dtPicker = new DatePicker();
            dtPicker.SetBinding(Controls.DatePicker.SelectedDateProperty, new Binding(propertyName));
            dtPicker.DisplayDateStart = min;
            dtPicker.SetBinding(Controls.DatePicker.DisplayDateEndProperty, new Binding(maxPropName) { Mode = BindingMode.OneWay });
            return dtPicker;
        }

        public static DatePicker DatePicker(string propertyName, string minPropName, DateTime max)
        {
            var dtPicker = new DatePicker();
            dtPicker.SetBinding(Controls.DatePicker.SelectedDateProperty, new Binding(propertyName));
            dtPicker.SetBinding(Controls.DatePicker.DisplayDateStartProperty, new Binding(minPropName) { Mode = BindingMode.OneWay });
            dtPicker.DisplayDateEnd = max;
            return dtPicker;
        }

        public static TextBox TextBox(string propertyName)
        {
            var tb = new TextBox();
            tb.SetBinding(Controls.TextBox.TextProperty, new Binding(propertyName));
            return tb;
        }

        public static Button Button(string text, ICommand onClick) => new()
        {
            Content = text,
            Command = onClick
        };

        public static ProgressBar ProgressBar(string propertyName, string maxPropName)
        {
            var pBar = new ProgressBar();
            pBar.SetBinding(RangeBase.ValueProperty, new Binding(propertyName) { Mode = BindingMode.OneWay });
            pBar.SetBinding(RangeBase.MaximumProperty, new Binding(maxPropName) { Mode = BindingMode.OneWay });
            return pBar;
        }
    }
}
