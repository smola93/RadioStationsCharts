using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RadioStationsCharts
{
    public class DatabaseAccess : IDatabaseAccess
    {
        public DatabaseAccess(IConfiguration config)
        {
            Configuration = config;
            Connect();
        }
        
        private readonly IConfiguration Configuration;
        SqlConnection connection;
        public void Connect()
        {
            try
            {
                string connetionString = Configuration.GetSection("ConnectionStrings").GetSection("DBConnString").Value;
                connection = new SqlConnection(connetionString);
                connection.Open();
                Console.WriteLine("Connection Open!");
                connection.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void ExecDatatableProcedure(string procedure, DataTable dt)
        {
            try
            {
                string connetionString = Configuration.GetSection("ConnectionStrings").GetSection("DBConnString").Value;
                connection = new SqlConnection(connetionString);
                connection.Open();
                SqlCommand sqlCommand = new SqlCommand(procedure, connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@ChartsDT", dt));
                sqlCommand.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable ExecProcedureToDatatable(string procedure)
        {
            try
            {
                DataTable table = new DataTable();
                SqlCommand sqlCommand = new SqlCommand(procedure, connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCommand);
                sqlAdapter.Fill(table);

                return table;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void ExecProcedureWithParameters(string procedure, string[] parameters)
        {
            try
            {
                string connetionString = Configuration.GetSection("ConnectionStrings").GetSection("DBConnString").Value;
                connection = new SqlConnection(connetionString);
                connection.Open();
                SqlCommand cmd = new SqlCommand(procedure, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Name", parameters[0]);
                cmd.Parameters.AddWithValue("@Email", parameters[1]);
                cmd.Parameters.AddWithValue("@ApiKey", parameters[2]);

                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string CheckApiKeyInDatabase(string apiKey)
        {
            try
            {
                object obj;
                string connetionString = Configuration.GetSection("ConnectionStrings").GetSection("DBConnString").Value;
                connection = new SqlConnection(connetionString);
                connection.Open();
                SqlCommand cmd = new SqlCommand("CheckApiKey", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ApiKey", apiKey);

                obj = cmd.ExecuteScalar();
                string key = (string)obj;
                connection.Close();

                return key;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void LogInDetailsToDatabaseAsync(HttpContext context, string responseMsg)
        {
            try
            {
                var task = PrepareDataForLoggingToDbAsync(context, responseMsg);
                string[] parameters = task.Result;
                task.Wait();
                ExecuteLoggingInToDb(parameters);
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<string[]> PrepareDataForLoggingToDbAsync(HttpContext context, string responseMsg)
        {
            ConnectionInfo conn = context.Connection;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            string remoteIpAddress = conn.RemoteIpAddress.ToString();
            string method = request.Method;
            string headers = string.Join(", ", request.Headers);
            string host = request.Headers["Host"];
            string date = DateTime.Now.ToString();

            string body = "";
            if (method.Equals("POST"))
            {
                request.EnableBuffering();
                request.Body.Seek(0, SeekOrigin.Begin);
                using (StreamReader stream = new StreamReader(request.Body))
                {
                    body = await stream.ReadToEndAsync();
                }
            }
            string code = response.StatusCode.ToString();
            string responseBody = responseMsg;


            string[] parameters = { remoteIpAddress, method, headers, host, date, body, responseBody, code };

            return parameters;
        }
        private void ExecuteLoggingInToDb (string[] parameters)
        {
            string connetionString = Configuration.GetSection("ConnectionStrings").GetSection("DBConnString").Value;
            connection = new SqlConnection(connetionString);
            connection.Open();
            SqlCommand cmd = new SqlCommand("LogCallDetails", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IP", parameters[0]);
            cmd.Parameters.AddWithValue("@HttpMethod", parameters[1]);
            cmd.Parameters.AddWithValue("@Headers", parameters[2]);
            cmd.Parameters.AddWithValue("@HostName", parameters[3]);
            cmd.Parameters.AddWithValue("@CallDate", parameters[4]);
            if (parameters[1].Equals("POST"))
            {
                cmd.Parameters.AddWithValue("@RequestBody", parameters[5]);
            }
            else
            {
                cmd.Parameters.AddWithValue("@RequestBody", "GET Method");
            }
            cmd.Parameters.AddWithValue("@ResponseBody", parameters[6]);
            cmd.Parameters.AddWithValue("@StatusCode", parameters[7]);

            cmd.ExecuteNonQuery();
            connection.Close();
        }
    }
}
