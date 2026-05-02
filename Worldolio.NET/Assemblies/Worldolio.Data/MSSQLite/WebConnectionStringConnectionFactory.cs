using Microsoft.Data.Sqlite;
using System.Data;

namespace Worldolio.Data.MSSQLite
{
    public class WebConnectionStringConnectionFactory : IConnectionFactory
    {
        private string _connectionString;

        public WebConnectionStringConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        IDbConnection IConnectionFactory.GetOpenConnection()
        {
            IDbConnection connection = new SqliteConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}
