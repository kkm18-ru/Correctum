using Correctum.Enums;
using Correctum.Exceptions;
using Correctum.Models;
using Correctum.Utils;

namespace Correctum.ReceiptDeserializator;

public sealed class PlatformOfdReceiptDeserializator : IReceiptDeserializator
{
    /// <inheritdoc/>
    /// <exception cref="ReceiptDeserializationException">Ошибка при конвертации значения тега</exception>
    public Receipt Deserialize(ReadOnlySpan<char> text)
    {
        Stack<ushort> stack = new(5);
        var receipt = new Receipt();
        Span<char> rawValue = stackalloc char[128];
        foreach (var range in text.Split('\n'))
        {
            var line = text[range];
            if (line.IsEmpty) continue;
            if (line[1] is not ('+' or '|')) continue;

            var level = line.Count('|');
            while (level < stack.Count)
                stack.Pop();

            var tagIndex = line.IndexOf('[');
            if (!ushort.TryParse(line.Slice(tagIndex + 1, 4), out var tag)) continue;

            if (line.Length < 43)
            {
                stack.Push(tag);
                if (tag == 1059) AddPosition(ref receipt);
                continue;
            }
            rawValue.Clear();
            line[43..].TrimEnd('·').Replace(rawValue, '·', ' ');
            var valueLength = rawValue.IndexOf('\r');
            var value = rawValue[..valueLength];

            try
            {
                ChangeReceipt(tag, value, ref receipt, stack);
            }
            catch (Exception ex)
            {
                throw new ReceiptDeserializationException($"Не удалось обработать тег {tag} со значением {value}", ex);
            }
        }

        return receipt;
    }

    private delegate void UpdatePositionDelegate(ref Position position, ReadOnlySpan<char> value);

    private static void ChangeReceipt(ushort tag, ReadOnlySpan<char> value, ref Receipt receipt, Stack<ushort> stack)
    {
        switch (tag)
        {
            // Простые реквизиты чека
            case 1077: receipt.FiscalSign = uint.Parse(value); break;
            case 1012: receipt.DateTime = DateTimeOffset.UnixEpoch.AddSeconds(ulong.Parse(value)).DateTime; break;
            case 1041: receipt.DriveSerial = value.ToString(); break;
            case 1037: receipt.RegNumber = value.ToString(); break;
            case 1038: receipt.SessionNumber = uint.Parse(value); break;
            case 1020: receipt.TotalSum = ulong.Parse(value); break;
            case 1055: receipt.Taxation = (TaxationSystem)int.Parse(value); break;
            case 1008: receipt.Buyer = value.ToString(); break;
            case 1042: receipt.NumberInSession = uint.Parse(value); break;
            case 1040: receipt.Number = uint.Parse(value); break;
            case 1054: receipt.Type = (ReceiptType)byte.Parse(value); break;
            // Суммы способов оплаты
            case 1031: receipt.CashSum = ulong.Parse(value); break;
            case 1081: receipt.NonCashSum = ulong.Parse(value); break;
            case 1215: receipt.PrepaidSum = ulong.Parse(value); break;
            case 1216: receipt.CreditSum = ulong.Parse(value); break;
            case 1217: receipt.OtherSum = ulong.Parse(value); break;
            // Позиции
            case 2108: UpdatePosition((ref p, v) => p.Unit = (PositionUnit)byte.Parse(v), value, ref receipt, stack); break;
            case 1079: UpdatePosition((ref p, v) => p.Price = ulong.Parse(v), value, ref receipt, stack); break;
            case 1043: UpdatePosition((ref p, v) => p.Cost = ulong.Parse(v), value, ref receipt, stack); break;
            case 1199: UpdatePosition((ref p, v) => p.TaxRate = EnumUtils.ToTaxRate(byte.Parse(v)), value, ref receipt, stack); break;
            case 1214: UpdatePosition((ref p, v) => p.CalculationMethod = (PositionCalculationMethod)byte.Parse(v), value, ref receipt, stack); break;
            case 1212: UpdatePosition((ref p, v) => p.SubjectType = (PositionType)byte.Parse(v), value, ref receipt, stack); break;
            case 1023: UpdatePosition((ref p, v) => p.Quantity = (ulong)Math.Round(double.Parse(v) * 1000, 0), value, ref receipt, stack); break;
            case 1030: UpdatePosition((ref p, v) => p.Name = v.ToString(), value, ref receipt, stack); break;

            default: break;
        }
    }

    private static void AddPosition(ref Receipt receipt)
    {
        var positions = receipt.Items ?? [];
        Array.Resize(ref positions, positions.Length + 1);
        receipt.Items = positions;
    }

    private static void UpdatePosition(UpdatePositionDelegate setter, ReadOnlySpan<char> value, ref Receipt receipt, Stack<ushort> stack)
    {
        if (stack.Count != 1 || stack.Peek() != 1059) return;

        var position = receipt.Items[^1];
        setter(ref position, value);
        receipt.Items[^1] = position;
    }
}
