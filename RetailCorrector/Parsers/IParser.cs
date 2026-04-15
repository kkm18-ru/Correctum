using Correctum.Core.Models;

namespace RetailCorrector.Parsers;

internal interface IParser
{
    string Name { get; }
    bool Parse(string raw, ref Receipt receipt);
}