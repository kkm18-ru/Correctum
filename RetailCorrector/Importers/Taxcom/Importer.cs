using RetailCorrector.Data;
using RetailCorrector.Utils.Integrations.Clients;

namespace RetailCorrector.Importers.Taxcom;

internal partial class Taxcom : ImporterBase<Taxcom, Taxcom.View>
{
    public override string Name => "Такском-Касса";
    private readonly TaxcomClient _client = new();

    public override ImportSession GenerateInfo() =>
        parseInfo = new ImportSession(Name);

    public override IAsyncEnumerable<Receipt> Import(IProgress<long> progress, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}