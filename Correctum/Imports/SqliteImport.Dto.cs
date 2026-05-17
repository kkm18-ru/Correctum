using Correctum.Core.Enums;
using Correctum.Core.Models;
using Correctum.Core.ValueObjects;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace Correctum.Imports;

partial class SqliteImport
{
    internal readonly struct ReceiptDto
    {
        public ulong Id { get; init; }
        public string? DateTime { get; init; }
        public int? Type { get; init; }
        public string? Sign { get; init; }
        public string? KktRegId { get; init; }
        public string? DriveId { get; init; }
        public uint? Session { get; init; }
        public uint? DocId { get; init; }
        public uint? DocInSession { get; init; }
        public ulong? Cash { get; init; }
        public ulong? NonCash { get; init; }
        public ulong? Advance { get; init; }
        public ulong? Credit { get; init; }
        public ulong? Provision { get; init; }
        public ulong? Total { get; init; }
        public int? Taxation { get; init; }
        public string? Buyer { get; init; }
        public string? RetailAddress { get; init; }
        public string? SenderAddress { get; init; }
        public string? RetailPlace { get; init; }
        public string? Cashier { get; init; }
        public string? CashierInn { get; init; }

        public async Task<Receipt> ToModelAsync(SqliteConnection conn)
        {
            var items = await conn.QueryAsync<PositionDto>(GET_POSITIONS, this);
            return new()
            {
                Items = [.. items.Select(PositionDto.ToModel)],
                Created = DateTime is null ? null : System.DateTime.Parse(DateTime, CultureInfo.InvariantCulture),
                RegId = KktRegId,
                StorageId = DriveId,
                SessionId = Session,
                DocNumber = DocId,
                DocInSession = DocInSession,
                DocSign = Sign is null ? null : FiscalSign.Parse(Sign),
                TotalSum = (Currency?)Total,
                CashSum = (Currency?)Cash,
                NonCashSum = (Currency?)NonCash,
                AdvanceSum = (Currency?)Advance,
                CreditSum = (Currency?)Credit,
                ProvisionSum = (Currency?)Provision,
                Type = (ReceiptType?)Type,
                BuyerContact = Buyer,
                TaxationSystem = (TaxationSystems?)Taxation,
                RetailAddress = RetailAddress,
                SenderAddress = SenderAddress,
                RetailPlace = RetailPlace,
                OperatorName = Cashier,
                OperatorInn = CashierInn is null ? null : Inn.Parse(CashierInn),
            };
        }
    }

    internal readonly struct PositionDto
    {
        public string? Name { get; init; }
        public long? Type { get; init; }
        public long? CalcMethod { get; init; }
        public long? Tax { get; init; }
        public long? Unit { get; init; }
        public long? Price { get; init; }
        public long? Quantity { get; init; }
        public long? Cost { get; init; }

        public static Position ToModel(PositionDto dto) => new()
        {
            Name = dto.Name,
            Cost = (Currency?)dto.Cost,
            Unit = (UnitOfMeasure?)dto.Unit,
            CalcMethod = (CalculationMethod?)dto.CalcMethod,
            Price = (Currency?)dto.Price,
            Quantity = (ulong?)dto.Quantity,
            TaxRate = NDSRate.FromFns(dto.Tax),
            Type = (PositionType?)dto.Type
        };
    }
}
