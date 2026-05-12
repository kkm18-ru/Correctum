using System.Data;

namespace Correctum.Abstractions;

public interface IReport
{
    abstract static DataTable GenerateReport();
}
