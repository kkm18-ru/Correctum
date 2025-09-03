namespace RetailCorrector.Parsers
{
    internal partial class FxPOS
    {
        const int LIMIT = 100;
        const string COUNT_RECEIPTS = "select count(ID) from DOCUMENTS where DOCUMENTSTATE = 1 and ECRDOCUMENTTYPE = 0;";
        const string GET_RECEIPTS = "select ID, CLOSEDATETIME, TOTAL from DOCUMENTS where DOCUMENTSTATE = 1 and ECRDOCUMENTTYPE = 0 limit @limit offset @offset;";
    }
}
