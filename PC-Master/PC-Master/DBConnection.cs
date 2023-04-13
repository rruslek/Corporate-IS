using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Master
{
    class DBConnection
    {

        public static MySqlConnection
        GetDBConnection()
        {
            String connString = "server=localhost;user=root;database=mydb;password=870125";

            MySqlConnection conn = new MySqlConnection(connString);

            return conn;
        }

    }
}
