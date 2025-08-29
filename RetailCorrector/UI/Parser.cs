namespace RetailCorrector.UI
{
    internal partial class Parser
    {
        public static Parser? Instance => 
            (Parser?)View.Instance?.DataContext;

        public static void Show() => 
            new View().ShowDialog();
    }
}
