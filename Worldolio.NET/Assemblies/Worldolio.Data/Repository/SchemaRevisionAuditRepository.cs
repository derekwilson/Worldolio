using Dapper;
using System.Data;
using Worldolio.Data.Model;
using Worldolio.Data.MSSQLite;

namespace Worldolio.Data.Repository
{
    public interface ISchemaRevisionAuditRepository
    {
        Task<ICollection<SchemaRevisionAudit>> GetAllAsync();
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
    }
}
