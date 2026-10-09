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
        if (receipt.OperatorName is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Receipt, receipt.Id, "Отсутствует имя кассира");

        if (Enum.GetName(receipt.Type.Value) is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Receipt, receipt.Id, "Некорректный тип чека");
        if (receipt.TaxationSystem.HasValue && Enum.GetName(receipt.TaxationSystem.Value) is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Receipt, receipt.Id, "Некорректная система налогооблажения");

        foreach (var position in receipt.Items) 
            PositionValidator.Validate(position);
    }
}
