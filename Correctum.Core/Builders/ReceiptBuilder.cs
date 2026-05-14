using Correctum.Core.Enums;
using Correctum.Core.Models;
using Correctum.Core.ValueObjects;

namespace Correctum.Core.Builders
{
    /// <summary>
    /// Построитель кассового чека с fluent-интерфейсом.
    /// </summary>
    public sealed class ReceiptBuilder
    {
        /// <summary>
        /// Возвращает текущее количество добавленных позиций.
        /// </summary>
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

        /// <summary>
        /// Инициализирует пустой построитель чека.
        /// </summary>
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

        /// <summary>
        /// Инициализирует построитель значениями существующего чека.
        /// </summary>
        /// <param name="receipt">Исходный чек.</param>
        public ReceiptBuilder(Receipt receipt)
        {
            _positions = [..receipt.Items?.Select(i => new PositionBuilder(i)) ?? []];
            _cash = (ulong?)receipt.CashSum;
            _noncash = (ulong?)receipt.NonCashSum;
            _advance = (ulong?)receipt.AdvanceSum;
            _credit = (ulong?)receipt.CreditSum;
            _provision = (ulong?)receipt.ProvisionSum;
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

        /// <summary>
        /// Возвращает построитель позиции по индексу в коллекции добавленных позиций.
        /// </summary>
        /// <param name="index">Индекс позиции.</param>
        public PositionBuilder this[int index] => _positions[index];

        /// <summary>
        /// Добавляет новую позицию в чек и возвращает её построитель.
        /// </summary>
        /// <returns>Построитель добавленной позиции.</returns>
        public PositionBuilder AddItem()
        {
            var builder = new PositionBuilder(this);
            _positions.Add(builder);
            return builder;
        }

        /// <summary>
        /// Устанавливает сумму оплаты наличными.
        /// </summary>
        /// <param name="value">Сумма в копейках.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        public ReceiptBuilder SetCashSum(Currency? value)
        {
            _cash = (ulong?)value;
            return this;
        }

        /// <summary>
        /// Устанавливает сумму безналичной оплаты.
        /// </summary>
        /// <param name="value">Сумма в копейках.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        public ReceiptBuilder SetNonCashSum(Currency? value)
        {
            _noncash = (ulong?)value;
            return this;
        }

        /// <summary>
        /// Устанавливает сумму оплаты авансом (предоплатой).
        /// </summary>
        /// <param name="value">Сумма в копейках.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        public ReceiptBuilder SetAdvanceSum(Currency? value)
        {
            _advance = (ulong?)value;
            return this;
        }

        /// <summary>
        /// Устанавливает сумму оплаты в кредит.
        /// </summary>
        /// <param name="value">Сумма в копейках.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        public ReceiptBuilder SetCreditSum(Currency? value)
        {
            _credit = (ulong?)value;
            return this;
        }

        /// <summary>
        /// Устанавливает сумму оплаты встречным предоставлением.
        /// </summary>
        /// <param name="value">Сумма в копейках.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        public ReceiptBuilder SetProvisionSum(Currency? value)
        {
            _provision = (ulong?)value;
            return this;
        }

        /// <summary>
        /// Устанавливает контакт покупателя (телефон или e-mail).
        /// </summary>
        /// <param name="value">Контакт покупателя.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        public ReceiptBuilder SetBuyerContact(string? value)
        {
            _buyer = value;
            return this;
        }

        /// <summary>
        /// Устанавливает дату и время формирования чека.
        /// </summary>
        /// <param name="value">Дата и время.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        public ReceiptBuilder SetCreated(DateTime? value)
        {
            _createdAt = value;
            return this;
        }

        /// <summary>
        /// Устанавливает порядковый номер документа в смене.
        /// </summary>
        /// <param name="value">Номер документа в смене.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        public ReceiptBuilder SetDocInSession(uint? value)
        {
            _docShift = value;
            return this;
        }

        /// <summary>
        /// Устанавливает номер смены.
        /// </summary>
        /// <param name="value">Номер смены.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        public ReceiptBuilder SetSessionId(uint? value)
        {
            _shift = value;
            return this;
        }

        /// <summary>
        /// Устанавливает номер фискального документа.
        /// </summary>
        /// <param name="value">Номер документа.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        public ReceiptBuilder SetDocNumber(uint? value)
        {
            _docId = value;
            return this;
        }

        /// <summary>
        /// Устанавливает тип операции чека.
        /// </summary>
        /// <param name="value">Тип операции.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если передано невалидное значение перечисления.</exception>
        public ReceiptBuilder SetOperation(ReceiptType? value)
        {
            if (value is { } notNull && Enum.GetName(notNull) is null)
                throw new ArgumentOutOfRangeException(nameof(value));
            _type = value;
            return this;
        }

        /// <summary>
        /// Устанавливает фискальный признак документа.
        /// </summary>
        /// <param name="value">Фискальный признак.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        public ReceiptBuilder SetDocSign(FiscalSign? value)
        {
            _sign = value;
            return this;
        }

        /// <summary>
        /// Устанавливает регистрационный номер ККТ.
        /// </summary>
        /// <param name="value">Регистрационный номер.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        public ReceiptBuilder SetRegId(string? value)
        {
            _regId = value;
            return this;
        }

        /// <summary>
        /// Устанавливает серийный номер фискального накопителя.
        /// </summary>
        /// <param name="value">Серийный номер накопителя.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        public ReceiptBuilder SetStorageId(string? value)
        {
            _drive = value;
            return this;
        }

        /// <summary>
        /// Устанавливает систему налогообложения, применяемую в чеке.
        /// </summary>
        /// <param name="value">Система налогообложения.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если передано невалидное значение перечисления.</exception>
        public ReceiptBuilder SetTaxationSystem(TaxationSystems? value)
        {
            if (value is { } notNull && Enum.GetName(notNull) is null)
                throw new ArgumentOutOfRangeException(nameof(value));
            _taxation = value;
            return this;
        }

        /// <summary>
        /// Устанавливает итоговую сумму чека.
        /// </summary>
        /// <param name="value">Итоговая сумма в копейках.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        public ReceiptBuilder SetTotalSum(Currency? value)
        {
            _total = (ulong?)value;
            return this;
        }

        /// <summary>
        /// Создаёт экземпляр <see cref="Receipt"/> из установленных значений.
        /// </summary>
        /// <returns>Построенный кассовый чек.</returns>
        public Receipt Build() => new()
        {
            Items = [.. _positions.Select(b => b.Build())],
                CashSum = _cash,
                NonCashSum = _noncash,
                AdvanceSum = _advance,
                CreditSum = _credit,
                ProvisionSum = _provision,
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
