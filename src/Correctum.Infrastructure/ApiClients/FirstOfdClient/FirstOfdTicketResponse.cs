using System.Text.Json.Serialization;

namespace Correctum.ApiClients.FirstOfdClient;

[method: JsonConstructor]
internal readonly struct FirstOfdTicketResponse(FirstOfdTicket ticket)
{
    public FirstOfdTicket Ticket { get; } = ticket;
}
