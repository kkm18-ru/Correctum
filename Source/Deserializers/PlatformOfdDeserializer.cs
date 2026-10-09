using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Enums;
using Correctum.Models;
using Correctum.ValueObjects;
using System.Globalization;

namespace Correctum.Deserializers;

[DeserializerInfo("Платформа ОФД (Реквизиты)")]
public class PlatformOfdDeserializer : IDeserializer
{
    private static readonly Stack<ushort> _propStack = new(5);
    public static Receipt Deserialize(ReadOnlySpan<char> text)
    {
        _propStack.Clear();
        var receipt = new Receipt();
        Position currPosition = null!;
        Span<char> rawValue = stackalloc char[256];
        foreach (var range in text.Split('\n'))
        {
            var line = text[range];
            if (line.IsEmpty) continue;
            if (line[1] is not ('+' or '|')) continue;

            var level = line.Count('|');
            while (level < _propStack.Count)
                _propStack.Pop();

            var tagIndex = line.IndexOf('[');
            if (!ushort.TryParse(line.Slice(tagIndex + 1, 4), out var tag)) continue;

            if (line.Length < 43)
            {
                _propStack.Push(tag);
                if (tag == 1059)
                {
                    currPosition = new();
                    receipt.Items.Add(currPosition);
                }
                continue;
            }
            rawValue.Clear();
            line[43..].TrimEnd('·').Replace(rawValue, '·', ' ');
            var value = rawValue.Trim(['\r', '\n', '\0', '\t', ' ']);

            ChangeReceipt(tag, value, receipt);
            ChangePosition(tag, value, currPosition);
        }
        return receipt;
    }
    private static void ChangeReceipt(ushort tag, ReadOnlySpan<char> value, Receipt builder)
    {
        if (_propStack.Count > 0) return;
        switch (tag)
        {
            // Простые реквизиты чека
            case 1077: builder.DocSign = FiscalSign.Parse(value); break;
            case 1012: builder.Created = DateTimeOffset.UnixEpoch.AddSeconds(ulong.Parse(value)).DateTime; break;
            case 1041: builder.StorageId = value.ToString(); break;
            case 1037: builder.RegId = value.ToString(); break;
            case 1038: builder.SessionId = uint.Parse(value); break;
            case 1020: builder.TotalSum = ulong.Parse(value); break;
            case 1055: builder.TaxationSystem = (TaxationSystems)int.Parse(value); break;
            case 1008: builder.BuyerContact = value.ToString(); break;
            case 1042: builder.DocInSession = uint.Parse(value); break;
            case 1040: builder.DocNumber = uint.Parse(value); break;
            case 1054: builder.Type = (ReceiptType)byte.Parse(value); break;
            case 1117: builder.SenderAddress = value.ToString(); break;
            case 1203: builder.OperatorInn = Inn.Parse(value.ToString()); break;
            case 1009: builder.RetailAddress = value.ToString(); break;
            case 1021: builder.OperatorName = value.ToString(); break;
            case 1187: builder.RetailPlace = value.ToString(); break;
            // Суммы способов оплаты
            case 1031: builder.CashSum = ulong.Parse(value); break;
            case 1081: builder.NonCashSum = ulong.Parse(value); break;
            case 1215: builder.AdvanceSum = ulong.Parse(value); break;
            case 1216: builder.CreditSum = ulong.Parse(value); break;
            case 1217: builder.ProvisionSum = ulong.Parse(value); break;

            default: break;
        }
    }

    private static void ChangePosition(ushort tag, ReadOnlySpan<char> value, Position position)
    {
        if (_propStack.Count != 1 || _propStack.Peek() != 1059) return;
        switch (tag)
        {
            case 2108: position.Unit = (UnitOfMeasure)byte.Parse(value); break;
            case 1079: position.Price = ulong.Parse(value); break;
            case 1043: position.Cost = ulong.Parse(value); break;
            case 1199: position.TaxRate = NDSRate.FromFns(byte.Parse(value)); break;
            case 1214: position.CalcMethod = (CalculationMethod)byte.Parse(value); break;
            case 1212: position.Type = (PositionType)byte.Parse(value); break;
            case 1023: position.Quantity = decimal.Parse(value, CultureInfo.InvariantCulture); break;
            case 1030: position.Name = value.ToString(); break;

            default: break;
        }
    }
}
