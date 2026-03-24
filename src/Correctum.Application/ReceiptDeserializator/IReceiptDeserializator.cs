using Correctum.Models;

namespace Correctum.ReceiptDeserializator;

public interface IReceiptDeserializator
{
    /// <summary>
    /// Test
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    Receipt Deserialize(ReadOnlySpan<char> text);
}
