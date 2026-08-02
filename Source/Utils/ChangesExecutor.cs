using Correctum.Core.Enums;
using Correctum.Core.Models;
using Correctum.Core.ValueObjects;
using Correctum.Enums;
using Correctum.Enums.ReceiptMetadata;
using Correctum.Models;

namespace Correctum.Utils;

internal static class ChangesExecutor
{
    public static Position? ApplyChanges(this Position position)
    {
        if (position.Changes.Any(ch => ch.Type == PositionChangeType.Remove)) return null;
        var clone = position.Clone();

        position.Changes.ApplyLastChangeItem(PositionChangeType.SettingName, value => clone.Name = value);
        position.Changes.ApplyLastChangeItem(PositionChangeType.SettingSubjectType, value => clone.Type = ParseEnum<PositionType>(value));
        position.Changes.ApplyLastChangeItem(PositionChangeType.SettingCalculationMethod, value => clone.CalcMethod = ParseEnum<CalculationMethod>(value));
        position.Changes.ApplyLastChangeItem(PositionChangeType.SettingTaxRate, value => clone.TaxRate = ParseEnum<NDSRate>(value));
        position.Changes.ApplyLastChangeItem(PositionChangeType.SettingMeasureUnit, value => clone.Unit = ParseEnum<UnitOfMeasure>(value));
        position.Changes.ApplyLastChangeItem(PositionChangeType.SettingPrice, value => clone.Price = ParseCurrency(value));
        position.Changes.ApplyLastChangeItem(PositionChangeType.SettingQuantity, value => clone.Quantity = ParseDecimal(value));
        position.Changes.ApplyLastChangeItem(PositionChangeType.SettingCost, value => clone.Cost = ParseCurrency(value));

        return clone;
    }

    public static Receipt ApplyChanges(this Receipt receipt)
    {
        var mode = (ActionMode?)receipt.Metadata.LastOrDefault(md => md.Property == ReceiptMetadataProperty.ActionMode)?.Value ?? 0;
        if (mode != ActionMode.Correction) return receipt;
        
        var clone = receipt.Clone(false);

        receipt.Items
            .Select(i => i.ApplyChanges())
            .Where(i => i is { })
            .Cast<Position>()
            .ForEach(clone.Items.Add);

        receipt.Changes.ApplyLastChangeItem(ReceiptChangeType.SettingCashboxNumb, value => clone.RegId = value);
        receipt.Changes.ApplyLastChangeItem(ReceiptChangeType.SettingStorageNumb, value => clone.StorageId = value);
        receipt.Changes.ApplyLastChangeItem(ReceiptChangeType.SettingDocNumb, value => clone.DocNumber = ParseUInt32(value));
        receipt.Changes.ApplyLastChangeItem(ReceiptChangeType.SettingShiftNumb, value => clone.SessionId = ParseUInt32(value));
        receipt.Changes.ApplyLastChangeItem(ReceiptChangeType.SettingShiftDocNumb, value => clone.DocInSession = ParseUInt32(value));
        receipt.Changes.ApplyLastChangeItem(ReceiptChangeType.SettingCreatedAt, value => clone.Created = ParseDateTime(value));
        receipt.Changes.ApplyLastChangeItem(ReceiptChangeType.SettingDocSign, value => clone.DocSign = ParseFiscalSign(value));
        receipt.Changes.ApplyLastChangeItem(ReceiptChangeType.SettingReceiptType, value => clone.Type = ParseEnum<ReceiptType>(value));
        receipt.Changes.ApplyLastChangeItem(ReceiptChangeType.SettingCashSum, value => clone.CashSum = ParseCurrency(value));
        receipt.Changes.ApplyLastChangeItem(ReceiptChangeType.SettingNonCashSum, value => clone.NonCashSum = ParseCurrency(value));
        receipt.Changes.ApplyLastChangeItem(ReceiptChangeType.SettingAdvanceSum, value => clone.AdvanceSum = ParseCurrency(value));
        receipt.Changes.ApplyLastChangeItem(ReceiptChangeType.SettingCreditSum, value => clone.CreditSum = ParseCurrency(value));
        receipt.Changes.ApplyLastChangeItem(ReceiptChangeType.SettingProvisionSum, value => clone.ProvisionSum = ParseCurrency(value));
        receipt.Changes.ApplyLastChangeItem(ReceiptChangeType.SettingTotalSum, value => clone.TotalSum = ParseCurrency(value));
        receipt.Changes.ApplyLastChangeItem(ReceiptChangeType.SettingBuyerContact, value => clone.BuyerContact = value);
        receipt.Changes.ApplyLastChangeItem(ReceiptChangeType.SettingTaxationSystem, value => clone.TaxationSystem = ParseEnum<TaxationSystems>(value));
        receipt.Changes.ApplyLastChangeItem(ReceiptChangeType.SettingRetailPlace, value => clone.RetailPlace = value);
        receipt.Changes.ApplyLastChangeItem(ReceiptChangeType.SettingRetailAddress, value => clone.RetailAddress = value);
        receipt.Changes.ApplyLastChangeItem(ReceiptChangeType.SettingSenderAddress, value => clone.SenderAddress = value);
        receipt.Changes.ApplyLastChangeItem(ReceiptChangeType.SettingOperatorName, value => clone.OperatorName = value);
        receipt.Changes.ApplyLastChangeItem(ReceiptChangeType.SettingOperatorInn, value => clone.OperatorInn = ParseInn(value));

        return clone;
    }

    private static void ApplyLastChangeItem
        (this IList<PositionChangeItem> changes, PositionChangeType type, Action<string?> setter)
    {
        if (changes.LastOrDefault(ch => ch.Type.Equals(type)) is { } ch)
            setter.Invoke(ch.Value);
    }

    private static void ApplyLastChangeItem
        (this IList<ReceiptChangeItem> changes, ReceiptChangeType type, Action<string?> setter)
    {
        if (changes.LastOrDefault(ch => ch.Type.Equals(type)) is { } ch)
            setter.Invoke(ch.Value);
    }

    private static uint? ParseUInt32(string? str) =>
        str is null ? null : uint.Parse(str);

    private static decimal? ParseDecimal(string? str) =>
        str is null ? null : decimal.Parse(str);

    private static Inn? ParseInn(string? str) =>
        str is null ? null : Inn.Parse(str);

    private static DateTime? ParseDateTime(string? str) =>
        str is null ? null : DateTime.Parse(str);

    private static FiscalSign? ParseFiscalSign(string? str) =>
        str is null ? null : FiscalSign.Parse(str);

    private static Currency? ParseCurrency(string? str) => (Currency?)ParseDecimal(str);

    private static TEnum? ParseEnum<TEnum>(string? str) where TEnum : struct, Enum =>
        str is null ? null : Enum.Parse<TEnum>(str);
}
