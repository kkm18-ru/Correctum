namespace Correctum.Enums;

public enum KktFnStatusCode : sbyte
{
    /// <summary>
    /// ККТ и/или ФН зарегистрированы в ФНС
    /// </summary>
    Ok,
    /// <summary>
    /// ККТ и/или ФН не зарегистрированы в ФНС
    /// </summary>
    NeedRegister,
    /// <summary>
    /// Отсутствует запись о произведенном экземпляре ККТ и/или ФН
    /// </summary>
    NoCreated,
    /// <summary>
    /// ФН закрыт и не может быть использован
    /// </summary>
    Archived,

    /// <summary>
    /// ФНС вернула ошибку при обработке запроса
    /// </summary>
    Error = -1,
    /// <summary>
    /// Неизвестный ответа (Не удалось определить статус)
    /// </summary>
    Unknown = -2
}