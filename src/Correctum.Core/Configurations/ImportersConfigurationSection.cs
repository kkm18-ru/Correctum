using System.ComponentModel.DataAnnotations;

namespace Correctum.Configurations;

public sealed class ImportersConfigurationSection
{
    [Range(1, 1000)] public ushort SabyReceiptsSize { get; set; }
    [Range(1, 100)] public int TaxcomResponseSize { get; set; }
    [Range(1, 100)] public int FirstOfdTransactionSize { get; set; }
    public string TaxcomInteratorId { get; set; }

    public static ImportersConfigurationSection Default => new()
    {
        SabyReceiptsSize = 100,
        TaxcomResponseSize = 100,
        FirstOfdTransactionSize = 100,
        TaxcomInteratorId = string.Empty
    };
}
