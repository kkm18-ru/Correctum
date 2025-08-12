using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RetailCorrector.Controls
{
    internal class ActionMenu : MenuItem
    {
        public ActionMenu()
        {
            BorderThickness = new(1);
            BorderBrush = new SolidColorBrush(Color.FromRgb(0x70, 0x70, 0x70));
            Background = new SolidColorBrush(Color.FromRgb(0xDD, 0xDD, 0xDD));
            var template = new DataTemplate();
            var fbric = new FrameworkElementFactory(typeof(TextBlock));
            fbric.SetValue(TextBlock.MarginProperty,new Thickness(-20, 0, -50, 0));
            fbric.SetValue(TextBlock.TextProperty, "Действия");
            template.VisualTree = fbric;
            HeaderTemplate = template;
        }
    }
}
