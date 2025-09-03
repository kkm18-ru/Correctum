namespace RetailCorrector.Parsers
{
    internal partial class FxPOS
    {
        const int LIMIT = 100;
        const string COUNT_RECEIPTS = "select count(ID) from DOCUMENTS where DOCUMENTSTATE = 1 and ECRDOCUMENTTYPE = 0;";
        const string GET_RECEIPTS = """
            select 
                d.ID as Id, 
                d.CLOSEDATETIME as Date, 
                cast(d.TOTAL/100 as integer) as Total,
                cast(tr.WAREMARK as integer) as Sign,
                cast(d.ECRDOCUMENTTYPE as integer) as Type
            from DOCUMENTS as d inner join TRANSACTIONS as tr on tr.DOCUMENTID = d.ID
            where d.DOCUMENTSTATE = 1 and d.ECRDOCUMENTTYPE = 0 and tr.TRTYPE = 45 
            limit @limit offset @offset;
            """;
        const string GET_POSITIONS = 
            """
            select             
                cast(w.NAME as TEXT) as Name, 
                cast(w.PAYMENT_TYPE as INTEGER) as PayMethod,
                cast(w.ITEM_TYPE as INTEGER) as Type,
                (select cast(VALUE/1000000 as INTEGER) from TAXRATE where tr.TAXRATE_CODE = CODE) as TaxRate,
                cast(tr.PRICE/100 as INTEGER) as Price,
                cast(tr.QUANTITY/1000 as INTEGER) as Quantity,    
                cast(w.MEASURE as INTEGER) as MeasureUnit,
                cast(tr.TOTAL/100 as INTEGER) as Cost
            from TRANSACTIONS as tr inner join WARE as w on w.CODE = tr.WARECODE
            where tr.TRTYPE in (6,16) and tr.DOCUMENTID = @Id;
            """;
        const string GET_PAYMENT =
            """  
            select 
                GROUP_CONCAT(CASE WHEN Type == 0 THEN Sum ELSE 0 END) as Cash,
                GROUP_CONCAT(CASE WHEN Type == 1 THEN Sum ELSE 0 END) as ECash,
                GROUP_CONCAT(CASE WHEN Type == 2 THEN Sum ELSE 0 END) as Pre,
                GROUP_CONCAT(CASE WHEN Type == 3 THEN Sum ELSE 0 END) as Post,
                GROUP_CONCAT(CASE WHEN Type == 4 THEN Sum ELSE 0 END) as Provision
            from (select p.ECRPAYMENT as Type, sum(total)/100 as Sum
                from TRANSACTIONS as tr inner join PAYMENT as p on tr.warecode = p.CODE
                where tr.TRTYPE in (40, 41) and tr.documentid = @Id
                group by p.ECRPAYMENT);
            """;
    }
}
