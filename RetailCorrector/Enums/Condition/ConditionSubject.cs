namespace RetailCorrector.Enums.Condition;

/// <summary>Субъект условия</summary>
public enum ConditionSubject
{
    None = -1,
    /// <summary>Реквизит чека/позиции</summary>
    Default,
    /// <summary>Реквизит хотя бы одного из позиций</summary>
    AnyPosition,
    /// <summary>Реквизит всех позиций</summary>
    AllPositions,
    /// <summary>Реквизит чека позиций</summary>
    ParentReceipt
}