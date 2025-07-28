namespace RetailCorrector.Parsers
{
    internal partial class Parser
    {
        public static Parser? Instance => 
            (Parser?)ParserWindow.Instance?.DataContext;

        public static void Show() => 
            new ParserWindow().ShowDialog();
    }
}
