using Correctum.Core.Enums;

namespace RetailCorrector.Utils;

public static class EnumUtils
{
    public static TaxRate ToTaxRate(byte rawTaxRate) => rawTaxRate switch
    {
        1 => TaxRate.Tax20,
        2 => TaxRate.Tax10,
        3 => TaxRate.Calc20,
        4 => TaxRate.Calc10,
        5 => TaxRate.Tax0,
        6 => TaxRate.Without,
        7 => TaxRate.Tax5,
        8 => TaxRate.Tax7,
        9 => TaxRate.Calc5,
        10 => TaxRate.Calc7,
        11 => TaxRate.Tax22,
        12 => TaxRate.Calc22,
        _ => TaxRate.Unknown
    };
}