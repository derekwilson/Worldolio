using Dapper;
using System.Data;
using Worldolio.Data.Model;
using Worldolio.Data.MSSQLite;

namespace Worldolio.Data.Repository
{
    public interface ISchemaRevisionAuditRepository
    {
        Task<ICollection<SchemaRevisionAudit>> GetAllAsync();

        Tuple<string, long> GetDatabaseSchemaVersions();
    }

    public class SchemaRevisionAuditRepository : ISchemaRevisionAuditRepository
    {
        private const string SQL_SELECT = @"
                    SELECT sra.*
                    FROM sra_schema_revision_audit sra
                    ";

        private const string SQL_SELECT_ALL = SQL_SELECT;

        private IConnectionFactory _connectionFactory;

        public SchemaRevisionAuditRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ICollection<SchemaRevisionAudit>> GetAllAsync()
        {
            using (IDbConnection connection = _connectionFactory.GetOpenConnection())
            {
                var result = await connection.QueryAsync<SchemaRevisionAudit>(SQL_SELECT_ALL);
                return result.ToList();
            }
        }

        public Tuple<string, long> GetDatabaseSchemaVersions()
        {
            using (IDbConnection connection = _connectionFactory.GetOpenConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT sqlite_version();";
                string version = command.ExecuteScalar()?.ToString() ?? "UNKNOWN";

                command.CommandText = "PRAGMA user_version;";
                long userVersion = (long)(command.ExecuteScalar() ?? -1);

                return Tuple.Create(version, userVersion);
            }
        }
    }
}
