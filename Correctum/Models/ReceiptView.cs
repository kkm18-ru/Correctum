using Correctum.Core.Models;
using Correctum.Utils;

namespace Correctum.Models;

public readonly struct ReceiptView(Receipt model)
{
    public string? RegId { get; } = model.RegId;
    public string? StorageId { get; } = model.StorageId;
    public string? DocNumber { get; } = model.DocNumber?.ToString();
    public string? SessionId { get; } = model.SessionId?.ToString();
    public string? DocInSession { get; } = model.DocInSession?.ToString();
    public string? Created { get; } = model.Created?.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");
    public string? DocSign { get; } = model.DocSign?.ToString();
    public string? Type { get; } = EnumLocales.ReceiptTypes.GetLocale(model.Type, null);
    public string? TaxationSystem { get; } = EnumLocales.TaxationSystems.GetLocale(model.TaxationSystem, null);
    public string? BuyerContact { get; } = model.BuyerContact;
    public string? RetailPlace { get; } = model.RetailPlace;
    public string? RetailAddress { get; } = model.RetailAddress;
    public string? SenderAddress { get; } = model.SenderAddress;
    public string? OperatorName { get; } = model.OperatorName;
    public string? OperatorInn { get; } = model.OperatorInn?.ToString();
    public PositionView[] Items { get; } = [.. model.Items.Select(m => new PositionView(m))];
    public string? CashSum { get; } = model.CashSum is null ? null : $"{model.CashSum} руб.";
    public string? NonCashSum { get; } = model.NonCashSum is null ? null : $"{model.NonCashSum} руб.";
    public string? AdvanceSum { get; } = model.AdvanceSum is null ? null : $"{model.AdvanceSum} руб.";
    public string? CreditSum { get; } = model.CreditSum is null ? null : $"{model.CreditSum} руб.";
    public string? ProvisionSum { get; } = model.ProvisionSum is null ? null : $"{model.ProvisionSum} руб.";
    public string? TotalSum { get; } = model.TotalSum is null ? null : $"{model.TotalSum} руб.";
}
