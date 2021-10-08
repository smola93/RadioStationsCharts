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
        [HttpGet]
        [Route("eska")]
        public ChartsScraping GetEskaCharts()
        {
            ChartsScraping list = new ChartsScraping();
            list.Charts = new List<Charts>();
            list.Station = "Radio Eska";
            list.ChartsName = "Gorąca 20";
            DataTable charts = db.ExecProcedureToDatatable("GetEskaCharts");

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
        [HttpGet]
        [Route("radio-zet")]
        public ChartsScraping GetRadioZetCharts()
        {
            ChartsScraping list = new ChartsScraping();
            list.Charts = new List<Charts>();
            list.Station = "Radio Zet";
            list.ChartsName = "Lista Przebojów Radia Zet";
            DataTable charts = db.ExecProcedureToDatatable("GetRadioZetCharts");

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
        [HttpGet]
        [Route("vox-fm")]
        public ChartsScraping GetVoxFmCharts()
        {
            ChartsScraping list = new ChartsScraping();
            list.Charts = new List<Charts>();
            list.Station = "VOX FM";
            list.ChartsName = "Bestlista";
            DataTable charts = db.ExecProcedureToDatatable("GetVoxFmCharts");

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
        [HttpGet]
        [Route("polskie-radio-1")]
        public ChartsScraping GetPolskieRadio1Charts()
        {
            ChartsScraping list = new ChartsScraping();
            list.Charts = new List<Charts>();
            list.Station = "Polskie Radio 1";
            list.ChartsName = "Przeboje Przyjaciół Radiowej Jedynki";
            DataTable charts = db.ExecProcedureToDatatable("GetPolskieRadio1Charts");

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
