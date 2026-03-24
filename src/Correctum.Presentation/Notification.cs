using MsBox.Avalonia.Enums;

namespace Correctum;

public class Notification(string message, string header, Icon icon, ButtonEnum buttons, ClickEnum defaultButton, ClickEnum cancelButton)
{
    public string Header { get; } = header;
    public string Message { get; } = message;
    public Icon Icon { get; } = icon;
    public ButtonEnum Buttons { get; } = buttons;
    public ClickEnum DefaultButton { get; } = defaultButton;
    public ClickEnum CancelButton { get; } = cancelButton;

    public Notification(string message, Icon icon, ButtonEnum buttons, ClickEnum defaultButton, ClickEnum cancelButton) : this(message, "", icon, buttons, defaultButton, cancelButton) { }
    public Notification(string message, string header, ButtonEnum buttons, ClickEnum defaultButton, ClickEnum cancelButton) : this(message, header, Icon.None, buttons, defaultButton, cancelButton) { }
    public Notification(string message, ButtonEnum buttons, ClickEnum defaultButton, ClickEnum cancelButton) : this(message, "", Icon.None, buttons, defaultButton, cancelButton) { }
    public Notification(string message, string header, Icon icon, ButtonEnum buttons) : this(message, header, icon, buttons, ClickEnum.Default, ClickEnum.None) { }
    public Notification(string message, Icon icon, ButtonEnum buttons) : this(message, "", icon, buttons, ClickEnum.Default, ClickEnum.None) { }
    public Notification(string message, string header, ButtonEnum buttons) : this(message, header, Icon.None, buttons, ClickEnum.Default, ClickEnum.None) { }
    public Notification(string message, ButtonEnum buttons) : this(message, "", Icon.None, buttons, ClickEnum.Default, ClickEnum.None) { }
    public Notification(string message, string header, Icon icon) : this(message, header, icon, ButtonEnum.Ok, ClickEnum.Ok, ClickEnum.Ok) { }
    public Notification(string message, Icon icon) : this(message, "", icon, ButtonEnum.Ok, ClickEnum.Ok, ClickEnum.Ok) { }
    public Notification(string message, string header) : this(message, header, Icon.None, ButtonEnum.Ok, ClickEnum.Ok, ClickEnum.Ok) { }
    public Notification(string message) : this(message, "", Icon.None, ButtonEnum.Ok, ClickEnum.Ok, ClickEnum.Ok) { }
}
