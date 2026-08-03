namespace Correctum.Enums;

/// <remarks>
/// Ошибки кодируются в первых двух байтах, в следующих двух байта - предупреждения 
/// </remarks>
[Flags]
public enum AtolKktProblems : uint
{
    None = 0,

    #region Маски
    ErrorsMask = 0xffff0000,
    WarningsMask = 0x0000ffff,
    #endregion

    #region Предупреждения

    AdditionalPrintExpired = 0x00000001,
    AdditionalPrintNoFound  = 0x00000002,
    ConfigurationOutdated   = 0x00000004,
    
    ModelKktUnknown         = 0x00000010,
    CheckKktUnknown         = 0x00000020,
    CheckKktError           = 0x00000040,
    CheckKktNeedRegister    = 0x00000080,
    
    ModelFnUnknown          = 0x00000100,
    CheckFnUnknown          = 0x00000200,
    CheckFnError            = 0x00000400,
    CheckFnNeedRegister     = 0x00000800,

    #endregion

    #region Ошибки

    CheckKktNoCreated   = 0x00100000,
    CheckFnNoCreated    = 0x02000000,
    CheckFnArchived     = 0x04000000,
    
    #endregion
}

public static class AtolKktProblemsExtensions
{
    extension(AtolKktProblems problems)
    {
        public bool HasErrors => (problems & AtolKktProblems.ErrorsMask) != 0;
    }    
}