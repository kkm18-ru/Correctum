using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MessageDialog
{
    public sealed class MessageDialog : Window
    {
        public MessageDialog(MessageDialogOptions options) : 
            this(Application.Current.MainWindow, options) { }

        public MessageDialog(Window owner, MessageDialogOptions options)
        {
            MessageDialogOptions.Validate(options);
            ConfigureWindow(owner, options.Title ?? "");

            var rootPanel = new DockPanel { Margin = new(10) };
            Content = rootPanel;

            rootPanel.Children.Add(InitializeButtonPanel(options.Buttons));
            rootPanel.Children.Add(InitializeMessagesBlock(options.Message?.Split('\n') ?? []));            
        }

        private void ConfigureWindow(Window owner, string title)
        {
            Background = new SolidColorBrush(Color.FromRgb(0xf0, 0xf0, 0xf0));
            WindowStyle = WindowStyle.ToolWindow;
            SizeToContent = SizeToContent.WidthAndHeight;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = owner;
            Title = title;
        }

        private WrapPanel InitializeButtonPanel(IList<MessageDialogButtonOptions> options)
        {
            var btnPanel = new WrapPanel
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Orientation = Orientation.Horizontal,
            };
            DockPanel.SetDock(btnPanel, Dock.Bottom);
            foreach (var option in options)
            {
                var btn = new Button()
                {
                    Height = 24,
                    Margin = new(5),
                    Padding = new(16, 1, 16, 1),
                    FontSize = 14.4,
                    MinWidth = 90,
                    FontFamily = new("Segoe UI"),
                    Content = option.Text,
                    IsCancel = option.Type == MessageDialogButtonType.Cancel,
                    IsDefault = option.Type == MessageDialogButtonType.Default,
                };
                btn.Click += (_, _) =>
                {
                    Close();
                    option.Action();
                };
                btnPanel.Children.Add(btn);
            }
            return btnPanel;
        }

        private static RichTextBox InitializeMessagesBlock(string[] lines)
        {
            var messagesBlock = new RichTextBox
            {
                MaxHeight = 350,
                MaxWidth = 500,
                IsReadOnly = true,
                IsReadOnlyCaretVisible = false,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Background = null,
                BorderBrush = null,
                Padding = new(0, 10, 0, 10),
                BorderThickness = new(0),
                Document = new FlowDocument()
            };
            foreach (var line in lines)
            {
                var paragraph = new Paragraph
                {
                    Margin = new(10),
                    FontSize = 16,
                    FontFamily = new("Segoe UI"),
                    Foreground = new SolidColorBrush(Colors.Black)
                };
                paragraph.Inlines.Add(line);
                messagesBlock.Document.Blocks.Add(paragraph);
            }
            return messagesBlock;
        }
    }
}
