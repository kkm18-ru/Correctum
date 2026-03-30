using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Correctum.Enums;
using Correctum.ViewModels.Windows;
using YamlDotNet.Core.Tokens;

namespace Correctum.Controls;

public sealed class IODialogItem : Button
{
    public string FileName { get; }
    public IODialogItemType Type { get; }
    private OpenFileDialogViewModel _viewModel { get; }

    public IODialogItem(string filename, IODialogItemType type, OpenFileDialogViewModel vm)
    {
        _viewModel = vm;
        FileName = filename;
        Type = type;
        Height = 30;
        var panel = new Grid
        {
            Background = Brushes.Transparent,
            ColumnSpacing = 5,
            ColumnDefinitions =
            [
                new ColumnDefinition(Height, GridUnitType.Pixel),
                new ColumnDefinition(1, GridUnitType.Star),
            ]
        };
        var icon = new Image
        {
            Source = new Bitmap(Type.ToImagePath()),
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
        };
        var text = new TextBlock { Text = FileName };
        Grid.SetColumn(text, 2);
        panel.Children.Add(icon);
        panel.Children.Add(text);
        Content = panel;
    }

    protected override void OnClick() =>
        _viewModel.SelectedItem = this;
}
