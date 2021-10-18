using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RadioStationsCharts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RadioStationsCharts.Controllers
{
    [Route("register")]
    [ApiController]
    public class ApiKeyController : ControllerBase
    {
        [HttpPost]
        public string RegisterAndGetApiKey(ApiKeyRequest request)
        {
            try
            {

                return "Ok";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
    }
}
