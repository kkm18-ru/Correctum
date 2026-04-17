using Correctum.Core.Models;
using Correctum.Core.Enums;
using RetailCorrector.Utils;
using Correctum.Core.ValueObjects;
using System.Globalization;
using Correctum.Core.Builders;

namespace RetailCorrector.Parsers;

internal sealed class PlatformOfd : IParser
{
    public string Name => "Платформа ОФД (Реквизиты)";
    public static PlatformOfd Instance => field ??= new PlatformOfd();
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly Stack<ushort> _propStack = new(5);
    
    public bool Parse(string raw, out Receipt receipt)
    {
        var builder = new ReceiptBuilder();
        PositionBuilder currPosition = null!;
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
                if (tag == 1059)
                    currPosition = builder.AddItem();
                continue;
            }
            rawValue.Clear();
            line[43..].TrimEnd('·').Replace(rawValue,'·', ' ');
            var value = rawValue.Trim(['\r', '\n', '\0', '\t', ' ']);

            try
            {
                ChangeReceipt(tag, value, builder);
                ChangePosition(tag, value, currPosition);
            }
            catch(Exception ex)
            {
                receipt = default;
                Logger.Error(ex, "Не удалось обработать тег {Tag} со значением {Value}", tag, value.ToString());
                return false;
            }
        }
        receipt = builder.Build();
        return true;
    }

    private void ChangeReceipt(ushort tag, ReadOnlySpan<char> value, ReceiptBuilder builder)
    {
        if (_propStack.Count > 0 && _propStack.Peek() == 1059) return;
        switch (tag)
        {
            // Простые реквизиты чека
            case 1077: builder.SetDocSign(FiscalSign.Parse(value)); break;
            case 1012: builder.SetCreated(DateTimeOffset.UnixEpoch.AddSeconds(ulong.Parse(value)).DateTime); break;
            case 1041: builder.SetStorageId(value.ToString()); break;
            case 1037: builder.SetRegId(value.ToString()); break;
            case 1038: builder.SetSessionId(uint.Parse(value)); break;
            case 1020: builder.SetTotalSum(ulong.Parse(value)); break;
            case 1055: builder.SetTaxationSystem((TaxationSystems)int.Parse(value)); break;
            case 1008: builder.SetBuyerContact(value.ToString()); break;
            case 1042: builder.SetDocInSession(uint.Parse(value)); break;
            case 1040: builder.SetDocNumber(uint.Parse(value)); break;
            case 1054: builder.SetOperation((ReceiptType)byte.Parse(value)); break;
            // Суммы способов оплаты
            case 1031: builder.SetCashSum(ulong.Parse(value)); break;
            case 1081: builder.SetNonCashSum(ulong.Parse(value)); break;
            case 1215: builder.SetAdvanceSum(ulong.Parse(value)); break;
            case 1216: builder.SetCreditSum(ulong.Parse(value)); break;
            case 1217: builder.SetProvisionSum(ulong.Parse(value)); break;

            default: break;
        }
    }

    private void ChangePosition(ushort tag, ReadOnlySpan<char> value, PositionBuilder builder)
    {
        if (_propStack.Count != 1 || _propStack.Peek() != 1059) return;
        switch (tag)
        {
            case 2108: builder.SetUnit((UnitOfMeasure)byte.Parse(value)); break;
            case 1079: builder.SetPrice(ulong.Parse(value)); break;
            case 1043: builder.SetCost(ulong.Parse(value)); break;
            case 1199: builder.SetTaxRate(EnumUtils.ToTaxRate(byte.Parse(value))); break;
            case 1214: builder.SetCalcMethod((CalculationMethod)byte.Parse(value)); break;
            case 1212: builder.SetSubjectType((PositionType)byte.Parse(value)); break;
            case 1023: builder.SetQuantity((ulong)Math.Round(double.Parse(value, CultureInfo.InvariantCulture) * 1000, 0)); break;
            case 1030: builder.SetName(value.ToString()); break;

            default: break;
        }
    }
}