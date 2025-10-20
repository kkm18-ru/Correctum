using RetailCorrector.Data;

namespace RetailCorrector.Deserializers;

internal interface IReceiptDeserializer
{
    void Deserialize(string raw, ref Receipt receipt);
}