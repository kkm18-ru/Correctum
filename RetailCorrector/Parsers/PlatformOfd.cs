using Correctum.Core.Models;
using Correctum.Core.Enums;
using RetailCorrector.Utils;
using Correctum.Core.ValueObjects;
using System.Globalization;

namespace RetailCorrector.Parsers;

internal sealed class PlatformOfd : IParser
{
    public string Name => "Платформа ОФД (Реквизиты)";
    public static PlatformOfd Instance => field ??= new PlatformOfd();
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly Stack<ushort> _propStack = new(5);
    
    private delegate void UpdatePaymentDelegate(ref Payment payment, ulong sum);
    private delegate void UpdatePositionDelegate(ref Position position, ReadOnlySpan<char> value);
    
    public bool Parse(string raw, ref Receipt receipt)
    {
        var span = raw.AsSpan();
        Span<char> rawValue = stackalloc char[256];
        foreach (var range in span.Split('\n'))
        {
            var line = span[range];
            if(line.IsEmpty) continue;
            if(line[1] is not ('+' or '|')) continue;
            
            var level = line.Count('|');
            while (level < _propStack.Count)
                _propStack.Pop();
            
            var tagIndex = line.IndexOf('[');
            if (!ushort.TryParse(line.Slice(tagIndex + 1, 4), out var tag)) continue;

            if (line.Length < 43)
            {
                _propStack.Push(tag);
                if (tag == 1059) AddPosition(ref receipt);
                continue;
            }
            rawValue.Clear();
            line[43..].TrimEnd('·').Replace(rawValue,'·', ' ');
            var value = rawValue.Trim(['\r', '\n', '\0', '\t', ' ']);

            try
            {
                ChangeReceipt(tag, value, ref receipt);
            }
            catch(Exception ex)
            {
                Logger.Error(ex, "Не удалось обработать тег {Tag} со значением {Value}", tag, value.ToString());
                return false;
            }
        }

        return true;
    }

    private void ChangeReceipt(ushort tag, ReadOnlySpan<char> value, ref Receipt receipt)
    {
        switch (tag)
        {
            // Простые реквизиты чека
            case 1077: receipt.DocSign = FiscalSign.Parse(value); break;
            case 1012: receipt.Created = DateTimeOffset.UnixEpoch.AddSeconds(ulong.Parse(value)).DateTime; break;
            case 1041: receipt.StorageId = value.ToString(); break;
            case 1037: receipt.RegId = value.ToString(); break;
            case 1038: receipt.SessionId = uint.Parse(value); break;
            case 1020: receipt.TotalSum = ulong.Parse(value); break;
            case 1055: receipt.TaxationSystem = (TaxationSystems)int.Parse(value); break;
            case 1008: receipt.BuyerContact = value.ToString(); break;
            case 1042: receipt.DocInSession = uint.Parse(value); break;
            case 1040: receipt.DocNumber = uint.Parse(value); break;
            case 1054: receipt.Type = (ReceiptType)byte.Parse(value); break;
            // Суммы способов оплаты
            case 1031: UpdatePayment((ref p, sum) => p.Cash = sum, value, ref receipt); break;
            case 1081: UpdatePayment((ref p, sum) => p.ECash = sum, value, ref receipt); break;
            case 1215: UpdatePayment((ref p, sum) => p.Pre = sum, value, ref receipt); break;
            case 1216: UpdatePayment((ref p, sum) => p.Post = sum, value, ref receipt); break;
            case 1217: UpdatePayment((ref p, sum) => p.Provision = sum, value, ref receipt); break;
            // Позиции
            case 2108: UpdatePosition((ref p, v) => p.Unit = (UnitOfMeasure)byte.Parse(v), value, ref receipt); break;
            case 1079: UpdatePosition((ref p, v) => p.Price = ulong.Parse(v), value, ref receipt); break;
            case 1043: UpdatePosition((ref p, v) => p.Cost = ulong.Parse(v), value, ref receipt); break;
            case 1199: UpdatePosition((ref p, v) => p.TaxRate = EnumUtils.ToTaxRate(byte.Parse(v)), value, ref receipt); break;
            case 1214: UpdatePosition((ref p, v) => p.PayMethod = (CalculationMethod)byte.Parse(v), value, ref receipt); break;
            case 1212: UpdatePosition((ref p, v) => p.Type = (PositionType)byte.Parse(v), value, ref receipt); break;
            case 1023: UpdatePosition((ref p, v) => p.Quantity = (ulong)Math.Round(double.Parse(v, CultureInfo.InvariantCulture) * 1000, 0), value, ref receipt); break;
            case 1030: UpdatePosition((ref p, v) => p.Name = v.ToString(), value, ref receipt); break;
            
            default: break;
        }
    }

    private static void AddPosition(ref Receipt receipt)
    {
        var positions = receipt.Items ?? [];
        Array.Resize(ref positions, positions.Length+1);
        receipt.Items = positions;
    }
    
    private static void UpdatePayment(UpdatePaymentDelegate setter, ReadOnlySpan<char> sum, ref Receipt receipt)
    {
        var payment = receipt.Payment;
        setter(ref payment, ulong.Parse(sum));
        receipt.Payment = payment;
    }
    
    private void UpdatePosition(UpdatePositionDelegate setter, ReadOnlySpan<char> value, ref Receipt receipt)
    {
        if (_propStack.Count != 1 || _propStack.Peek() != 1059) return;
        
        var position = receipt.Items[^1];
        setter(ref position, value);
        receipt.Items[^1] = position;
    }
}