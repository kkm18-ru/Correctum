using System.Windows.Controls;

namespace RetailCorrector.Exporters
{
    public interface IExporter
    {
        string Name { get; }
        string? FileFilter { get; }
        ValueTask Export(string? path);
        void InitVisual(Grid content) { }
    }
}
