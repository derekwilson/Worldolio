using System.Data;
using Dapper;
using Worldolio.Data.Model;
using Worldolio.Data.MSSQLite;

namespace Worldolio.Data.Repository
{
    public interface IDriveSideRepository
    {
        Task<DriveSide?> GetByIdAsync(long id);

        Task<ICollection<DriveSide>> GetAllAsync();
    }

    public class DriveSideRepository : IDriveSideRepository
    {
        private const string SQL_SELECT = @"
                    SELECT dsi.*
                    FROM dsi_driveside dsi
                    ";
        private const string SQL_WHERE_ID_SUFFIX = " WHERE dsi.dsi_id = @ID ";

        private const string SQL_SELECT_ALL = SQL_SELECT;
        private const string SQL_SELECT_BY_ID = SQL_SELECT + SQL_WHERE_ID_SUFFIX;

        private IConnectionFactory _connectionFactory;


        public DriveSideRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ICollection<DriveSide>> GetAllAsync()
        {
            using (IDbConnection connection = _connectionFactory.GetOpenConnection())
            {
                var result = await connection.QueryAsync<DriveSide>(SQL_SELECT_ALL);
                return result.ToList();
            }
        }

        public async Task<DriveSide?> GetByIdAsync(long id)
        {
            using (IDbConnection connection = _connectionFactory.GetOpenConnection())
            {
                var result = await connection.QueryAsync<DriveSide>(SQL_SELECT_BY_ID,new { ID = id });
                return result.FirstOrDefault();
            }
        }

    }
}
