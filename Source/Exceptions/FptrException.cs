using Correctum.Utils.Integrations;

namespace Correctum.Exceptions;

/// <summary>
/// Исключение, которое возникает, когда метод Fptr-драйвера завершился с ошибкой.
/// </summary>
public sealed class FptrException : Exception
{
    /// <summary>
    /// Код ошибки. 
    /// <see href="https://integration.atol.ru/api/#error_list">Список кодов ошибок</see>
    /// </summary>
    public int Code { get; }

    /// <summary>
    /// Текстовое описание ошибки.
    /// </summary>
    public string Description { get; }

    internal FptrException(FptrHandle driver) : this(driver.ErrorCode, driver.ErrorDescription) { }
    private FptrException(int code, string description) : base($"[{code}] {description}")
    {
        Code = code;
        Description = description;
    }
}
