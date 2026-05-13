using Correctum.Core.Models;
using Correctum.Utils;

namespace Correctum.Models;

public readonly struct PositionView(Position model)
{
    public string? Name { get; } = model.Name;
    public string? SubjectType { get; } = EnumLocales.PositionTypes.GetLocale(model.Type, null);
    public string? CalcMethod { get; } = EnumLocales.PositionPayments.GetLocale(model.CalcMethod, null);
    public string? NDS { get; } = EnumLocales.TaxRates.GetLocale(model.TaxRate, null);
    public string? UnitOfMeasure { get; } = EnumLocales.PositionUnits.GetLocale(model.Unit, null);
    public string? Price { get; } = model.Price is null ? null : $"{model.Price} руб.";
    public string? Quantity { get; } = model.Quantity?.ToString();
    public string? Cost { get; } = model.Cost is null ? null : $"{model.Cost} руб.";
}
