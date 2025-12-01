using RetailCorrector.Data;
using RetailCorrector.Utils;
using RetailCorrector.Utils.Integrations;
using System.ComponentModel;
using System.Windows.Input;

namespace RetailCorrector.Parsers;

internal sealed partial class FirstOfd : ParserBase<FirstOfd, FirstOfd.View>, INotifyPropertyChanged
{
    public override string Name => "Первый ОФД";

    #region Свойства
    public string ApiKey
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            PropertyChanged?.Invoke(this, CachedApiKeyArgs);
        }
    } = "";
    #endregion

    #region Команды
    public ICommand AuthCommand => new RelayCommand(_ =>
    {
        client?.Dispose();
        client = new FirstOfdClient(ApiKey);
    });
    #endregion

    #region Кэш аргументов событий
    private static readonly PropertyChangedEventArgs CachedApiKeyArgs = new(nameof(ApiKey));
    #endregion

    public event PropertyChangedEventHandler? PropertyChanged;
    private FirstOfdClient? client;

    public override ParseInfo GenerateInfo() =>
        parseInfo = new ParseInfo(Name);

    public override IAsyncEnumerable<Receipt> Parse(IProgress<long> progress, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}