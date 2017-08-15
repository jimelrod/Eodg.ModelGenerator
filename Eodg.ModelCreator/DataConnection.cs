using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Eodg.ModelCreator
{
    public class DataConnection
    {
        private string _connectionString;

        public DataConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<T> RunQuery<T>(string query)
        {
            using (var connection = GetConnection())
            {
                try
                {
                    return connection.Query<T>(query);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        private IDbConnection GetConnection()
        {
            try
            {
                return new SqlConnection(_connectionString);
            }
            catch
            {
                Console.WriteLine($"Invalid connection string: {_connectionString}");
                Environment.Exit(0);
                return null;
            }
        }
    }
}
