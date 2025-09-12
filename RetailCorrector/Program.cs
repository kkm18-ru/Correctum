namespace RetailCorrector
{
    internal class Program
    {
        [STAThread]
        public static void Main()
        {
            App.Instance.Run(UI.Windows.MainWindow.Instance);
        }
    }
}
