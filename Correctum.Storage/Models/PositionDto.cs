using Correctum.Core.Enums;
using Correctum.Core.Models;
using Correctum.Storage.Enums;
using Correctum.Storage.Extensions;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;

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
            Name        = IsNullProperties.HasFlag(IsNullPositionProperties.Name)       ? null : Name.TrimEnd(' '),
            Type        = IsNullProperties.HasFlag(IsNullPositionProperties.Type)       ? null : (PositionType)Type,
            CalcMethod  = IsNullProperties.HasFlag(IsNullPositionProperties.CalcMethod) ? null : (CalculationMethod)CalcMethod,
            TaxRate     = IsNullProperties.HasFlag(IsNullPositionProperties.TaxRate)    ? null : (NDSRate)TaxRate,
            Unit        = IsNullProperties.HasFlag(IsNullPositionProperties.Unit)       ? null : (UnitOfMeasure)Unit,
            Price       = IsNullProperties.HasFlag(IsNullPositionProperties.Price)      ? null : Price,
            Quantity    = IsNullProperties.HasFlag(IsNullPositionProperties.Quantity)   ? null : Quantity,
            Cost        = IsNullProperties.HasFlag(IsNullPositionProperties.Cost)       ? null : Cost,
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
                (model.Name         is null ? IsNullPositionProperties.Name         : 0) |
                (model.Type         is null ? IsNullPositionProperties.Type         : 0) |
                (model.CalcMethod   is null ? IsNullPositionProperties.CalcMethod   : 0) |
                (model.TaxRate      is null ? IsNullPositionProperties.TaxRate      : 0) |
                (model.Unit         is null ? IsNullPositionProperties.Unit         : 0) |
                (model.Price        is null ? IsNullPositionProperties.Price        : 0) |
                (model.Quantity     is null ? IsNullPositionProperties.Quantity     : 0) |
                (model.Cost         is null ? IsNullPositionProperties.Cost         : 0)
        };

        public void ToBytes(Span<byte> bytes)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(bytes.Length, ByteCount);
            Encoding.CorrectumStorage.GetBytes(Name).CopyTo(bytes.Slice(NameOffset, NameLength));
            bytes[TypeOffset] = Type;
            bytes[CalcMethodOffset] = CalcMethod;
            bytes[TaxRateOffset] = TaxRate;
            bytes[UnitOffset] = Unit;
            BinaryPrimitives.WriteUInt64LittleEndian(bytes.Slice(PriceOffset, sizeof(ulong)), Price);
            MemoryMarshal.Write(bytes.Slice(QuantityOffset, sizeof(decimal)), Quantity);
            BinaryPrimitives.WriteUInt64LittleEndian(bytes.Slice(CostOffset, sizeof(ulong)), Cost);
            bytes[IsNullPropertiesOffset] = IsNullPropertiesOffset;
        }

        public static PositionDto FromBytes(ReadOnlySpan<byte> bytes)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(bytes.Length, ByteCount);
            return new()
            {
                Name = Encoding.CorrectumStorage.GetString(bytes.Slice(NameOffset, NameLength)),
                Type = bytes[TypeOffset],
                CalcMethod = bytes[CalcMethodOffset],
                TaxRate = bytes[TaxRateOffset],
                Unit = bytes[UnitOffset],
                Price = BinaryPrimitives.ReadUInt64LittleEndian(bytes.Slice(PriceOffset, sizeof(ulong))),
                Quantity = MemoryMarshal.Read<decimal>(bytes.Slice(QuantityOffset, sizeof(decimal))),
                Cost = BinaryPrimitives.ReadUInt64LittleEndian(bytes.Slice(CostOffset, sizeof(ulong))),
                IsNullProperties = (IsNullPositionProperties)bytes[IsNullPropertiesOffset]
            };
        }
    }
}
