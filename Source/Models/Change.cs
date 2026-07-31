namespace Correctum.Models;

public sealed class Change
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IList<ReceiptChangeItem> ReceiptItems { get; } = [];
    public IList<ReceiptMetadataChangeItem> ReceiptMetadataItems { get; } = [];
    public IList<PositionChangeItem> PositionItems { get; } = [];
}
