namespace RetailCorrector.Enums.Condition;

public enum ConditionOperator
{
    None = -1,
    
    StartsWith = 0,
    StartsWithIgnoreCase = 1,
    Contains = 2,
    ContainsIgnoreCase = 3,
    EndsWith = 4,
    EndsWithIgnoreCase = 5,
    
    Equal = 6,
    NotEqual = 7,
    LessThan = 8,
    EqualOrLessThan = 9,
    MoreThan = 10,
    EqualOrMoreThan = 11,
    
    IsTrue = 12,
    IsFalse = 13
}