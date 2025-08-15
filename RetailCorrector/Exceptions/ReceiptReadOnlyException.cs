namespace RetailCorrector.Exceptions
{
    internal class ReceiptReadOnlyException() :
        Exception("Запрещено редактировать чек, у которого активен режим \"Только для чтения\"");
}
