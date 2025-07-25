using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace System.Windows
{
    internal static class UIGenerators
    {
        public static TextBlock Text(string text) => new()
        {
            Text = text,
            VerticalAlignment = VerticalAlignment.Center,
        };

        public static DatePicker DatePicker (
            DateTime min, DateTime max, string propertyName,
            [CallerArgumentExpression(nameof(min))] string minPropName = "",
            [CallerArgumentExpression(nameof(max))] string maxPropName = "")
        {
            var dtPicker = new DatePicker();
            dtPicker.SetBinding(Controls.DatePicker.SelectedDateProperty, new Binding(propertyName));
            if (string.IsNullOrWhiteSpace(minPropName))
                dtPicker.DisplayDateStart = min;
            else
                dtPicker.SetBinding(Controls.DatePicker.DisplayDateStartProperty, new Binding(minPropName) { Mode = BindingMode.OneWay });
            if (string.IsNullOrWhiteSpace(maxPropName))
                dtPicker.DisplayDateEnd = max;
            else
                dtPicker.SetBinding(Controls.DatePicker.DisplayDateEndProperty, new Binding(maxPropName) { Mode = BindingMode.OneWay });
            return dtPicker;
        }

        public static ProgressBar ProgressBar(
            int min, int max, string propertyName,
            [CallerArgumentExpression(nameof(min))] string minPropName = "",
            [CallerArgumentExpression(nameof(max))] string maxPropName = "")
        {
            var pBar = new ProgressBar();
            pBar.SetBinding(RangeBase.ValueProperty, new Binding(propertyName));
            if (string.IsNullOrWhiteSpace(minPropName))
                pBar.Minimum = min;
            else
                pBar.SetBinding(RangeBase.MinimumProperty, new Binding(minPropName) { Mode = BindingMode.OneWay });
            if (string.IsNullOrWhiteSpace(maxPropName))
                pBar.Maximum = max;
            else
                pBar.SetBinding(RangeBase.MaximumProperty, new Binding(maxPropName) { Mode = BindingMode.OneWay });
            return pBar;
        }

        public static TextBox TextBox(string propertyName)
        {
            var tb = new TextBox();
            tb.SetBinding(Controls.TextBox.TextProperty, new Binding(propertyName));
            return tb;
        }
    }
}
