using Microsoft.Data.Sqlite;
using System.Data;

namespace Worldolio.Data.MSSQLite
{
    public class LocalFileDbConnectionFactory : IConnectionFactory
    {
        string _dbFilePath;

        public LocalFileDbConnectionFactory(string filepath)
        {
            _dbFilePath = filepath;
        }

        public IDbConnection GetOpenConnection()
        {
            if (!File.Exists(_dbFilePath))
            {
                throw new FileNotFoundException("Database cannot be found", _dbFilePath);
            }
            IDbConnection connection = new SqliteConnection(string.Format("Data Source={0};", _dbFilePath));
            connection.Open();
            return connection;
        }
    }
}
