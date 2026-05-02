using Correctum.Core.Models;

namespace Correctum.Parsers;

internal interface IParser
{
    string Name { get; }
    bool Parse(string raw, out Receipt receipt);
}