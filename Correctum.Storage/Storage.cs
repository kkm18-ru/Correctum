using Correctum.Storage.Models;

namespace Correctum.Storage;

public sealed class Storage
{
    internal List<ReceiptDto> DtoReceipts { get; private set; }
    internal List<PositionDto> DtoPositions { get; private set; }
}
