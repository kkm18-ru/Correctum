using Correctum.Models;

namespace Correctum.Abstractions;

public interface IDeserializer
{
    abstract static Receipt Deserialize(ReadOnlySpan<char> text);
}
