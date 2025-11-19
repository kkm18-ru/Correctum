namespace RetailCorrector.Exporters
{
    internal partial class Sqlite
    {
        const string CREATE_POSITION_TABLE = """
            CREATE TABLE position (
                id       INTEGER NOT NULL
                                 CONSTRAINT PK_position PRIMARY KEY AUTOINCREMENT,
                name     TEXT,
                payment  INTEGER NOT NULL,
                type     INTEGER NOT NULL,
                tax      INTEGER NOT NULL,
                unit     INTEGER,
                price    INTEGER NOT NULL,
                quantity INTEGER NOT NULL,
                total    INTEGER NOT NULL,
                receipt  INTEGER NOT NULL,
                CONSTRAINT FK_position_receipt_receipt FOREIGN KEY (
                    receipt
                )
                REFERENCES receipt (id) ON DELETE CASCADE
            );
            """;
        const string CREATE_RECEIPT_TABLE = """
            CREATE TABLE receipt (
                id    INTEGER NOT NULL
                              CONSTRAINT PK_receipt PRIMARY KEY AUTOINCREMENT,
                date  TEXT    NOT NULL,
                type  INTEGER NOT NULL,
                sign  TEXT    NOT NULL,
                corr  INTEGER NOT NULL,
                buyer TEXT,
                act   TEXT,
                cash  INTEGER NOT NULL,
                ecash INTEGER NOT NULL,
                pre   INTEGER NOT NULL,
                post  INTEGER NOT NULL,
                other INTEGER NOT NULL,
                total INTEGER NOT NULL
            );            
            """;
        const string CREATE_PKEY_RECEIPT_INDEX = "CREATE INDEX receipt_pkey ON receipt (id);";
        const string CREATE_RID_POSITION_INDEX = "CREATE INDEX position_rid ON position (receipt);";
        const string ADD_RECEIPT = """
            INSERT INTO receipt (date, type, sign, corr, act, cash, ecash, pre, post, other, total, buyer) 
            VALUES (@date, @type, @sign, 0, ' ', @cash, @ecash, @pre, @post, @other, @total, @buyer) RETURNING id;
            """;
        const string ADD_POSITION = """
            INSERT INTO position (name, payment, type, tax, unit, price, quantity, total, receipt) 
            VALUES (@name, @payment, @type, @tax, @unit, @price, @quantity, @total, @receipt);
            """;
    }
}
