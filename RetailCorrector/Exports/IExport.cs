using System.Windows.Controls;

namespace RetailCorrector.Exports
{
    internal interface IExport
    {
        ValueTask<string> Export();
        void InitVisual(Grid content) { }
    }
}
