using Correctum.Core.Models;

namespace Correctum.Deserializers;

public interface IDeserializer
{
    abstract static Receipt Deserialize(ReadOnlySpan<char> text);
}
