using RetailCorrector.Data;

namespace RetailCorrector.Parsers;

internal interface IParser
{
    string Name { get; }
    bool Parse(string raw, ref Receipt receipt);
}