using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System;
using adesoft.adepos.webview.Data.Interfaces;

namespace adesoft.adepos.webview.Data
{
    public class SqlManagerService : ISqlManagerService
    {
        private readonly IConfiguration _configuration;

        public SqlManagerService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public int ExecuteNoQuery(string queryString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable ExecuteQuery(string queryString, string connectionStringName)
        {
            try
            {
                DataTable dataTable = new DataTable();

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionStringName)))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.CommandTimeout = 300;
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    dataTable.Load(reader);

                    reader.Close();
                }

                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool BulkData(DataTable dataTable, string tableName, string connectionStringName)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString(connectionStringName);
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
                {
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    }

                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.BulkCopyTimeout = 600;
                    bulkCopy.WriteToServer(dataTable);
                    bulkCopy.Close();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int DeleteRecords(string tableName)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                var queryString = $"DELETE FROM {tableName}";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
