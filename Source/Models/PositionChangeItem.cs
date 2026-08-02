using Correctum.Core.Models;
using Correctum.Enums;

namespace Correctum.Models;

public sealed class PositionChangeItem
{
    public int Id { get; set; }
    public PositionChangeType Type { get; set; }
    public string? Value { get; set; }
    public Changes Change { get; set; }
    public int ChangeId { get; set; }
    public Position Position { get; set; }
    public int PositionId { get; set; }
}
