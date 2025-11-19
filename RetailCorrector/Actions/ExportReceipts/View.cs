using Microsoft.Win32;
using RetailCorrector.Utils;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.Actions
{
    internal partial class ExportReceipts
    {
        internal class View : ActionView<View, ExportReceipts>
        {
            private readonly Grid content = new();
            public View()
            {
                content.RowDefinitions.Clear();
                content.Children.Clear();
                Content = content;

                content.AddColumn(GridUnitType.Pixel, 115);
                content.AddColumn(GridUnitType.Pixel, 5);
                content.AddColumn(GridUnitType.Pixel, 180);

                content.AddRow(GridUnitType.Auto);
                content.AddRow(GridUnitType.Pixel, 5);
                content.AddRow(GridUnitType.Auto);

                content.AddChild(new TextBlock()
                {
                    Text = "Экспортировать как",
                    VerticalAlignment = VerticalAlignment.Center
                });
                content.AddChild(DrawSelector(), 2);

                content.AddChild(new TextBlock()
                {
                    Text = "Путь экспорта",
                    VerticalAlignment = VerticalAlignment.Center
                }, y: 2);
                var tb = new TextBox();
                tb.SetBinding(TextBox.TextProperty, nameof(Context.Path));
                content.AddChild(tb, 2, 2);
                var btn = new Button
                {
                    Width = 20,
                    Height = 20,
                    Content = "...",
                    HorizontalAlignment = HorizontalAlignment.Right,
                };
                btn.Click += ShowSaveDialog;
                content.AddChild(btn, 2, 2);

                Context.Exporter.InitVisual(content);
            }

            private void ShowSaveDialog(object? s, RoutedEventArgs args)
            {
                if (Context.Exporter.FileFilter is null)
                {
                    var dialog = new OpenFolderDialog();
                    if(dialog.ShowDialog() == true)
                        Context.Path = dialog.FolderName;
                }
                else
                {
                    var dialog = new SaveFileDialog 
                    { 
                        Filter = Context.Exporter.FileFilter,
                    };
                    if (dialog.ShowDialog() == true)
                        Context.Path = dialog.FileName;
                }
            }

            private ComboBox DrawSelector()
            {
                var cb = new ComboBox
                {
                    DisplayMemberPath = "Name",
                    ItemsSource = ModuleRegistry.Exporters,
                };
                cb.SetBinding(ComboBox.SelectedItemProperty, nameof(Context.Exporter));
                cb.SelectionChanged += (_, _) =>
                {
                    if (content.RowDefinitions.Count > 1)
                    {
                        content.RowDefinitions.RemoveRange(1, content.RowDefinitions.Count - 1);
                        content.Children.RemoveRange(2, content.Children.Count - 2);
                    }
                    Context.Path = "";
                    Context.Exporter.InitVisual(content);
                };
                return cb;
            }
        }
    }
}