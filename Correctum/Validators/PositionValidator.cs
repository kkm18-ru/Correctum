using Correctum.Core.Models;
using Correctum.Exceptions;

namespace Correctum.Validators;

public static class PositionValidator
{
    public static void Validate(Position position)
    {
        if (string.IsNullOrWhiteSpace(position.Name))
            throw new ModelValidationException(ModelValidationException.ModelType.Position, position.Id, "Отсутствует наименование позиции");
        if (position.Type is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Position, position.Id, "Отсутствует тип позиции");
        if (position.CalcMethod is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Position, position.Id, "Отсутствует способ расчета");
        if (position.Unit is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Position, position.Id, "Отсутствует мера количества позиции");
        if (position.TaxRate is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Position, position.Id, "Отсутствует ставка НДС");
        if (position.Price is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Position, position.Id, "Отсутствует цена позиции");
        if (position.Quantity is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Position, position.Id, "Отсутствует количество позиции");
        if (position.Cost is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Position, position.Id, "Отсутствует стоимость позиции");

        if (Enum.GetName(position.Type.Value) is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Position, position.Id, "Некорректный тип позиции");
        if (Enum.GetName(position.CalcMethod.Value) is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Position, position.Id, "Некорректный способ расчета");
        if (Enum.GetName(position.Unit.Value) is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Position, position.Id, "Некорректная мера количества позиции");
        if (Enum.GetName(position.TaxRate.Value) is null)
            throw new ModelValidationException(ModelValidationException.ModelType.Position, position.Id, "Некорректная ставка НДС");
    }
}
