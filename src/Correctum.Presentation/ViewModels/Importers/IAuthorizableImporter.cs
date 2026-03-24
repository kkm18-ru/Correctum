using CommunityToolkit.Mvvm.ComponentModel;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;

namespace Correctum.ViewModels.Importers;

public abstract partial class AuthorizableImporterViewModel : ImporterViewModel
{
    [ObservableProperty, NotifyPropertyChangedFor(nameof(CanStart))] private bool m_authorized = false;
    [ObservableProperty] private bool m_authorizing = false;
    public override bool CanStart => Authorized;

    public async void Auth()
    {
        try
        {
            Authorized = false;
            Authorizing = true;
            await InternalAuthAsync();
            Authorized = true;
        }
        catch (Exception ex)
        {
            var mboxParams = new MessageBoxStandardParams
            {
                ContentHeader = ex.Message,
                Icon = Icon.Error,
                ContentMessage = ex.StackTrace?.ToString() ?? "" // todo add debugMode
            };

            MessageBoxManager.GetMessageBoxStandard(mboxParams);
        }
        finally
        {
            Authorizing = false;
        }
    }
    protected abstract Task InternalAuthAsync();
}
