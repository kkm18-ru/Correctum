using Correctum.Core.Enums;
using Correctum.Core.Models;
using Correctum.Core.ValueObjects;

namespace Correctum.Core.Builders
{
    public sealed class ReceiptBuilder
    {
        public int Count => _positions.Count;

        private readonly List<PositionBuilder> _positions;
        private ulong? _cash;
        private ulong? _noncash;
        private ulong? _advance;
        private ulong? _credit;
        private ulong? _provision;
        private string? _buyer;
        private DateTime? _createdAt;
        private uint? _docShift;
        private uint? _docId;
        private FiscalSign? _sign;
        private string? _regId;
        private uint? _shift;
        private string? _drive;
        private TaxationSystems? _taxation;
        private ulong? _total;
        private ReceiptType? _type;

        public ReceiptBuilder()
        {
            _positions = [];
            _cash = null;
            _noncash = null;
            _advance = null;
            _credit = null;
            _provision = null;
            _buyer = null;
            _createdAt = null;
            _docShift = null;
            _docId = null;
            _sign = null;
            _regId = null;
            _shift = null;
            _drive = null;
            _taxation = null;
            _total = null;
            _type = null;
        }

        public ReceiptBuilder(Receipt receipt)
        {
            _positions = [..receipt.Items?.Select(i => new PositionBuilder(i)) ?? []];
            _cash = receipt.Payment is null ? null : (ulong?)receipt.Payment.Value.CashSum;
            _noncash = receipt.Payment is null ? null : (ulong?)receipt.Payment.Value.NonCashSum;
            _advance = receipt.Payment is null ? null : (ulong?)receipt.Payment.Value.AdvanceSum;
            _credit = receipt.Payment is null ? null : (ulong?)receipt.Payment.Value.CreditSum;
            _provision = receipt.Payment is null ? null : (ulong?)receipt.Payment.Value.ProvisionSum;
            _buyer = receipt.BuyerContact;
            _createdAt = receipt.Created;
            _docShift = receipt.DocInSession;
            _docId = receipt.DocNumber;
            _sign = receipt.DocSign;
            _regId = receipt.RegId;
            _shift = receipt.SessionId;
            _drive = receipt.StorageId;
            _taxation = receipt.TaxationSystem;
            _total = (ulong?)receipt.TotalSum;
            _type = receipt.Type;
        }

        public PositionBuilder this[int index] => _positions[index];

        public PositionBuilder AddItem()
        {
            var builder = new PositionBuilder(this);
            _positions.Add(builder);
            return builder;
        }

        public ReceiptBuilder SetCashSum(Currency value)
        {
            _cash = (ulong)value;
            return this;
        }

        public ReceiptBuilder SetNonCashSum(Currency value)
        {
            _noncash = (ulong)value;
            return this;
        }

        public ReceiptBuilder SetAdvanceSum(Currency value)
        {
            _advance = (ulong)value;
            return this;
        }

        public ReceiptBuilder SetCreditSum(Currency value)
        {
            _credit = (ulong)value;
            return this;
        }

        public ReceiptBuilder SetProvisionSum(Currency value)
        {
            _provision = (ulong)value;
            return this;
        }

        public ReceiptBuilder SetBuyerContact(string value)
        {
            _buyer = value;
            return this;
        }

        public ReceiptBuilder SetCreated(DateTime value)
        {
            _createdAt = value;
            return this;
        }

        public ReceiptBuilder SetDocInSession(uint value)
        {
            _docShift = value;
            return this;
        }

        public ReceiptBuilder SetSessionId(uint value)
        {
            _shift = value;
            return this;
        }

        public ReceiptBuilder SetDocNumber(uint value)
        {
            _docId = value;
            return this;
        }

        public ReceiptBuilder SetOperation(ReceiptType value)
        {
            if (Enum.GetName(value) is null)
                throw new ArgumentOutOfRangeException(nameof(value));
            _type = value;
            return this;
        }

        public ReceiptBuilder SetDocSign(FiscalSign value)
        {
            _sign = value;
            return this;
        }

        public ReceiptBuilder SetRegId(string value)
        {
            _regId = value;
            return this;
        }

        public ReceiptBuilder SetStorageId(string value)
        {
            _drive = value;
            return this;
        }

        public ReceiptBuilder SetTaxationSystem(TaxationSystems value)
        {
            if (Enum.GetName(value) is null)
                throw new ArgumentOutOfRangeException(nameof(value));
            _taxation = value;
            return this;
        }

        public ReceiptBuilder SetTotalSum(Currency value)
        {
            _total = (ulong)value;
            return this;
        }

        public Receipt Build() => new()
        {
            Items = [.. _positions.Select(b => b.Build())],
            Payment = new Payment()
            {
                CashSum = _cash,
                NonCashSum = _noncash,
                AdvanceSum = _advance,
                CreditSum = _credit,
                ProvisionSum = _provision,
            },
            BuyerContact = _buyer,
            Created = _createdAt,
            DocInSession = _docShift,
            DocNumber = _docId,
            DocSign = _sign,
            RegId = _regId,
            SessionId = _shift,
            StorageId = _drive,
            TaxationSystem = _taxation,
            TotalSum = _total,
            Type = _type,
        };
    }
}
