using Correctum.Core.Models;
using Correctum.Enums;

namespace Correctum.Models;

public sealed class ReceiptMetadataChangeItem
{
    public int Id { get; set; }
    public ReceiptMetadataProperty Property { get; set; }
    public int Value { get; set; }
    public Changes Change { get; set; }
    public int ChangeId { get; set; }
    public Receipt Receipt { get; set; }
    public int ReceiptId { get; set; }
}
