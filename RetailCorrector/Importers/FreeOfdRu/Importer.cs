using RetailCorrector.Data;
using RetailCorrector.Utils.Integrations.Clients;

namespace RetailCorrector.Importers
{
    internal partial class FreeOfdRu : ImporterBase<FreeOfdRu, FreeOfdRu.View>
    {
        public override string Name => "ОФД.ру (Логин-Пароль)";
        public FreeOfdRuClient Client { get; } = new();

        public override ImportSession GenerateInfo() =>
            parseInfo = new ImportSession(Name);

        public override async IAsyncEnumerable<Receipt> Import(IProgress<long> progress, CancellationToken cancellationToken)
        {
            progress.Report(-((int)(End - Begin).TotalDays + 1) * SelectedDevices.Count);
            var i = 0;
            foreach (var device in SelectedDevices)
            {
                foreach(var storage in await Client.FetchStoragesByDeviceAsync(device, cancellationToken))
                {

                    for (var day = Begin; day <= End; day = day.AddDays(1))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await foreach (var docNumber in Client.FetchDocsByStorageAsync(device.RegNumber, storage, day, day.AddSeconds(86399),cancellationToken))
                            yield return await Client.FetchDetailDocumentAsync(parseInfo.Id, device.AgreementId, docNumber, storage, cancellationToken);
                        progress.Report(++i);
                    }
                }
            }
        }
    }
}
