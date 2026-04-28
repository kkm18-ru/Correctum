using Correctum.Core.Enums;
using Correctum.Core.Models;
using Correctum.Core.ValueObjects;
using Correctum.Storage.Enums;
using Correctum.Storage.Helpers;

namespace Correctum.Storage.Models
{
    internal struct ReceiptDto : IDto<ReceiptDto, Receipt>
    {
        private ulong RegId { get; init; }
        private ulong StorageId { get; init; }
        private uint DocNumber { get; init; }
        private uint SessionId { get; init; }
        private uint DocInSession { get; init; }
        private long CreatedAt { get; init; }
        private uint FiscalSign { get; init; }
        private byte Type { get; init; }
        private ulong CashSum { get; init; }
        private ulong NonCashSum { get; init; }
        private ulong AdvanceSum { get; init; }
        private ulong CreditSum { get; init; }
        private ulong ProvisionSum { get; init; }
        private ulong TotalSum { get; init; }
        private string BuyerContact { get; init; }
        private byte TaxationSystem { get; init; }
        private uint PositionIndex { get; init; }
        private byte PositionCount { get; init; }
        private IsNullReceiptProperties IsNullProperties { get; init; }

        private const int BuyerContactLength = 64;
        private const int ByteCount =
            sizeof(ulong) + sizeof(ulong) + sizeof(uint) + sizeof(uint) +
            sizeof(uint) + sizeof(ulong) + sizeof(uint) + sizeof(byte) +
            sizeof(ulong) + sizeof(ulong) + sizeof(ulong) + sizeof(ulong) +
            sizeof(ulong) + sizeof(ulong) + BuyerContactLength + sizeof(byte) +
            sizeof(uint) + sizeof(byte) + sizeof(IsNullReceiptProperties);

        private const int RegIdOffset = 0;
        private const int StorageIdOffset = 8;
        private const int DocNumberOffset = 16;
        private const int SessionIdOffset = 20;
        private const int DocInSessionOffset = 24;
        private const int CreatedAtOffset = 28;
        private const int FiscalSignOffset = 36;
        private const int TypeOffset = 40;
        private const int CashSumOffset = 41;
        private const int NonCashSumOffset = 49;
        private const int AdvanceSumOffset = 57;
        private const int CreditSumOffset = 65;
        private const int ProvisionSumOffset = 73;
        private const int TotalSumOffset = 81;
        private const int BuyerContactOffset = 89;
        private const int TaxationSystemOffset = 153;
        private const int PositionIndexOffset = 154;
        private const int PositionCountOffset = 158;
        private const int IsNullPropertiesOffset = 159;

        public static ReceiptDto FromBytes(ReadOnlySpan<byte> bytes)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(bytes.Length, ByteCount);
            return new()
            {
                RegId = BytesHelper.ReadUInt64(bytes, RegIdOffset),
                StorageId = BytesHelper.ReadUInt64(bytes, StorageIdOffset),
                DocNumber = BytesHelper.ReadUInt32(bytes, DocNumberOffset),
                SessionId = BytesHelper.ReadUInt32(bytes, SessionIdOffset),
                DocInSession = BytesHelper.ReadUInt32(bytes, DocInSessionOffset),
                CreatedAt = BytesHelper.ReadInt64(bytes, CreatedAtOffset),
                FiscalSign = BytesHelper.ReadUInt32(bytes, FiscalSignOffset),
                Type = BytesHelper.ReadUInt8(bytes, TypeOffset),
                CashSum = BytesHelper.ReadUInt64(bytes, CashSumOffset),
                NonCashSum = BytesHelper.ReadUInt64(bytes, NonCashSumOffset),
                AdvanceSum = BytesHelper.ReadUInt64(bytes, AdvanceSumOffset),
                CreditSum = BytesHelper.ReadUInt64(bytes, CreditSumOffset),
                ProvisionSum = BytesHelper.ReadUInt64(bytes, ProvisionSumOffset),
                TotalSum = BytesHelper.ReadUInt64(bytes, TotalSumOffset),
                BuyerContact = BytesHelper.ReadString(bytes, BuyerContactOffset, BuyerContactLength),
                TaxationSystem = BytesHelper.ReadUInt8(bytes, TaxationSystemOffset),
                PositionIndex = BytesHelper.ReadUInt32(bytes, PositionIndexOffset),
                PositionCount = BytesHelper.ReadUInt8(bytes, PositionCountOffset),
                IsNullProperties = (IsNullReceiptProperties)BytesHelper.ReadUInt32(bytes, IsNullPropertiesOffset),
            };
        }

        public static ReceiptDto FromModel(Receipt model) => new()
        {
            RegId = model.RegId ?? default,
            StorageId = model.StorageId ?? default,
            DocNumber = model.DocNumber ?? default,
            SessionId = model.SessionId ?? default,
            DocInSession = model.DocInSession??default,
            CreatedAt = model.Created?.Ticks ?? default,
            FiscalSign = (uint?)model.DocSign ?? default,
            Type = (byte?)model.Type ?? default,
            CashSum = (ulong?)model.CashSum ?? default,
            NonCashSum = (ulong?)model.NonCashSum ?? default,
            AdvanceSum = (ulong?)model.AdvanceSum ?? default,
            CreditSum = (ulong?)model.CreditSum ?? default,
            ProvisionSum = (ulong?)model.ProvisionSum ?? default,
            TotalSum = (ulong?)model.TotalSum ?? default,
            BuyerContact = model.BuyerContact ?? string.Empty,
            TaxationSystem = (byte?)model.TaxationSystem ?? default,
            PositionCount = (byte?)model.Items?.Length ?? default,
            IsNullProperties = 
                FlagIfNull(model.RegId,          IsNullReceiptProperties.RegId         ) |
                FlagIfNull(model.StorageId,      IsNullReceiptProperties.StorageId     ) |
                FlagIfNull(model.DocNumber,      IsNullReceiptProperties.DocNumber     ) |
                FlagIfNull(model.SessionId,      IsNullReceiptProperties.SessionId     ) |
                FlagIfNull(model.DocInSession,   IsNullReceiptProperties.DocInSession  ) |
                FlagIfNull(model.Created,        IsNullReceiptProperties.CreatedAt     ) |
                FlagIfNull(model.DocSign,        IsNullReceiptProperties.FiscalSign    ) |
                FlagIfNull(model.Type,           IsNullReceiptProperties.Type          ) |
                FlagIfNull(model.CashSum,        IsNullReceiptProperties.CashSum       ) |
                FlagIfNull(model.NonCashSum,     IsNullReceiptProperties.NonCashSum    ) |
                FlagIfNull(model.AdvanceSum,     IsNullReceiptProperties.AdvanceSum    ) |
                FlagIfNull(model.CreditSum,      IsNullReceiptProperties.CreditSum     ) |
                FlagIfNull(model.ProvisionSum,   IsNullReceiptProperties.ProvisionSum  ) |
                FlagIfNull(model.TotalSum,       IsNullReceiptProperties.TotalSum      ) |
                FlagIfNull(model.BuyerContact,   IsNullReceiptProperties.BuyerContact  ) |
                FlagIfNull(model.TaxationSystem, IsNullReceiptProperties.TaxationSystem) |
                FlagIfNull(model.Items,          IsNullReceiptProperties.Positions     ),
        };

        public readonly void ToBytes(Span<byte> bytes)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(bytes.Length, ByteCount);
            BytesHelper.Write(bytes, RegIdOffset, RegId);
            BytesHelper.Write(bytes, StorageIdOffset, StorageId);
            BytesHelper.Write(bytes, DocNumberOffset, DocNumber);
            BytesHelper.Write(bytes, SessionIdOffset, SessionId);
            BytesHelper.Write(bytes, DocInSessionOffset, DocInSession);
            BytesHelper.Write(bytes, CreatedAtOffset, CreatedAt);
            BytesHelper.Write(bytes, FiscalSignOffset, FiscalSign);
            BytesHelper.Write(bytes, TypeOffset, Type);
            BytesHelper.Write(bytes, CashSumOffset, CashSum);
            BytesHelper.Write(bytes, NonCashSumOffset, NonCashSum);
            BytesHelper.Write(bytes, AdvanceSumOffset, AdvanceSum);
            BytesHelper.Write(bytes, CreditSumOffset, CreditSum);
            BytesHelper.Write(bytes, ProvisionSumOffset, ProvisionSum);
            BytesHelper.Write(bytes, TotalSumOffset, TotalSum);
            BytesHelper.Write(bytes, BuyerContactOffset, BuyerContactLength, BuyerContact);
            BytesHelper.Write(bytes, TaxationSystemOffset, TaxationSystem);
            BytesHelper.Write(bytes, PositionIndexOffset, PositionIndex);
            BytesHelper.Write(bytes, PositionCountOffset, PositionCount);
            BytesHelper.Write(bytes, IsNullPropertiesOffset, (uint)IsNullProperties);
        }

        public Receipt ToModel() => new()
        {
            RegId =          GetOrNull(IsNullReceiptProperties.RegId, RegId),
            StorageId =      GetOrNull(IsNullReceiptProperties.StorageId, StorageId),
            DocNumber =      GetOrNull(IsNullReceiptProperties.DocNumber, DocNumber),
            SessionId =      GetOrNull(IsNullReceiptProperties.SessionId, SessionId),
            DocInSession =   GetOrNull(IsNullReceiptProperties.DocInSession, DocInSession),
            CashSum =        GetOrNull(IsNullReceiptProperties.CashSum, (Currency)CashSum),
            NonCashSum =     GetOrNull(IsNullReceiptProperties.NonCashSum, (Currency)NonCashSum),
            AdvanceSum =     GetOrNull(IsNullReceiptProperties.AdvanceSum, (Currency)AdvanceSum),
            CreditSum =      GetOrNull(IsNullReceiptProperties.CreditSum, (Currency)CreditSum),
            ProvisionSum =   GetOrNull(IsNullReceiptProperties.ProvisionSum, (Currency)ProvisionSum),
            TotalSum =       GetOrNull(IsNullReceiptProperties.TotalSum, (Currency)TotalSum),
            BuyerContact =   GetOrNull(IsNullReceiptProperties.BuyerContact, BuyerContact),
            Created =        GetOrNull(IsNullReceiptProperties.CreatedAt, new DateTime(CreatedAt, 0)),
            DocSign =        GetOrNull(IsNullReceiptProperties.FiscalSign, FiscalSign),
            TaxationSystem = GetOrNull(IsNullReceiptProperties.TaxationSystem, (TaxationSystems)TaxationSystem),
            Type =           GetOrNull(IsNullReceiptProperties.Type, (ReceiptType)Type),
            Items =          GetOrNull(IsNullReceiptProperties.Positions, 
                Storage.Current.DtoPositions
                    .Skip((int)PositionIndex)
                    .Take(PositionCount)
                    .Select(p => p.ToModel())
                    .ToArray()),
        };

        private static IsNullReceiptProperties FlagIfNull<T>(T? value, IsNullReceiptProperties flag) =>
            value is null ? 0 : flag;

        private readonly T? GetOrNull<T>(IsNullReceiptProperties flag, T value) =>
            IsNullProperties.HasFlag(flag) ? default : value;
    }
}
