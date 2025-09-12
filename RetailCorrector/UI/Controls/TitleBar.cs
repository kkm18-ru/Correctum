using RetailCorrector.Attributes;
using RetailCorrector.UI.Windows;
using RetailCorrector.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace RetailCorrector.UI.Controls
{
    [Singleton]
    internal partial class TitleBar : Canvas
    {
        private TitleBar()
        {
            Height = 29;
            Background = SystemColors.AccentColorBrush;

            var tb = new TextBlock
            {
                Text = "RETAILCORRECTOR",
                FontFamily = new("Free 3 of 9"),
                FontSize = 38,
                Foreground = new SolidColorBrush(SystemColors.AccentColor.Subtraction(9, 9, 9))
            };
            Canvas.SetLeft(tb, 5);
            Children.Add(tb);

            var btn = CreateExitButton();
            Canvas.SetRight(btn, 0);
            Children.Add(btn);
            
            btn = CreateButton("");
            btn.Click += (_, _) => new SettingsWindow().ShowDialog();
            Canvas.SetRight(btn, 45);
            Children.Add(btn);

            btn = CreateButton("");
            btn.Click += (_,_) => MainWindow.Instance.WindowState = WindowState.Minimized;
            Canvas.SetRight(btn, 90);
            Children.Add(btn);
        }

        private static Button CreateExitButton()
        {
            var btn = new Button
            {
                Background = SystemColors.AccentColorBrush,
                Template = CreateTemplateButton("", Colors.Red)
            };
            btn.Click += (_, _) => MainWindow.Instance.Close();
            return btn;
        }
        private static Button CreateButton(string text) => new()
        {
            Background = SystemColors.AccentColorBrush,
            Template = CreateTemplateButton(text,
                    SystemColors.AccentColor.AddOrSubtraction(0x22, 0x22, 0x22))
        };

        private static ControlTemplate CreateTemplateButton(string text, Color selectionColor)
        {
            var template = new ControlTemplate();

            #region Графические фабрики
            var content = new FrameworkElementFactory(typeof(TextBlock));
            var border = new FrameworkElementFactory(typeof(Border));
            border.AppendChild(content);

            content.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            content.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            content.SetValue(TextBlock.TextProperty, text);
            content.SetValue(TextBlock.FontSizeProperty, 15.0);
            content.SetValue(TextBlock.ForegroundProperty, new Binding(nameof(Button.Background))
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),
                Converter = new ColorExtensions.TextColorConverter()
            });
            content.SetValue(TextBlock.FontFamilyProperty, new FontFamily("HoloLens MDL2 Assets"));

            border.SetValue(Border.BorderThicknessProperty, new Thickness(0));
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(0));
            border.SetBinding(Border.BackgroundProperty, new Binding(nameof(Button.Background)) 
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) 
            });
            border.SetValue(Border.WidthProperty, 45.0);
            border.SetValue(Border.HeightProperty, 29.0);

            template.VisualTree = border;
            #endregion Графические фабрики

            #region Триггеры
            var trigger = new Trigger
            {
                Property = Button.IsMouseOverProperty,
                Value = true
            };
            var storyboard = new Storyboard();
            var animation = new ColorAnimation(selectionColor, new(TimeSpan.FromMilliseconds(1)));
            Storyboard.SetTargetProperty(animation, new("(Control.Background).(SolidColorBrush.Color)"));
            storyboard.Children.Add(animation);
            var action = new BeginStoryboard
            {
                Storyboard = storyboard
            };
            trigger.EnterActions.Add(action);

            storyboard = new Storyboard();
            animation = new ColorAnimation
            {
                Duration = new(TimeSpan.FromMilliseconds(1))
            };
            Storyboard.SetTargetProperty(animation, new("(Control.Background).(SolidColorBrush.Color)"));
            storyboard.Children.Add(animation);
            action = new BeginStoryboard
            {
                Storyboard = storyboard
            };
            trigger.ExitActions.Add(action);

            template.Triggers.Add(trigger);
            #endregion Триггеры

            return template;
        }
    }
}
