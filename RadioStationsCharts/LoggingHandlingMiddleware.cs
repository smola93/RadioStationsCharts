using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioStationsCharts
{
    public class LoggingHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        readonly DatabaseAccess db;

        public LoggingHandlingMiddleware(RequestDelegate next, IConfiguration config)
        {
            db = new DatabaseAccess(config);
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;
                
                await _next(context);

                var response = await FormatResponse(context.Response);

                db.LogInDetailsToDatabaseAsync(context, response);

                //Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);

            string text = await new StreamReader(response.Body).ReadToEndAsync();

            response.Body.Seek(0, SeekOrigin.Begin);

            return text;
        }
    }
}
