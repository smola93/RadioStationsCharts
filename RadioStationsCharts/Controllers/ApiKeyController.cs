using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using RadioStationsCharts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace RadioStationsCharts.Controllers
{
    [Route("register")]
    [ApiController]
    public class ApiKeyController : ControllerBase
    {
        readonly DatabaseAccess db;
        private readonly IConfiguration Configuration;
        public ApiKeyController(IConfiguration config)
        {
            Configuration = config;
            db = new DatabaseAccess(config);
        }

        [HttpPost]
        public string RegisterAndGetApiKey(ApiKeyRequest request)
        {
            try
            {
                bool isValidEmail = IsValidEmail(request.Email);

                if (isValidEmail)
                {
                    string apiKey = GenerateApiKey();
                    CheckForEmailAndNameDuplication(request.Name, request.Email, apiKey);
                    string[] procedureParams = { request.Name, request.Email, apiKey };
                    db.ExecProcedureWithParameters("InsertUserDetails", procedureParams);
                    SendEmailWithApiKey(request.Email, apiKey);
                }
                else
                {
                    return "E-mail address is not valid";
                }

                return "ApiKey was generated successful. Check your E-mail to find it!";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
        private string GenerateApiKey()
        {
            var key = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);
            string apiKey = Convert.ToBase64String(key);
            return apiKey;
        }
        private bool IsValidEmail(string email)
        {
            if (email.Trim().EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        private void CheckForEmailAndNameDuplication(string name, string email, string apiKey)
        {
            string[] procedureParams = { name, email, apiKey };
            db.ExecProcedureWithParameters("CheckForDuplication", procedureParams);

        }
        private void SendEmailWithApiKey(string newUserEmail, string ApiKey)
        {
            string emailPass = Configuration.GetSection("MailPass").Value;
            string message = "Your secret ApiKey for RadioStationsCharts Is: " + ApiKey;
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("radiostationscharts@gmail.com", emailPass),
                    EnableSsl = true,
                };

                smtpClient.Send("radiostationscharts@gmail.com", newUserEmail, "Your ApiKey For RadioStationsCharts API!", message);
            }
            catch (SmtpException ex)
            {
                throw new ApplicationException
                  ("SmtpException has occured: " + ex.Message);
            }
        }
    }
}
