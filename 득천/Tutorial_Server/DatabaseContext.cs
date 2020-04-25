using MySql.Data.MySqlClient;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tutorial_Server
{
    public class DatabaseContext : IDisposable
    {
        MySqlConnection mConnection;
        public DatabaseContext(string connectionString)
        {
            mConnection = new MySqlConnection(connectionString);
            mConnection.Open();
        }


        public static DatabaseContext Open()
        {
            return new DatabaseContext("Server=15.164.165.164;Database=tutorialtest;User Id=root;Password=0000;");

        }

        public MySqlDataReader ExecuteQuery(string query)
        {
            var cmd = mConnection.CreateCommand();
            cmd.CommandText = query;
            return cmd.ExecuteReader();
        }

        public void Dispose()
        {
            mConnection.Close();
        }

        public int ExecuteNonQuery(string query)
        {
            var cmd = mConnection.CreateCommand();
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            return cmd.ExecuteNonQuery();
        }

    }

    public class ORMContext
    {
        static OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory("Server=15.164.165.164;Database=tutorialtest;User Id=root;Password=0000;", MySqlDialect.Provider); // ConnectionString
        public static System.Data.IDbConnection Open()
        {
            var con = factory.CreateDbConnection();
            con.Open();
            return con;
        }
    }
}
