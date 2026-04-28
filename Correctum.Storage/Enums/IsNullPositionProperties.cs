namespace Correctum.Storage.Enums;

[Flags] internal enum IsNullPositionProperties : byte
{
    Name = 1 << 0,
    Type = 1 << 1,
    CalcMethod = 1 << 2,
    TaxRate = 1 << 3,
    Unit = 1 << 4,
    Price = 1 << 5,
    Quantity = 1 << 6,
    Cost = 1 << 7,
}
