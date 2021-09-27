using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RadioStationsCharts.Controllers
{
    [Route("charts")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        readonly DatabaseAccess db;
        public ChartsController(IConfiguration config)
        {
            db = new DatabaseAccess(config);
        }
        [HttpGet]
        [Route("rmf")]
        public ChartsScraping GetRmfFmCharts() {
            ChartsScraping list = new ChartsScraping();
            list.Charts = new List<Charts>();
            list.Station = "Rmf FM";
            list.ChartsName = "POPlista";
            DataTable charts = db.ExecProcedureToDatatable("GetRmfCharts");
            
            foreach (DataRow row in charts.Rows)
            {
                Charts field = new Charts();
                field.Position = Convert.ToInt32(row["Number"]);
                field.Artist = row["Artist"].ToString();
                field.Title = row["Title"].ToString();
                list.Charts.Add(field);
            }
            
            return list;
        }
    }
}
