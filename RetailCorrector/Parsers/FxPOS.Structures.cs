namespace RetailCorrector.Parsers
{
    internal partial class FxPOS
    {
        private struct Document
        {
            public int ID { get; set; }
            public DateTime CLOSEDATETIME { get; set; }
            public long TOTAL { get; set; }
        }
    }
}
