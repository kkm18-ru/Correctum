using Correctum.Exceptions;
using Correctum.Models;

namespace Correctum.Validators;

public static class ReceiptValidator
{
    public static void Validate(Receipt receipt, int ffd)
    {
        switch (ffd)
        {
            case 120: Validate120(receipt); break;
            default: throw new InvalidOperationException("Программа поддерживает только ККТ с ФФД 1.2");
        }
    }

    private static void Validate120(Receipt receipt)
    {
        if (receipt.Type is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Receipt, receipt.Id, "Отсутствует тип чека");
        if (receipt.CashSum is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Receipt, receipt.Id, "Отсутствует сумма наличной оплаты");
        if (receipt.NonCashSum is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Receipt, receipt.Id, "Отсутствует сумма безналичной оплаты");
        if (receipt.AdvanceSum is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Receipt, receipt.Id, "Отсутствует сумма предоплаты");
        if (receipt.CreditSum is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Receipt, receipt.Id, "Отсутствует сумма оплаты кредита");
        if (receipt.ProvisionSum is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Receipt, receipt.Id, "Отсутствует встречное предоставление");
        if (receipt.TotalSum is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Receipt, receipt.Id, "Отсутствует итоговая сумма");
        if (receipt.OperatorName is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Receipt, receipt.Id, "Отсутствует имя кассира");
        if (receipt.Created is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Receipt, receipt.Id, "Отсутствует дата");

        if (Enum.GetName(receipt.Type.Value) is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Receipt, receipt.Id, "Некорректный тип чека");
        if (receipt.TaxationSystem.HasValue && Enum.GetName(receipt.TaxationSystem.Value) is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Receipt, receipt.Id, "Некорректная система налогооблажения");

        foreach (var position in receipt.Items) 
            PositionValidator.Validate(position);
    }
}
