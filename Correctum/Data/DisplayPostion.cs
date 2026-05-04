using System.Diagnostics;
using Correctum.Utils;
using Correctum.Core.Models;
using System.Text;

namespace Correctum.Data
{
    [DebuggerDisplay("{Name} - {Price} * {Quantity} == {Cost}")]
    internal readonly struct DisplayPostion
    {
        public string Receipt { get; }
        public string Name { get; }
        public string Type { get; }
        public string PayMethod { get; }
        public string TaxRate { get; }
        public string Price { get; }
        public string Quantity { get; }
        public string Cost { get; }

        public DisplayPostion(Position origin, DisplayReceipt receipt)
        {
            Receipt = receipt.ToString();
            Name = origin.Name ?? string.Empty;
            Type = EnumLocales.PositionTypes.GetLocale(origin.Type, string.Empty);
            PayMethod = EnumLocales.PositionPayments.GetLocale(origin.CalcMethod, string.Empty);
            TaxRate = EnumLocales.TaxRates.GetLocale(origin.TaxRate, string.Empty);
            Price = origin.Price?.ToString() ?? string.Empty;
            Cost = origin.Cost?.ToString() ?? string.Empty;
            var QuantityBuilder = new StringBuilder();
            QuantityBuilder.Append(origin.Quantity is null ? "-" : (origin.Quantity * 0.001).ToString());
            QuantityBuilder.Append(EnumLocales.PositionUnits.GetLocale(origin.Unit, string.Empty));
            Quantity = QuantityBuilder.ToString();
        }
    }
}
