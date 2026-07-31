using Correctum.Utils;
using Correctum.Utils.Integrations.Clients;
using System.Windows.Input;

namespace Correctum.Abstractions;

public abstract class OfdImport<TClient> : BaseImport
    where TClient : ApiClient<TClient>, new()
{
    public bool Authorized { get; private set => SetField(ref field, value); }
    public bool Authorizing { get; private set => SetField(ref field, value); }
    public ICommand AuthCommand { get; }

    protected readonly TClient client;

    protected OfdImport()
    {
        client = new TClient();
        AuthCommand = new RelayCommand(Auth, () => !Authorizing);
    }

    public async void Auth()
    {
        try
        {
            Authorizing = true;
            await InternalAuthAsync();
            Authorized = true;
        }
        catch (Exception e)
        {
            Authorized = false;
            logger.Error(e, "При авторизации в ОФД произошла ошибка.");
        }
        finally
        {
            Authorizing = false;
        }
    }

    protected abstract Task InternalAuthAsync();

    public override void OnClosing()
    {
        client.Dispose();
        base.OnClosing();
    }
}
