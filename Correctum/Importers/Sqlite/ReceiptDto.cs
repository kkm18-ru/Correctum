using Correctum.Core.Enums;
using Correctum.Core.Models;
using Correctum.Core.ValueObjects;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace Correctum.Importers;

partial class Sqlite
{
    internal readonly struct ReceiptDto
    {
        public ulong Id { get; init; }
        public string? DateTime { get; init; }
        public int? Type { get; init; }
        public string? Sign { get; init; }
        public string? KktRegId { get; init; }
        public string? DriveId { get; init; }
        public string? Cashier { get; init; }
        public string? CashierInn { get; init; }
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
                Payment = new Payment
                {
                    CashSum = (Currency?)Cash,
                    NonCashSum = (Currency?)NonCash,
                    AdvanceSum = (Currency?)Advance,
                    CreditSum = (Currency?)Credit,
                    ProvisionSum = (Currency?)Provision,
                },
                Type = (ReceiptType?)Type,
                BuyerContact = Buyer,
                TaxationSystem = (TaxationSystems?)Taxation,
                CashierInn = CashierInn is null ? null : Inn.Parse(CashierInn),
                Cashier = Cashier,
            };
        }
    }
}