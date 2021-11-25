using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using RadioStationsCharts.Attributes;

namespace RadioStationsCharts.Controllers
{
    [Route("charts")]
    [ApiController]
    [ApiKey]
    public class ChartsController : ControllerBase
    {
        readonly DatabaseAccess db;

        public ChartsController(IConfiguration config)
        {
            db = new DatabaseAccess(config);
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ChartsScraping))]
        [ProducesResponseType(500, Type = typeof(string))]
        [Route("rmf")]
        public IActionResult GetRmfFmCharts()
        {
            try
            {
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
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ChartsScraping))]
        [ProducesResponseType(500, Type = typeof(string))]
        [Route("eska")]
        public IActionResult GetEskaCharts()
        {
            try
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

                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ChartsScraping))]
        [ProducesResponseType(500, Type = typeof(string))]
        [Route("radio-zet")]
        public IActionResult GetRadioZetCharts()
        {
            try
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

                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ChartsScraping))]
        [ProducesResponseType(500, Type = typeof(string))]
        [Route("vox-fm")]
        public IActionResult GetVoxFmCharts()
        {
            try
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

                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ChartsScraping))]
        [ProducesResponseType(500, Type = typeof(string))]
        [Route("polskie-radio-1")]
        public IActionResult GetPolskieRadio1Charts()
        {
            try
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

                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ChartsScraping))]
        [ProducesResponseType(500, Type = typeof(string))]
        [Route("trojka")]
        public IActionResult GetTrojkaCharts()
        {
            try
            {
                ChartsScraping list = new ChartsScraping();
                list.Charts = new List<Charts>();
                list.Station = "Trojka Polskie Radio";
                list.ChartsName = "Lista Przebojów Programu 3";
                DataTable charts = db.ExecProcedureToDatatable("GetTrojkaCharts");

                foreach (DataRow row in charts.Rows)
                {
                    Charts field = new Charts();
                    field.Position = Convert.ToInt32(row["Number"]);
                    field.Artist = row["Artist"].ToString();
                    field.Title = row["Title"].ToString();
                    list.Charts.Add(field);
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
