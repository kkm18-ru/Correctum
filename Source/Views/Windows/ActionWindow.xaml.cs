using System.ComponentModel;
using System.Reflection;

namespace Correctum.Views.Windows;

public partial class ActionWindow
{

#if DEBUG
    public ActionWindow() : this(new object()) { }
#endif

    public ActionWindow(object context)
    {
        DataContext = context;
        Title = context.GetType()
            .GetProperty("Name", BindingFlags.Static | BindingFlags.Public)?
            .GetValue(null) as string ?? string.Empty;
        InitializeComponent();
    }

    private void Submit(object sender, System.Windows.RoutedEventArgs e) => DialogResult = true;
    protected override void OnClosing(CancelEventArgs e) => DialogResult ??= false;
}
