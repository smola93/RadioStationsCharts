using Microsoft.AspNetCore.Http;
using System.Data;

namespace RadioStationsCharts
{
    interface IDatabaseAccess
    {
        void TestConnection();
        void ExecDatatableProcedure(string procedure, DataTable dt);
        DataTable ExecProcedureToDatatable(string procedure);
        void ExecProcedureWithParameters(string procedure, string[] parameters);
        string CheckApiKeyInDatabase(string apiKey);
        void LogInDetailsToDatabaseAsync(HttpContext context, string responseMsg);
    }
}
