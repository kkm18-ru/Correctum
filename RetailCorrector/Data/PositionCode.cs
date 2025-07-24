namespace RetailCorrector.Data
{
    internal struct PositionCode
    {
        public string Unknown { get; set; }
        public string EAN8 { get; set; }
        public string EAN13 { get; set; }
        public string ITF14 { get; set; }
        public string GS1_0 { get; set; }
        public string GS1_M { get; set; }
        public string KMK { get; set; }
        public string MI { get; set; }
        public string EGAIS2 { get; set; }
        public string EGAIS3 { get; set; }
        public string F1 { get; set; }
        public string F2 { get; set; }
        public string F3 { get; set; }
        public string F4 { get; set; }
        public string F5 { get; set; }
        public string F6 { get; set; }

        public readonly string? Code 
        {
            get
            {
                var arr = new[] { 
                    GS1_0, GS1_M, KMK, EGAIS2, EGAIS3, EAN8, EAN13,
                    ITF14, MI, F1, F2, F3, F4, F5, F6, Unknown
                };
                return arr.FirstOrDefault(c => !string.IsNullOrEmpty(c));
            }
        }
    }
}
