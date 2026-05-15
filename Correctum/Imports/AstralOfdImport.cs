using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Core.Models;
using Correctum.Utils;
using Correctum.Utils.Integrations.Clients;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Correctum.Imports;

[ImportInfo("Астрал ОФД")]
[TextBox("Идентификатор сессии", nameof(SessionId))]
[DateRange("Период импортирования", nameof(DateRange))]
[CheckComboBox("Организации", nameof(Organizations), nameof(SelectedOrganizations))]
[AuthorizationProperties(nameof(SessionId))]
public sealed class AstralOfdImport : OfdImport<AstralOfdClient>
{
    public string SessionId { get; set => SetField(ref field, value); } = string.Empty;
    public DateRange DateRange { get; } = new();
    public ObservableCollection<AstralOfdOrganization> Organizations { get; } = [];
    public ObservableCollection<AstralOfdOrganization> SelectedOrganizations { get; } = [];

    public override async IAsyncEnumerable<Receipt> ImportAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach(var org in SelectedOrganizations)
        {
            foreach (var dt in DateRange)
            {
                var tickets = client
                    .EnumTicketInfoAsync(org, dt.ToDateTime(TimeOnly.MinValue), cancellationToken)
                    .Where(ticket => ticket.DocumentType.StartsWith('3'))
                    .WithCancellation(cancellationToken);

                await foreach (var ticket in tickets)
                    yield return await client.GetReceiptAsync(ticket, cancellationToken);
            }
        }
    }

    protected override async Task InternalAuthAsync()
    {
        client.SetSessionId(SessionId);
        SelectedOrganizations.Clear();
        Organizations.Clear();
        await foreach (var org in client.EnumOrganizationAsync())
            Organizations.Add(org);
    }
}
