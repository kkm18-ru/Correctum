using RetailCorrector.Data;

namespace RetailCorrector.Parsers;

internal interface IParser
{
    string Name { get; }
    void Parse(string raw, ref Receipt receipt);
}