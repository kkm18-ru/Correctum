using Correctum.Core.Enums;
using Correctum.Core.Models;
using Correctum.Storage.Enums;
using Correctum.Storage.Helpers;

namespace Correctum.Storage.Models
{
    internal readonly struct PositionDto : IDto<PositionDto, Position>
    {
        private string Name { get; init; }
        private byte Type { get; init; }
        private byte CalcMethod { get; init; }
        private byte TaxRate { get; init; }
        private byte Unit { get; init; }
        private ulong Price { get; init; }
        private decimal Quantity { get; init; }
        private ulong Cost { get; init; }
        private IsNullPositionProperties IsNullProperties { get; init; }

        private const int NameLength = 128;
        private const int ByteCount = 
            sizeof(IsNullPositionProperties) + NameLength + sizeof(byte) + 
            sizeof(byte) + sizeof(byte) + sizeof(byte) + 
            sizeof(ulong) + sizeof(decimal) + sizeof(ulong);

        private const int NameOffset = 0;
        private const int TypeOffset = 128;
        private const int CalcMethodOffset = 129;
        private const int TaxRateOffset = 130;
        private const int UnitOffset = 131;
        private const int PriceOffset = 132;
        private const int QuantityOffset = 140;
        private const int CostOffset = 156;
        private const int IsNullPropertiesOffset = 164;

        public readonly Position ToModel() => new()
        {
            Name       = GetOrNull(IsNullPositionProperties.Name, Name.TrimEnd(' ')),
            Type       = GetOrNull(IsNullPositionProperties.Type, (PositionType)Type),
            CalcMethod = GetOrNull(IsNullPositionProperties.CalcMethod, (CalculationMethod)CalcMethod),
            TaxRate    = GetOrNull(IsNullPositionProperties.TaxRate, (NDSRate)TaxRate),
            Unit       = GetOrNull(IsNullPositionProperties.Unit, (UnitOfMeasure)Unit),
            Price      = GetOrNull(IsNullPositionProperties.Price, Price),
            Quantity   = GetOrNull(IsNullPositionProperties.Quantity, Quantity),
            Cost       = GetOrNull(IsNullPositionProperties.Cost, Cost),
        };

        public static PositionDto FromModel(Position model) => new()
        {
            Name = model.Name ?? string.Empty,
            Type = (byte?)model.Type ?? default,
            CalcMethod = (byte?)model.CalcMethod ?? default,
            TaxRate = (byte?)model.TaxRate ?? default,
            Unit = (byte?)model.Unit ?? default,
            Price = (ulong?)model.Price ?? default,
            Quantity = model.Quantity ?? default,
            Cost = (ulong?)model.Cost ?? default,
            IsNullProperties =
                FlagIfNull(model.Name, IsNullPositionProperties.Name) |
                FlagIfNull(model.Type, IsNullPositionProperties.Type) |
                FlagIfNull(model.CalcMethod, IsNullPositionProperties.CalcMethod) |
                FlagIfNull(model.TaxRate, IsNullPositionProperties.TaxRate) |
                FlagIfNull(model.Unit, IsNullPositionProperties.Unit) |
                FlagIfNull(model.Price, IsNullPositionProperties.Price) |
                FlagIfNull(model.Quantity, IsNullPositionProperties.Quantity) |
                FlagIfNull(model.Cost, IsNullPositionProperties.Cost)
        };

        public void ToBytes(Span<byte> bytes)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(bytes.Length, ByteCount);
            BytesHelper.Write(bytes, NameOffset, NameLength, Name);
            BytesHelper.Write(bytes, TypeOffset, Type);
            BytesHelper.Write(bytes, CalcMethodOffset, CalcMethod);
            BytesHelper.Write(bytes, TaxRateOffset, TaxRate);
            BytesHelper.Write(bytes, UnitOffset, Unit);
            BytesHelper.Write(bytes, PriceOffset, Price);
            BytesHelper.Write(bytes, QuantityOffset, Quantity);
            BytesHelper.Write(bytes, CostOffset, Cost);
            BytesHelper.Write(bytes, IsNullPropertiesOffset, IsNullPropertiesOffset);
        }

        public static PositionDto FromBytes(ReadOnlySpan<byte> bytes)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(bytes.Length, ByteCount);
            return new()
            {
                Name = BytesHelper.ReadString(bytes, NameOffset, NameLength),
                Type = BytesHelper.ReadUInt8(bytes, TypeOffset),
                CalcMethod = BytesHelper.ReadUInt8(bytes, CalcMethodOffset),
                TaxRate = BytesHelper.ReadUInt8(bytes, TaxRateOffset),
                Unit = BytesHelper.ReadUInt8(bytes, UnitOffset),
                Price = BytesHelper.ReadUInt64(bytes, PriceOffset),
                Quantity = BytesHelper.ReadDecimal(bytes, QuantityOffset),
                Cost = BytesHelper.ReadUInt64(bytes, CostOffset),
                IsNullProperties = (IsNullPositionProperties)BytesHelper.ReadUInt8(bytes, IsNullPropertiesOffset)
            };
        }

        private static IsNullPositionProperties FlagIfNull<T>(T? value, IsNullPositionProperties flag) =>
            value is null ? 0 : flag;

        private readonly T? GetOrNull<T>(IsNullPositionProperties flag, T value) =>
            IsNullProperties.HasFlag(flag) ? default : value;
    }
}
