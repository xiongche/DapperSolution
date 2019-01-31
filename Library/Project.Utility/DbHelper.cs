using System.Configuration;

namespace System.Data.SqlClient
{
    public class DbHelper
    {
        public static SqlConnection GetConnection(string instance)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[instance].ToString();
            var sqlConnection = new SqlConnection(connectionString);

            if (sqlConnection.State == ConnectionState.Closed)
            {
                sqlConnection.Open();
            }

            return sqlConnection;
        }

        public static string GetConnectionString(string instance)
        {
            return ConfigurationManager.ConnectionStrings[instance].ToString();
        }

        public static bool HasConnection(string name)
        {
            return ConfigurationManager.ConnectionStrings[name] != null;
        }
    }
}
