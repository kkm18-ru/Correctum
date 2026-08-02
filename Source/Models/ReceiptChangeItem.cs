using Correctum.Core.Models;
using Correctum.Enums;

namespace Correctum.Models;

public sealed class ReceiptChangeItem
{
    public int Id { get; set; }
    public ReceiptChangeType Type { get; set; }
    public string? Value { get; set; }
    public Changes Change { get; set; }
    public int ChangeId { get; set; }
    public Receipt Receipt { get; set; }
    public int ReceiptId { get; set; }
}
