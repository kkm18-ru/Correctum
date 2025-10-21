using RetailCorrector.Data;

namespace RetailCorrector.Deserializers;

internal interface IReceiptDeserializer
{
    string Name { get; }
    void Deserialize(string raw, ref Receipt receipt);
}