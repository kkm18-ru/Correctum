namespace RetailCorrector.Parsers
{
    internal partial class Sqlite
    {
        const string COUNT_RECEIPTS = "select count(id) from receipt;";
        const string GET_RECEIPTS = "select * from receipt limit @limit offset @offset;";
        const string GET_POSITIONS = "select * from position where receipt = @id";
    }
}
