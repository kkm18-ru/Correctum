using Correctum.Core.Enums;

namespace RetailCorrector.Utils;

public static class EnumUtils
{
    public static NDSRate ToTaxRate(byte rawTaxRate) => rawTaxRate switch
    {
        1 => NDSRate.NDS20,
        2 => NDSRate.NDS10,
        3 => NDSRate.NDS120,
        4 => NDSRate.NDS110,
        5 => NDSRate.NDS0,
        6 => NDSRate.NoNDS,
        7 => NDSRate.NDS5,
        8 => NDSRate.NDS7,
        9 => NDSRate.NDS105,
        10 => NDSRate.NDS107,
        11 => NDSRate.NDS22,
        12 => NDSRate.NDS122,
        _ => 0
    };
}