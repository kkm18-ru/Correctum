using System.Data;

namespace Correctum.Reports;

public interface IReport
{
    abstract static DataTable GenerateReport();
}
