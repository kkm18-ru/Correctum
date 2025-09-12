using System.Windows.Controls;

namespace RetailCorrector.Exports
{
    internal interface IExport
    {
        string Name { get; }
        ValueTask<string> Export();
        void InitVisual(Grid content) { }
    }
}
