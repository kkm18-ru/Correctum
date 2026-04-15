using System.Diagnostics;
using RetailCorrector.Utils;
using Correctum.Core.Models;

namespace RetailCorrector.Data
{
    [DebuggerDisplay("{Price}  {Name}")]
    internal readonly struct DisplayPostion(Position origin, DisplayReceipt receipt)
    {
        public string Receipt { get; } = receipt.ToString();
        public string Name { get; } = origin.Name;

        public string Type { get; } = EnumLocales.PositionTypes.GetLocale(origin.Type);
        public string PayMethod { get; } = EnumLocales.PositionPayments.GetLocale(origin.CalcMethod);
        public string TaxRate { get; } = EnumLocales.TaxRates.GetLocale(origin.TaxRate);
        public string Price { get; } = $"{(decimal)origin.Price:F2}";

        public string Quantity { get; } =
            $"{(origin.Quantity * 0.001):F3}{EnumLocales.PositionUnits.GetLocale(origin.Unit)}";
        public string Cost { get; } = $"{(decimal)origin.Cost:F2}";
    }
}
