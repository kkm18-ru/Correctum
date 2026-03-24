namespace Correctum.ApiClients.OfdRuTokenClient;

public readonly struct OfdRuTokenKkt : IOfdRuTokenModel
{
    public string KktRegId { get; init; }
    public string INN { get; init; }

    public override string ToString() => KktRegId;
}
