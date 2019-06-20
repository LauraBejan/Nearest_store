using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Oracle.ManagedDataAccess.Client;
using System.Configuration;

namespace distance_app
{
    public class Connection
    {
        public string connectionString = ConfigurationManager.ConnectionStrings["MyDataBase"].ConnectionString;
        public OracleConnection _connection = new OracleConnection();

        public Connection()
        {
        }
        public void Open()
        {

            _connection.ConnectionString = connectionString;
            _connection.Open();
            //string sql = "select * from image";

            //OracleCommand command = new OracleCommand(sql, _connection);
            //Console.WriteLine(command);
        }
        public void Close()
        {
            _connection.Close();
        }
    }
}
