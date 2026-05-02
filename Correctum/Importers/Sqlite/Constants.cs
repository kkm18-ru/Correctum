namespace Correctum.Importers;

internal partial class Sqlite
{
    const string COUNT_RECEIPTS = "SELECT COUNT(id) FROM receipt;";
    const string GET_RECEIPTS = """
        SELECT 
            id AS Id,
            date AS DateTime,
            type AS Type,
            sign AS Sign,
            devid AS KktRegId,
            drive AS DriveId,
            shift AS Session,
            doc AS DocId,
            req AS DocInSession,
            cash AS Cash,
            ecash AS NonCash,
            pre AS Advance,
            post AS Credit,
            other AS Provision,
            total AS Total,
            taxation AS Taxation,
            buyer AS Buyer
        FROM 
            receipt 
        LIMIT @limit OFFSET @offset;
        """;
    const string GET_POSITIONS = """
        SELECT 
            name AS Name,
            type AS Type,
            payment AS CalcMethod,
            tax AS Tax,
            unit AS Unit,
            price AS Price,
            quantity AS Quantity,
            total AS Cost,
        FROM 
            position 
        WHERE 
            receipt = @Id;
        """;
}
