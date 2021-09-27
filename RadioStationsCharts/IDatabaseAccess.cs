using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RadioStationsCharts
{
    interface IDatabaseAccess
    {
        void Connect();
        void ExecDatatableProcedure(string procedure, DataTable dt);
        DataTable ExecProcedureToDatatable(string procedure);
    }
}
