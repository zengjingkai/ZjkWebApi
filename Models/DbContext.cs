using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZjkWebAPIDemo.Models
{
    public class DbContext
    {

        public string ConnectionString { get; set; }

        public DbContext(string connectionString)
        {
            ConnectionString = connectionString;
        }
        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}
