using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;

namespace Correctum.Views.Windows;

public class DialogWindow : Window
{
    protected DialogWindow()
    {
        CanResize = false;
        //Owner = (Application.Current as IClassicDesktopStyleApplicationLifetime)!.MainWindow;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        KeyBindings.Add(GenerateCloseBinding());
        CanMaximize = false;
        CanMinimize = false;

        WeakReferenceMessenger.Default.Register<Notification>(this, new(ShowMessageBox));
            
    }

    private KeyBinding GenerateCloseBinding()
    {
        var cmd = new RelayCommand(Close);
        var hotkey = new KeyGesture(Key.Escape);
        return new KeyBinding { Command = cmd, Gesture = hotkey };
    }

    private async void ShowMessageBox(object _, Notification n){
        var messageBoxParams = new MessageBoxStandardParams
        {
            ContentHeader = n.Header,
            ContentMessage = n.Message,
            SizeToContent = SizeToContent.WidthAndHeight,
            Icon = n.Icon,
            ButtonDefinitions = n.Buttons,
            EscDefaultButton = n.CancelButton,
            EnterDefaultButton = n.DefaultButton,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        await Dispatcher.UIThread.InvokeAsync(async () => 
            await MessageBoxManager.GetMessageBoxStandard(messageBoxParams)
                .ShowWindowDialogAsync(this).ConfigureAwait(false));
    }
}
