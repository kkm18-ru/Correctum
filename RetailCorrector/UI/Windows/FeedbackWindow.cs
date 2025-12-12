using RetailCorrector.UI.Windows.Types;
using RetailCorrector.Utils.Integrations.Clients;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.UI.Windows;

internal class FeedbackWindow : DialogWindow
{
    public FeedbackWindow(): base()
    {
        Title = "Обратная связь";
        Width = 350;
        SizeToContent = SizeToContent.Height;
        var grid = new Grid { Margin = new(7, 5, 7, 5) };
        Content = grid;
        DataContext = new FeedbackViewModel();

        grid.AddRow(GridUnitType.Auto);
        grid.AddRow(GridUnitType.Auto);
        grid.AddRow(GridUnitType.Pixel, 2);
        grid.AddRow(GridUnitType.Auto);
        grid.AddRow(GridUnitType.Auto);
        grid.AddRow(GridUnitType.Pixel, 2);
        grid.AddRow(GridUnitType.Auto);
        grid.AddRow(GridUnitType.Auto);
        grid.AddRow(GridUnitType.Pixel, 2);
        grid.AddRow(GridUnitType.Auto);
        grid.AddRow(GridUnitType.Auto);
        grid.AddRow(GridUnitType.Pixel, 5);
        grid.AddRow(GridUnitType.Auto);

        grid.AddChild(new Label { Content = "Как обращаться к вам?:" });
        grid.AddChild(new TextBox()
            .SetEnabledBinding(FeedbackViewModel.SendIdlePropName)
            .SetBinding(FeedbackViewModel.NamePropName), y: 1);

        grid.AddChild(new Label { Content = "Ваш Email:" }, y: 3);
        grid.AddChild(new TextBox()
            .SetEnabledBinding(FeedbackViewModel.SendIdlePropName)
            .SetBinding(FeedbackViewModel.EmailPropName), y: 4);

        grid.AddChild(new Label { Content = "Заголовок:" }, y: 6);
        grid.AddChild(new TextBox()
            .SetEnabledBinding(FeedbackViewModel.SendIdlePropName)
            .SetBinding(FeedbackViewModel.TitlePropName), y: 7);

        grid.AddChild(new Label { Content = "Текст:" }, y: 9);
        grid.AddChild(new TextBox { MinHeight = 66, MaxHeight = 66, AcceptsReturn = true }
            .SetEnabledBinding(FeedbackViewModel.SendIdlePropName)
            .SetBinding(FeedbackViewModel.TextPropName), y: 10);

        grid.AddChild(new Button { Content = "Отправить", Width = 80 }
            .SetEnabledBinding(FeedbackViewModel.SendIdlePropName)
            .AddClick(DataContext, this), y: 12);
    }
}

file class FeedbackViewModel : INotifyPropertyChanged
{
    public readonly FeedbackClient Client = new();
    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, NameProperty);
        }
    } = "";
    public string Email
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, EmailProperty);
        }
    } = "";
    public string Title
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, TitleProperty);
        }
    } = "";
    public string Text
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, TextProperty);
        }
    } = "";
    public bool SendIdle
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, SendIdleProperty);
        }
    } = true;

    public static readonly string NamePropName = nameof(Name);
    public static readonly string EmailPropName = nameof(Email);
    public static readonly string TitlePropName = nameof(Title);
    public static readonly string TextPropName = nameof(Text);
    public static readonly string SendIdlePropName = nameof(SendIdle);

    private static readonly PropertyChangedEventArgs NameProperty = new(NamePropName);
    private static readonly PropertyChangedEventArgs EmailProperty = new(EmailPropName);
    private static readonly PropertyChangedEventArgs TitleProperty = new(TitlePropName);
    private static readonly PropertyChangedEventArgs TextProperty = new(TextPropName);
    private static readonly PropertyChangedEventArgs SendIdleProperty = new(SendIdlePropName);
}

file static class UIHelpers
{
    public static TextBox SetBinding(this TextBox box, string path)
    {
        box.SetBinding(TextBox.TextProperty, path);
        return box;
    }
    public static T SetEnabledBinding<T>(this T elem, string path)
        where T : FrameworkElement
    {
        elem.SetBinding(UIElement.IsEnabledProperty, path);
        return elem;
    }
    public static Button AddClick(this Button btn, object dataContext, Window window)
    {
        var vm = (FeedbackViewModel)dataContext;
        btn.Click += async (_, _) =>
        {
            vm.SendIdle = false;
            var builder = new StringBuilder();
            builder.AppendLine(vm.Text.Replace("\r", "").Replace("\n", "<br/>"));
            builder.AppendLine();
            builder.AppendLine($"_Пользователь [{vm.Name}](mailto:{vm.Email})_");
            try
            {
                await vm.Client.SendReportAsync(vm.Title, $"> {builder}");
                window.Close();
                MessageBox.Show("Сообщение отправлено!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                vm.SendIdle = true;
            }
        };
        return btn;
    }
}