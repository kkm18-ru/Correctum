namespace Correctum.Enums;

public enum ReceiptMetadataProperty
{
    // 0 - "Разрешена запись"
    // 1 - Только чтение
    IsReadOnly,
    // 0 - Игнорировать
    // 1 - Корректировать
    // 2 - Аннулировать
    ActionMode
}
