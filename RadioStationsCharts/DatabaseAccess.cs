using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;

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
    }
}
